using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int healthLevel = 10;
    public int maxHealth;
    public int currentHealth;

    public int staminaLevel = 10;
    public int maxStamina;
    public int currentStamina;

    public float dmgModifier = 1;

    public HealthBar healthBar;
    public StaminaBar staminaBar;

    AnimatorHandler animatorHandler;

    private void Awake()
    {
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
        staminaBar = FindObjectOfType<StaminaBar>();
        healthBar = FindObjectOfType<HealthBar>();
    }

    private void Start()
    {
        maxHealth = SetMaxHealthFromLevel();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetCurrentHealth(currentHealth);

        maxStamina = SetMaxStaminaFromLevel();
        currentStamina = maxStamina;
        staminaBar.SetMaxStamina(maxStamina);
        staminaBar.SetCurrentStamina(currentStamina);
    }

    private int SetMaxHealthFromLevel()
    {
        maxHealth = healthLevel * 10;
        return maxHealth;
    }
    public void IncreaseHealthLevel(int levelIncrease)
    {
        healthLevel += levelIncrease;
        maxHealth = SetMaxHealthFromLevel();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetCurrentHealth(currentHealth);
    }

    private int SetMaxStaminaFromLevel()
    {
        maxStamina = staminaLevel * 10;
        return maxStamina;
    }

    public void IncreaseStaminaLevel(int levelIncrease)
    {
        staminaLevel += levelIncrease;
        maxStamina = SetMaxStaminaFromLevel();
        currentStamina = maxStamina;
        staminaBar.SetMaxStamina(maxStamina);
        staminaBar.SetCurrentStamina(currentStamina);
    }

    public void BuffDamage()
    {
        dmgModifier = 1.2f;
    }

    public void TakeDamage(int damage)
    {
        currentHealth = currentHealth - damage;

        healthBar.SetCurrentHealth(currentHealth);
        animatorHandler.PlayTargetAnimation("Damage_01", true);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            animatorHandler.PlayTargetAnimation("Death", true);
            // handle player death
        }
    }

    public void TakeStaminaDamage(int damage)
    {
        currentStamina = currentStamina - damage;
        if (currentStamina <= 0)
        {
            currentStamina = 0;
        }
        staminaBar.SetCurrentStamina(currentStamina);
    }
}
