using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    // Configurable properties
    [SerializeField] float baseEngineThrust = 50f;
    [SerializeField] float dashDistance = 2f;
    [SerializeField] float dashCooldown = 1f;
    [SerializeField] SphereCollider shipCollider;

    // Cached references
    private Rigidbody rb;
    private GameManager gameManager;

    // Internal fields
    private Vector2 _movementInput;
    private Vector2 _mostRecentMovementInput;
    private float _timeSinceLastDash = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void Update()
    {
        if (!gameManager.GameIsActive) return;
        _timeSinceLastDash += Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (!gameManager.GameIsActive)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        rb.AddForce(baseEngineThrust * Time.fixedDeltaTime * new Vector3(_movementInput.x, 0, _movementInput.y));
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
    }

    public void OnDash(InputValue value)
    {
        if (!gameManager.GameIsActive) return;
        if (value.isPressed && _timeSinceLastDash >= dashCooldown)
        {
            Vector3 dashDirection = new Vector3(_mostRecentMovementInput.x, 0, _mostRecentMovementInput.y).normalized;
            if (dashDirection != Vector3.zero)
            {
                Dash(dashDirection);
            }
        }
    }

    private void Dash(Vector3 direction)
    {
        // Raycast ship collider in the dash direction to check for collisions
        if (Physics.SphereCast(transform.position, shipCollider.radius, direction, out RaycastHit hit, dashDistance))
        {
            // If we hit something, dash only up to the hit point
            rb.MovePosition(transform.position + direction * hit.distance);
        }
        else
        {
            // No collision, dash full distance
            rb.MovePosition(transform.position + direction * dashDistance);
        }

        // Reset dash cooldown
        _timeSinceLastDash = 0f;
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Enemy")) {

            // Simple bounce effect
            Vector3 bounceDirection = (transform.position - other.transform.position).normalized;
            rb.AddForce(bounceDirection * 10f, ForceMode.Impulse);

            // Deal collision damage to enemy
            if (other.gameObject.TryGetComponent(out Health enemyHealth)) {
                enemyHealth.TakeDamage(100); // Deal damage to the enemy on collision
            }

            // Deal collision damage to player
            if (TryGetComponent(out Health playerHealth)) {
                playerHealth.TakeDamage(20); // Player takes damage on collision
            } else {
                Debug.LogWarning("Player does not have a Health component!");
            }
        }
    }
}
