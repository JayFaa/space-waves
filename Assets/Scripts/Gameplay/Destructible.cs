using UnityEngine;
using UnityEngine.SceneManagement;

public class Destructible : MonoBehaviour
{
    [SerializeField] int maxHealth = 100;

    [SerializeField] bool invincibleOnHit = false;
    [SerializeField] float invincibilityWindow = 0.2f;

    [SerializeField] bool hasShield = false;
    [SerializeField] int maxShield = 50;
    [SerializeField] float shieldRegenDelay = 3f;
    [SerializeField] float shieldRegenRate = 5f;

    [SerializeField] GameObject lootSpawnerPrefab;
    [SerializeField] int lootPerKill = 5;

    // Cached references
    private GameManager gameManager;
    private UIManager uiManager;

    private int _currentShield;
    private int _currentHealth;
    private float _leftoverDamage = 0f;
    private float _shieldRegenAccumulator = 0f;
    private float _timeSinceLastDamage = 0f;
    private Vector3 _mostRecentDamageDirection = Vector3.back;
    private bool _quitting = false;

    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        uiManager = FindFirstObjectByType<UIManager>();
    }

    void Start()
    {
        _currentShield = maxShield;
        _currentHealth = maxHealth;
        UpdateUI();
    }

    void Update()
    {
        if (!gameManager.GameIsActive) return;
        if (hasShield) RechargeShield();
    }

    void RechargeShield()
    {
        // Add to last damage taken timer, but don't let it grow unbounded
        if (_timeSinceLastDamage < shieldRegenDelay) _timeSinceLastDamage += Time.deltaTime;

        // Regenerate shield if enough time has passed since last damage
        if (_timeSinceLastDamage >= shieldRegenDelay && _currentShield < maxShield)
        {
            _shieldRegenAccumulator += shieldRegenRate * Time.deltaTime;
            int shieldToRegen = Mathf.FloorToInt(_shieldRegenAccumulator);
            if (shieldToRegen > 0)
            {
                _currentShield = Mathf.Min(_currentShield + shieldToRegen, maxShield);
                _shieldRegenAccumulator -= shieldToRegen;
                UpdateShieldUI();
            }
        }
    }

    public void TakeDamage(float damage, Vector3 direction)
    {
        if (invincibleOnHit && _timeSinceLastDamage < invincibilityWindow) return;

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

        _currentHealth -= flooredDamage;
        UpdateUI();

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        // Reload scene if player dies
        if (gameObject.CompareTag("Player")) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        Destroy(gameObject);
    }

    private void UpdateUI()
    {
        UpdateShieldUI();
        UpdateHealthUI();
    }

    private void UpdateShieldUI()
    {
        if (gameObject.CompareTag("Player"))
        {
            uiManager.UpdateShield(_currentShield, maxShield);
        }
    }

    private void UpdateHealthUI()
    {
        if (gameObject.CompareTag("Player"))
        {
            uiManager.UpdateHealth(_currentHealth, maxHealth);
        }
    }

    void OnDestroy()
    {
        // Spawn loot if an enemy dies
        if (!_quitting && gameObject.scene.isLoaded && lootSpawnerPrefab != null)
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
