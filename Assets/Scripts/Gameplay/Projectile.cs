using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private int damage = 25;

    public float Speed { get; set; }

    private Rigidbody _rb;
    private GameManager gameManager;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        gameManager = FindFirstObjectByType<GameManager>();
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
            enemyHealth.TakeDamage(damage, -other.contacts[0].normal);
        }

        Destroy(gameObject);
    }
}
