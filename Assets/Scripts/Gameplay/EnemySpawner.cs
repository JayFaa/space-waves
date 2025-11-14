using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] float spawnInterval = 1f;

    private float _spawnTimeAccumulator = 0f;

    void Update()
    {
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
