using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float damage = 25;

    public float Speed { get; set; }

    private Rigidbody _rb;
    private GameManager gameManager;
    private StatsManager statsManager;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        gameManager = FindFirstObjectByType<GameManager>();
        statsManager = FindFirstObjectByType<StatsManager>();
    }

    void FixedUpdate()
    {
        if (!gameManager.GameIsActive) return;

        _rb.MovePosition(_rb.position + Speed * Time.fixedDeltaTime * transform.forward);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy") && other.gameObject.TryGetComponent(out Destructible enemyHealth))
        {
            float scaledDamage = statsManager.AttackDamageMultiplicativeModifier * (damage + statsManager.AttackDamageFlatModifier);
            Debug.Log($"Projectile dealing {scaledDamage} damage to enemy.");
            enemyHealth.TakeDamage(scaledDamage, -other.contacts[0].normal);
        }

        Destroy(gameObject);
    }
}
