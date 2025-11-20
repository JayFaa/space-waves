using Unity.VisualScripting;
using UnityEngine;

public class SnakeEnemyHead : MonoBehaviour
{
    [SerializeField] SnakeEnemySegment linkedSegment;
    [SerializeField] GameObject linkedSegmentPrefab;
    [SerializeField] float movementForce = 500f;
    [SerializeField] float turningRate = 5f;
    [SerializeField] float directionChangeIntervalMin = 1f;
    [SerializeField] float directionChangeIntervalMax = 2f;
    [SerializeField] Color startColor = Color.green;
    [SerializeField] Color endColor = Color.white;
    [SerializeField] float startSize = 1f;
    [SerializeField] float endSize = 0.2f;

    private Rigidbody _rb;
    private int _currentSegmentCount;
    private float _nextDirectionChangeTime;
    private float _directionChangeAccumulator = 0f;

    void Awake() {
        _rb = GetComponent<Rigidbody>();
        
        // Initialize direction change timing manually on Awake to avoid always turning the same direction first
        if (Random.value < 0.5f) {
            _directionChangeAccumulator = 0f;
        } else {
            _directionChangeAccumulator = -GetRandomChangeTime();
        }
        _nextDirectionChangeTime = GetRandomChangeTime();
    }

    void Start()
    {
        _currentSegmentCount = linkedSegment.SegmentCount();
        linkedSegment.SetSize(startSize, endSize);
        linkedSegment.SetColor(startColor, endColor);
    }

    private void FixedUpdate() {
        // First, pass on current position to linked segment if present
        if (linkedSegment != null) linkedSegment.EnqueuePosition(transform.position);

        // Update direction change accumulator
        _directionChangeAccumulator += Time.fixedDeltaTime;

        // Change direction if interval has passed
        if (_directionChangeAccumulator >= _nextDirectionChangeTime) {
            SetNewDirectionChangeTime();
        }

        // Rotate transform based on sign of accumulator
        transform.Rotate(Vector3.up, Mathf.Sign(_directionChangeAccumulator) * turningRate * Time.fixedDeltaTime);

        _rb.AddForce(movementForce * Time.fixedDeltaTime * transform.forward);
    }

    private void SetNewDirectionChangeTime()
    {
        // This will cross zero after a random interval and then be reset again after another random interval
        _nextDirectionChangeTime = GetRandomChangeTime();
        _directionChangeAccumulator = -GetRandomChangeTime();
    }

    private float GetRandomChangeTime()
    {
        return Random.Range(directionChangeIntervalMin, directionChangeIntervalMax);
    }

    public void ReduceSegmentCount(int count) {
        _currentSegmentCount -= count;

        if(_currentSegmentCount < 1) {
            DestroySegments();
            Destroy(gameObject);
        }
    }

    private void DestroySegments()
    {
        if (linkedSegment != null) 
        {
            linkedSegment.SetSeparated();
            linkedSegment.UnsetParent();
            Destroy(linkedSegment.gameObject);
        }
    }

    void OnDestroy()
    {
        // Destroy all attached segments when head is destroyed
        DestroySegments();

        // Also destroy the parent container when the head is destroyed (segments are already unparented)
        Destroy(gameObject.transform.parent.gameObject);
    }
}