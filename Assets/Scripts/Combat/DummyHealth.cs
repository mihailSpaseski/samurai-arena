using UnityEngine;

public class DummyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 2;
    [SerializeField] private HealthBarUI healthBar;  // assign in Inspector
    [SerializeField] private Vector3 healthBarOffset = new Vector3(0, 2.2f, 0);

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

    private void LateUpdate()
    {
        // keep bar above head as dummy moves
        if (healthBar != null)
            healthBar.transform.position = transform.position + healthBarOffset;
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}