using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    // Configurable properties
    [SerializeField] float baseEngineThrust = 50f;

    // Cached references
    Rigidbody rb;

    // Internal fields
    private Vector2 _movementInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rb.AddForce(baseEngineThrust * Time.fixedDeltaTime * new Vector3(_movementInput.x, 0, _movementInput.y));
    }

    public void OnMove(InputValue value)
    {
        _movementInput = value.Get<Vector2>();
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
