using UnityEngine;

public class RocketHealth : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float invulnerabilityDuration = 0.6f;

    float currentHealth;
    float invulnerabilityTimer = 0f;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    private void Update()
    {
        if (invulnerabilityTimer > 0)
        {
            invulnerabilityTimer -= Time.deltaTime;
        }
    }

    public bool TakeDamage(float damageAmount)
    {
        if (invulnerabilityTimer > 0) return false;

        currentHealth = Mathf.Max(0f, currentHealth - damageAmount);
        invulnerabilityTimer = invulnerabilityDuration;

        UpdateUI();
        Debug.Log($"[RocketHealth] Took {damageAmount} damage! Remaining Health: {currentHealth}/{maxHealth}");

        return currentHealth <= 0f;
    }

    public void Heal(float healAmount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
        UpdateUI();
    }

    private void UpdateUI()
    {
        GameUI ui = FindAnyObjectByType<GameUI>();
        if (ui != null)
        {
            ui.UpdateHealthBar(currentHealth, maxHealth);
        }
    }
}
