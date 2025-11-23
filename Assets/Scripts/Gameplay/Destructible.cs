using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField] int maxHealth = 100;
    [SerializeField] GameObject invincibilityEffect;

    [SerializeField] bool hasShield = false;
    [SerializeField] int maxShield = 50;
    [SerializeField] float shieldRegenDelay = 3f;
    [SerializeField] float shieldRegenRate = 5f;

    [SerializeField] GameObject lootSpawnerPrefab;
    [SerializeField] int lootPerKill = 5;

    // Cached references
    private GameManager gameManager;
    private StatsManager statsManager;
    private ShipUIManager uiManager;
    private WaveManager waveManager;

    private int _currentShield;
    private int _currentHealth;
    private float _regenHealthAccumulator = 0f;
    private float _leftoverDamage = 0f;
    private float _shieldRegenAccumulator = 0f;
    private float _timeSinceLastDamage = 0f;
    private bool _isInvincible = false;
    private float _invincibilityTimer = 0f;
    private float _invincibilityWindow = 0f;
    private Vector3 _mostRecentDamageDirection = Vector3.back;
    private bool _quitting = false;

    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        statsManager = FindFirstObjectByType<StatsManager>();
        uiManager = FindFirstObjectByType<ShipUIManager>();
        waveManager = FindFirstObjectByType<WaveManager>();
    }

    void Start()
    {
        _currentShield = GetUpgradedMaxShield();
        _currentHealth = GetUpgradedMaxHealth();
        UpdateUI();
    }

    void Update()
    {
        if (!gameManager.GameIsActive) return;
        if (hasShield) RechargeShield();
        RechargeHealth();
        UpdateInvincibility();
    }

    private void UpdateInvincibility()
    {
        if (_isInvincible)
        {
            _invincibilityTimer += Time.deltaTime;
            if (_invincibilityTimer >= _invincibilityWindow)
            {
                _isInvincible = false;
                if (invincibilityEffect != null) invincibilityEffect.SetActive(false);
                _invincibilityTimer = 0f;
            }
        }
    }

    public void MakeInvincible(float duration)
    {
        _isInvincible = true;
        if (invincibilityEffect != null) invincibilityEffect.SetActive(true);
        _invincibilityWindow = duration;
        _invincibilityTimer = 0f;
    }

    private void RechargeHealth()
    {
        // Early out if no health regen configured
        if (statsManager.HealthRegenAtFullShield <= 0f) return;

        // Regenerate health if shield is full
        if (_currentShield >= GetUpgradedMaxShield()){
            _regenHealthAccumulator += statsManager.HealthRegenAtFullShield * Time.deltaTime;
            int healthToRegen = Mathf.FloorToInt(_regenHealthAccumulator);
            if (healthToRegen > 0)
            {
                Heal(healthToRegen);
                _regenHealthAccumulator -= healthToRegen;
            }
        }
    }

    private void RechargeShield()
    {
        // Add to last damage taken timer, but don't let it grow unbounded
        if (_timeSinceLastDamage < shieldRegenDelay) _timeSinceLastDamage += Time.deltaTime;

        int currentMaxShield = GetUpgradedMaxShield();

        // Regenerate shield if enough time has passed since last damage
        if (_timeSinceLastDamage >= shieldRegenDelay && _currentShield < currentMaxShield)
        {
            float adjustedShieldRegenRate = shieldRegenRate;
            if (gameObject.CompareTag("Player")) adjustedShieldRegenRate *= statsManager.ShieldRegenRateMultiplicative;
            
            _shieldRegenAccumulator += adjustedShieldRegenRate * Time.deltaTime;

            int shieldToRegen = Mathf.FloorToInt(_shieldRegenAccumulator);
            if (shieldToRegen > 0)
            {
                _currentShield = Mathf.Min(_currentShield + shieldToRegen, currentMaxShield);
                _shieldRegenAccumulator -= shieldToRegen;
                UpdateShieldUI();
            }
        }
    }

    public void TakeDamage(float damage, Vector3 direction)
    {
        if (_isInvincible) return;

        // Apply damage reduction if the player is being damaged
        if (gameObject.CompareTag("Player")) damage *= statsManager.DamageReductionMultiplicative;

        Debug.Log($"{gameObject.name} took {damage} damage.");

        if (damage > 0){
            _timeSinceLastDamage = 0f;
            _mostRecentDamageDirection = direction;
        }

        int flooredDamage = Mathf.FloorToInt(damage);
        _leftoverDamage += damage - flooredDamage;

        if (_leftoverDamage >= 1f)
        {
            flooredDamage += 1;
            _leftoverDamage -= 1f;
        }

        if (hasShield && _currentShield > 0)
        {
            int shieldDamage = Mathf.Min(_currentShield, flooredDamage);
            _currentShield -= shieldDamage;
            flooredDamage -= shieldDamage;
        }

        _currentHealth = Mathf.Max(_currentHealth - flooredDamage, 0);
        UpdateUI();

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        // Reload scene if player dies
        if (gameObject.CompareTag("Player")){
            UpdateUI();
            _currentHealth = GetUpgradedMaxHealth();
            _currentShield = GetUpgradedMaxShield();
            gameManager.ResetGame();
            waveManager.FailWave();
        }
        else
        {
            Destroy(gameObject);  
        }
    }

    public void UpdateUI()
    {
        UpdateShieldUI();
        UpdateHealthUI();
    }

    public void Heal(int amount)
    {
        _currentHealth = Mathf.Min(_currentHealth + amount, GetUpgradedMaxHealth());
        UpdateHealthUI();
    }

    private int GetUpgradedMaxHealth()
    {
        if (gameObject.CompareTag("Player"))
            return maxHealth + statsManager.MaxHealthBonusFlat;
        else
            return maxHealth;
    }

    private int GetUpgradedMaxShield()
    {
        if (gameObject.CompareTag("Player"))
            return maxShield + statsManager.ShieldBonusFlat;
        else
            return maxShield;
    }

    private void UpdateShieldUI()
    {
        if (gameObject.CompareTag("Player"))
        {
            uiManager.UpdateShield(_currentShield, GetUpgradedMaxShield());
        }
    }

    private void UpdateHealthUI()
    {
        if (gameObject.CompareTag("Player"))
        {
            uiManager.UpdateHealth(_currentHealth, GetUpgradedMaxHealth());
        }
    }

    void OnDestroy()
    {
        // Spawn loot if an enemy dies
        if (!_quitting && !gameManager.GameIsResetting && lootSpawnerPrefab != null)
        {
            GameObject lootSpawner = Instantiate(lootSpawnerPrefab, transform.position, Quaternion.LookRotation(_mostRecentDamageDirection, Vector3.up));
            lootSpawner.GetComponent<LootSpawner>().SpawnLoot(lootPerKill);
            Destroy(lootSpawner, 1f);
        }
    }

    void OnApplicationQuit()
    {
        _quitting = true;
    }
}
