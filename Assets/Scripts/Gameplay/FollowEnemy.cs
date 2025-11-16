using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FollowEnemy : MonoBehaviour
{
    [SerializeField] float targetSpeed = 10f;

    private GameObject player;
    private GameManager gameManager;

    private Rigidbody _rb;
    private Vector3 _targetVelocity;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    void FixedUpdate()
    {
        if (!gameManager.GameIsActive){
            _rb.linearVelocity = Vector3.zero;
            return;
        }
        
        _targetVelocity = (player.transform.position - transform.position).normalized * targetSpeed;
        Debug.Log(_targetVelocity);
        _rb.linearVelocity = Vector3.Lerp(_rb.linearVelocity, _targetVelocity, 0.1f);
        transform.LookAt(player.transform);
    }
}
