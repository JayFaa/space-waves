using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] float spawnInterval = 1f;

    // Cached references
    private GameManager gameManager;

    // Internal fields
    private float _spawnTimeAccumulator = 0f;

    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void Update()
    {
        if (!gameManager.GameIsActive) return;

        if (gameManager.GameIsResetting)
        {
            _spawnTimeAccumulator = 0f;
            return;
        }

        if (_spawnTimeAccumulator < spawnInterval)
        {
            _spawnTimeAccumulator += Time.deltaTime;
        }
        else
        {
            Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            _spawnTimeAccumulator = 0f;
        }
    }
}
