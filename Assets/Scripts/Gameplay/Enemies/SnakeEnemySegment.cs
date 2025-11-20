using System.Collections.Generic;
using UnityEngine;

public class SnakeEnemySegment : MonoBehaviour
{
    [SerializeField] SnakeEnemyHead head;
    [SerializeField] SnakeEnemySegment linkedSegment;
    [SerializeField] float followDelay = 0.25f;
    [SerializeField] float deathDelay = 0.1f;

    private Rigidbody _rb;
    private GameManager gameManager;

    private bool _isSeparated = false;
    private Queue<Vector3> _positionQueue = new Queue<Vector3>();
    private Vector3 _initialPosition;
    private bool _destroying = false;

    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        _rb = GetComponent<Rigidbody>();

        _initialPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (!gameManager.GameIsActive) return;
        
        // Pass on current position to linked segment if present
        if (linkedSegment != null)
        {
            linkedSegment.EnqueuePosition(transform.position);
        }

        // If enough time has passed, move to the next position in the queue
        if (_positionQueue.Count * Time.fixedDeltaTime > followDelay)
        {
            // Calculate new position and rotation from queue
            Vector3 targetPosition = _positionQueue.Dequeue();
            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);
            _rb.MovePosition(targetPosition);
            _rb.MoveRotation(targetRotation);
            _rb.linearVelocity = Vector3.zero;
        }
        else if (_positionQueue.Count > 0)
        {
            Vector3 inferredPosition = Vector3.Lerp(_initialPosition, _positionQueue.Peek(), _positionQueue.Count * Time.fixedDeltaTime / followDelay);
            Quaternion inferredRotation = Quaternion.LookRotation(_positionQueue.Peek() - inferredPosition, Vector3.up);
            _rb.MovePosition(inferredPosition);
            _rb.MoveRotation(inferredRotation);
        }
    }

    public void SetSize(float startSize, float endSize)
    {
        float segmentCount = SegmentCount();
        float size = Mathf.Lerp(endSize, startSize, (segmentCount - 1) / segmentCount);
        transform.localScale = new Vector3(size, size, size);
        
        if (linkedSegment != null) linkedSegment.SetSize(size, endSize);
    }

    public void SetColor(Color startColor, Color endColor)
    {
        float segmentCount = SegmentCount();
        Color segmentColor = Color.Lerp(endColor, startColor, (segmentCount - 1) / segmentCount);
        if (TryGetComponent(out Renderer renderer))
        {
            renderer.material.color = segmentColor;
        }

        if (linkedSegment != null) linkedSegment.SetColor(segmentColor, endColor);
    }

    public int SegmentCount()
    {
        if (_isSeparated) return 0;
        if (linkedSegment == null) return 1;
        return 1 + linkedSegment.SegmentCount();
    }

    public void EnqueuePosition(Vector3 position)
    {
        _positionQueue.Enqueue(position);
    }

    private void SeparateFromHead()
    {
        // Nothing to do if already separated
        if (!_isSeparated)
        {
            int count = SegmentCount();
            SetSeparated();
            head.ReduceSegmentCount(count);
        }
    }

    public void SetSeparated()
    {
        if (!_destroying)
        {
            _isSeparated = true;
            gameObject.transform.parent = null;
        }

        if (linkedSegment != null) linkedSegment.SetSeparated();
    }

    void OnDestroy()
    {
        _destroying = true;

        // If not already separated, separate from head first
        if (!_isSeparated)
        {
            SeparateFromHead();
        }

        // Then start or continue to destroy all segments that are separated
        if (linkedSegment != null) Destroy(linkedSegment.gameObject, deathDelay);
    }
}
