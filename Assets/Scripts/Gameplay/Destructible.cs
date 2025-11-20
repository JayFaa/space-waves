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
    }

    void Start()
    {
        _currentShield = maxShield;
        _currentHealth = maxHealth;
    }

    void Update()
    {

        if (!gameManager.GameIsActive) return;

        if (hasShield) RechargeShield();

        // Expose health and shield for debugging until UI is implemented
        /// if (gameObject.CompareTag("Player")) Debug.Log($"Health: {_currentHealth} | Shield: {_currentShield}");
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

    void OnDestroy()
    {
        // Spawn loot if an enemy dies
        if (!_quitting && lootSpawnerPrefab != null)
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
