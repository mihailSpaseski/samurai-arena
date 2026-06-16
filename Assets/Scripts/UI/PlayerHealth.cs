using Photon.Pun;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 6;
    [SerializeField] private HealthBarUI healthBar;
    [SerializeField] private Vector3 healthBarOffset = new Vector3(0, 2.5f, 0);

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    private int currentHealth;

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

        if (networkedHealth != null)
        {
            if (GetComponent<PhotonView>().IsMine)
            {
                PlayerStats stats = GetComponent<PlayerStats>();

                if (DeathScreenUI.Instance != null && stats != null)
                    DeathScreenUI.Instance.ShowDeathScreen(
                        stats.DamageDealt,
                        stats.Kills
                    );
            }

            // RPC_Die lives in NetworkedHealth — call it through photonView
            GetComponent<PhotonView>().RPC("RPC_Die", RpcTarget.All);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}