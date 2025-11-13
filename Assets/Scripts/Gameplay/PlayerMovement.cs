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
}
