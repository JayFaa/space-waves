using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject warningIconPrefab;
    [SerializeField] float warningDuration = 1.5f;

    private GameManager gameManager;

    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    public IEnumerator SpawnEnemy(GameObject enemyPrefab)
    {
        return SpawnEnemyWithWarning(enemyPrefab);
    }

    private IEnumerator SpawnEnemyWithWarning(GameObject enemyPrefab)
    {
        // Instantiate warning icon
        GameObject warningIcon = Instantiate(warningIconPrefab, transform.position, Quaternion.AngleAxis(90, Vector3.right));

        // Wait for some time before spawning the next wave
        float elapsedTime = 0f;
        while (elapsedTime < warningDuration)
        {
            if (!gameManager.GameIsActive)
            {
                yield return null;
                continue;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Destroy warning icon
        Destroy(warningIcon);

        // Spawn the enemy
        CreateEnemy(enemyPrefab);
    }

    private void CreateEnemy(GameObject enemyPrefab)
    {
        Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }
}
