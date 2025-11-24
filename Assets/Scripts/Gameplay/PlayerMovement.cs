using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    // Configurable properties
    [SerializeField] float baseEngineThrust = 50f;
    [SerializeField] float dashDistance = 2f;
    [SerializeField] float dashCooldown = 1f;
    [SerializeField] float collisionDamageDealt = 100f;
    [SerializeField] float collisionDamageTaken = 20f;
    [SerializeField] AudioClip[] collisionSounds;
    [SerializeField] AudioClip bigCollisionSound;
    [SerializeField] float collisionSoundVolume = 0.5f;
    [SerializeField] float bigCollisionSoundVolume = 1f;
    [SerializeField] AudioClip dashSound;
    [SerializeField] float dashSoundVolume = 0.8f;

    [SerializeField] SphereCollider shipCollider;
    [SerializeField] SphereCollider lootCollider;

    [SerializeField] CollisionParticleEffect collisionEffect;

    // Cached references
    private Rigidbody rb;
    private Destructible playerDestructible;
    private GameManager gameManager;
    private StatsManager statsManager;

    // Internal fields
    private Vector2 _movementInput;
    private Vector2 _mostRecentMovementInput;
    private float _timeSinceLastDash;
    private bool _dealBonusDamage = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerDestructible = GetComponent<Destructible>();

        gameManager = FindFirstObjectByType<GameManager>();
        statsManager = FindFirstObjectByType<StatsManager>();

        // Allow player to dash immediately at the start of the game
        _timeSinceLastDash = dashCooldown * statsManager.DashCooldownReductionMultiplicative;
    }

    void Update()
    {
        if (!gameManager.GameIsActive) return;
        _timeSinceLastDash += Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (!gameManager.GameIsActive || gameManager.GameIsResetting)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        rb.AddForce(baseEngineThrust * Time.fixedDeltaTime * statsManager.MovementSpeedMultiplicative * new Vector3(_movementInput.x, 0, _movementInput.y));
    }

    public void OnMove(InputValue value)
    {
        // Set actual current movement input
        _movementInput = value.Get<Vector2>();

        // Keep track of most recent non-zero movement input
        if (_movementInput != Vector2.zero)
        {
            _mostRecentMovementInput = _movementInput;
        }

        gameManager.AcknowledgeTutorialMovement();
    }

    public void OnDash(InputValue value)
    {
        if (!gameManager.GameIsActive) return;
        if (value.isPressed && _timeSinceLastDash >= dashCooldown * statsManager.DashCooldownReductionMultiplicative)
        {
            Vector3 dashDirection = new Vector3(_mostRecentMovementInput.x, 0, _mostRecentMovementInput.y).normalized;
            if (dashDirection != Vector3.zero)
            {
                Dash(dashDirection);
            }
        }
        gameManager.AcknowledgeTutorialDashing();
    }

    private void Dash(Vector3 direction)
    {

        // Raycast ship collider in the dash direction to check for collisions
        float distance;
        float maxDashDistance = dashDistance * statsManager.DashDistanceBonusMultiplicative;
        if (Physics.SphereCast(transform.position, shipCollider.radius, direction, out RaycastHit hit, maxDashDistance, ~LayerMask.GetMask("Loot")))
        {
            // If we hit something, dash only up to the hit point
            distance = hit.distance;
            _dealBonusDamage = true;
        }
        else
        {
            // No collision, dash full distance
            distance = maxDashDistance;
        }

        RaycastHit[] lootHits = Physics.SphereCastAll(transform.position, lootCollider.radius, direction, distance, LayerMask.GetMask("Loot"));
        foreach (RaycastHit lootHit in lootHits)
        {
            // Destroy loot objects in the dash path
            if (lootHit.collider.gameObject.TryGetComponent(out LootChunk lootChunk))
            {
                lootChunk.Collect();
            }
        }
        rb.MovePosition(transform.position + direction * distance);
        float dashInvincibilityDuration = statsManager.DashInvincibilityDuration;
        if (dashInvincibilityDuration > 0f) playerDestructible.MakeInvincible(dashInvincibilityDuration);

        AudioManager.PlayClipAtPoint(dashSound, transform.position, dashSoundVolume);

        // Reset dash cooldown
        _timeSinceLastDash = 0f;
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Enemy")) {

            // Simple bounce effect
            Vector3 bounceDirection = (transform.position - other.transform.position).normalized;
            rb.AddForce(bounceDirection * 10f, ForceMode.Impulse);

            // Deal collision damage to enemy
            if (other.gameObject.TryGetComponent(out Destructible enemyHealth)) {
                float damageAmount = _dealBonusDamage ? collisionDamageDealt * statsManager.DashSlamDamageMultiplicative : collisionDamageDealt;
                enemyHealth.TakeDamage(damageAmount, -other.contacts[0].normal);
            }

            // Deal collision damage to player
            if (TryGetComponent(out Destructible playerHealth)) {
                playerHealth.TakeDamage(collisionDamageTaken, other.contacts[0].normal);
            } else {
                Debug.LogWarning("Player does not have a Health component!");
            }
        }
        if (_dealBonusDamage) AudioManager.PlayClipAtPoint(bigCollisionSound, other.contacts[0].point, bigCollisionSoundVolume);
        else PlayCollisionSound(other.contacts[0].point);

        // Trigger collision particle effect
        if (collisionEffect != null)
        {
            Debug.Log("Spawning collision effect");
            Instantiate(collisionEffect, other.contacts[0].point, Quaternion.LookRotation(other.contacts[0].normal));
        }

        // Reset bonus damage flag even if the thing hit wasn't dealt damage
        _dealBonusDamage = false;
    }

    private void PlayCollisionSound(Vector3 position)
    {
        if (collisionSounds.Length == 0) return;

        AudioClip clip = collisionSounds[Random.Range(0, collisionSounds.Length)];
        AudioManager.PlayClipAtPoint(clip, position, collisionSoundVolume);
    }
}
