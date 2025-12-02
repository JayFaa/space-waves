using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject warningIconPrefab;
    [SerializeField] float warningDuration = 1.5f;
    [SerializeField] float healthScale = 2f;

    private GameManager gameManager;

    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    public IEnumerator SpawnEnemy(GameObject enemyPrefab, int wave)
    {
        return SpawnEnemyWithWarning(enemyPrefab, Mathf.Pow(healthScale, wave - 1));
    }

    private IEnumerator SpawnEnemyWithWarning(GameObject enemyPrefab, float healthScale)
    {
        // Instantiate warning icon
        GameObject warningIcon = Instantiate(warningIconPrefab, transform.position, Quaternion.AngleAxis(90, Vector3.right));

        // Wait for some time before spawning the next wave
        float elapsedTime = 0f;
        while (elapsedTime < warningDuration)
        {
            // If a wave is reset, make sure the warning icons get cleaned up before exiting
            if (gameManager.GameIsResetting)
            {
                Destroy(warningIcon);
                yield break;
            }

            // Do not progress the timer if the game is paused
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
        CreateEnemy(enemyPrefab, healthScale);
    }

    private void CreateEnemy(GameObject enemyPrefab, float healthScale)
    {
        Destructible enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity).GetComponent<Destructible>();
        if (enemy != null)
        {
            enemy.ScaleHealth(healthScale);
        }
    }
}
