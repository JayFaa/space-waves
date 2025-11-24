using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float damage = 25;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] hitSounds;
    [SerializeField] private float hitSoundVolume = .25f;

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

            PlayHitSound(other.contacts[0].point);
        }

        Destroy(gameObject);
    }

    private void PlayHitSound(Vector3 position)
    {
        if (hitSounds.Length == 0) return;

        AudioClip clip = hitSounds[Random.Range(0, hitSounds.Length)];
        AudioManager.PlayClipAtPoint(clip, position, hitSoundVolume);
    }
}
