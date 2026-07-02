using Photon.Pun;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 6;
    [SerializeField] private HealthBarUI healthBar;
    [SerializeField] private Vector3 healthBarOffset = new Vector3(0, 2.5f, 0);
    [SerializeField] private Animator animator;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    private int currentHealth;
    private bool isInvulnerable = false;

    public void SetInvulnerable(bool state)
    {
        isInvulnerable = state;
    }

    private void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.transform.position = transform.position + healthBarOffset;
            healthBar.Initialize(maxHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable)
            return;

        currentHealth -= damage;

        if (healthBar != null)
            healthBar.UpdateHealth(currentHealth, maxHealth);

        if (currentHealth <= 0)
            Die();
    }

    public void UpdateHealthBar(int current, int max)
    {
        if (healthBar != null)
            healthBar.UpdateHealth(current, max);
    }

    private void LateUpdate()
    {
        if (healthBar != null)
            healthBar.transform.position = transform.position + healthBarOffset;
    }

    private void Die()
    {
        NetworkedHealth networkedHealth = GetComponent<NetworkedHealth>();

        if (animator != null)
            animator.SetBool("IsDead", true);

        if (networkedHealth != null)
        {
            PhotonView pv = GetComponent<PhotonView>();

            if (pv.IsMine)
            {
                PlayerStats stats = GetComponent<PlayerStats>();

                if (DeathScreenUI.Instance != null && stats != null)
                {
                    Debug.Log("DMG: " + stats.DamageDealt);
                    DeathScreenUI.Instance.ShowDeathScreen(
                        stats.DamageDealt,
                        stats.Kills
                    );
                }

                // report this death to GameManager
                GameManager.Instance?.ReportDeath(pv.Owner.ActorNumber);

                if (GameManager.Instance == null)
                    Debug.LogError("GameManager.Instance is NULL!");
            }

            pv.RPC("RPC_Die", RpcTarget.All);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}