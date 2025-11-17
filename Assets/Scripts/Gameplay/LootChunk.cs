using UnityEngine;

public class LootChunk : MonoBehaviour
{
    [SerializeField] float accelerationTowardsMiddle = 2f;

    private GameManager gameManager;

    private Rigidbody _rb;

    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        _rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // TODO: save previous velocity to restore after unpausing game
        if (!gameManager.GameIsActive){
            _rb.linearVelocity = Vector3.zero;
            return;
        }

        Vector3 accelerationDirection = new(0f, _rb.position.y > 0f ? -accelerationTowardsMiddle : accelerationTowardsMiddle, 0f);
        _rb.AddForce(accelerationDirection, ForceMode.Acceleration);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    public void Collect()
    {
        Destroy(gameObject);
    }
}
