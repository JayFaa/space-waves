using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] float emptyFieldPollingInterval = .5f;
    [SerializeField] float waveIntroTime = 1.5f;

    [SerializeField] TMPro.TMP_Text waveTitleText;
    [SerializeField] TMPro.TMP_Text enemiesRemainingText;
    [SerializeField] TMPro.TMP_Text centerScreenText;

    [SerializeField] GameObject followEnemyPrefab;
    [SerializeField] GameObject snakeEnemyPrefab;

    [SerializeField] GameObject outerCircleSpawnersParent;
    [SerializeField] GameObject middleCircleSpawnersParent;
    [SerializeField] GameObject innerCircleSpawnersParent;

    [SerializeField] GameObject outerSquareSpawnersParent;
    [SerializeField] GameObject outerSquareHalfSpawnersParent;
    [SerializeField] GameObject innerSquareSpawnersParent;

    [SerializeField] EnemySpawner centerSpawner;

    [SerializeField] AudioClip playerExplosionSound;
    [SerializeField] float playerExplosionSoundVolume = 1f;

    private GameManager gameManager;

    private List<EnemySpawner> _outerCircleSpawners;
    private List<EnemySpawner> _middleCircleSpawners;
    private List<EnemySpawner> _innerCircleSpawners;

    private List<EnemySpawner> _outerSquareSpawners;
    private List<EnemySpawner> _outerSquareHalfSpawners;
    private List<EnemySpawner> _innerSquareSpawners;

    private List<EnemySpawner> _centerSpawners;

    private int _currentWave = 0;
    private Coroutine _currentWaveCoroutine;
    private bool _waveStarted = false;

    void Awake()
    {
        if (FindAnyObjectByType<WaveManager>() != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        gameManager = FindFirstObjectByType<GameManager>();

        _outerCircleSpawners = outerCircleSpawnersParent.GetComponentsInChildren<EnemySpawner>().ToList();
        _middleCircleSpawners = middleCircleSpawnersParent.GetComponentsInChildren<EnemySpawner>().ToList();
        _innerCircleSpawners = innerCircleSpawnersParent.GetComponentsInChildren<EnemySpawner>().ToList();

        _outerSquareSpawners = outerSquareSpawnersParent.GetComponentsInChildren<EnemySpawner>().ToList();
        _outerSquareHalfSpawners = outerSquareHalfSpawnersParent.GetComponentsInChildren<EnemySpawner>().ToList();
        _innerSquareSpawners = innerSquareSpawnersParent.GetComponentsInChildren<EnemySpawner>().ToList();

        _centerSpawners = new List<EnemySpawner> { centerSpawner };
    }

    void Update()
    {
        // Nothing to do if there's already a wave active
        if (_waveStarted) return;

        Debug.Log($"Update starting Wave {_currentWave}.");
        _waveStarted = true;

        // If a wave is not active, start whichever one we're on
        _currentWaveCoroutine = StartCoroutine(_currentWave switch
            {
                0 => WaveZero(), // Wave zero can be used for tutorial time before enemies spawn
                1 => WaveBaseCoroutine(WaveOne()),
                2 => WaveBaseCoroutine(WaveTwo()),
                3 => WaveBaseCoroutine(WaveThree()),
                4 => WaveBaseCoroutine(WaveFour()),
                5 => WaveBaseCoroutine(WaveFive()),
                6 => WaveBaseCoroutine(WaveSix()),
                _ => GameCompleteCoroutine(),
            }
        );
    }

    private IEnumerator WaveBaseCoroutine(IEnumerator coreWaveCoroutine)
    {
        yield return WaveIntroCoroutine();
        yield return coreWaveCoroutine;
        yield return WaitForEmptyField();
        yield return WaveCompleteCoroutine();
        EndWave(nextWave: _currentWave + 1);
    }

    // *************************
    // Wave Implementations
    // *************************

    private IEnumerator WaveZero()
    {
        Debug.Log("Starting tutorial wait period.");

        waveTitleText.text = "Welcome!";
        waveTitleText.gameObject.SetActive(true);

        // Poll the game manager to see when the tutorial is complete and the first wave can start
        WaitForSeconds pollingInterval = new WaitForSeconds(emptyFieldPollingInterval);
        while (gameManager.PlayerIsInTutorial)
        {
            yield return pollingInterval;
        }

        Debug.Log("Tutorial complete, starting Wave One.");

        EndWave(nextWave: _currentWave + 1);
    }

    private IEnumerator WaveOne()
    {
        // Three waves of follow enemies from the middle circle.
        yield return PatternWave(_middleCircleSpawners.ToList(), new List<EnemySpawner>(), waves: 3, timeBetweenWaves: 10f, waitAfterLastSpawn: false);
    }

    private IEnumerator WaveTwo()
    {
        // Another three waves of follow enemies from the middle circle, with less time between.
        yield return PatternWave(_middleCircleSpawners.ToList(), new List<EnemySpawner>(), waves: 3, timeBetweenWaves: 2f, waitAfterLastSpawn: false);
    }

    private IEnumerator WaveThree()
    {
        // A wave from the outer and middle circles
        yield return PatternWave(_outerCircleSpawners.Concat(_middleCircleSpawners).ToList(), new List<EnemySpawner>(), waves: 1, timeBetweenWaves: 2.5f);
    
        // Followed by a wave from the middle circle and a snake in the center
        yield return PatternWave(_middleCircleSpawners.ToList(), _centerSpawners.ToList(), waves: 1, timeBetweenWaves: 5f, waitAfterLastSpawn: false);
    }

    private IEnumerator WaveFour()
    {
        // An inner circle of snakes
        yield return PatternWave(new List<EnemySpawner>(), _innerCircleSpawners.ToList(), waves: 1, timeBetweenWaves: 5f);

        // Then an outer square of follows and a middle circle of snakes
        yield return PatternWave(_outerSquareSpawners.ToList(), _middleCircleSpawners.ToList(), waves: 1, timeBetweenWaves: 10f, waitAfterLastSpawn: false);
    }

    private IEnumerator WaveFive()
    {
        // An Inner circle of follows and an outer square of snakes to sneak up on the player
        yield return PatternWave(_innerCircleSpawners.ToList(), _outerSquareHalfSpawners.ToList(), waves: 1, timeBetweenWaves: 2f);

        // Followed quickly by a middle circle of follows
        yield return PatternWave(_middleCircleSpawners.ToList(), new List<EnemySpawner>(), waves: 1, timeBetweenWaves: 2f);

        // And then an outer circle of follows and an inner circle of snakes
        yield return PatternWave(_outerCircleSpawners.ToList(), _innerCircleSpawners.ToList(), waves: 1, timeBetweenWaves: 5f, waitAfterLastSpawn: false);
    }

    private IEnumerator WaveSix()
    {
        // A random mix of enemies from all spawn points
        List<EnemySpawner> spawners = new List<EnemySpawner>();
        spawners.AddRange(_middleCircleSpawners);
        spawners.AddRange(_innerCircleSpawners);
        spawners.AddRange(_outerSquareHalfSpawners);
        spawners.AddRange(_innerSquareSpawners);
        spawners.AddRange(_centerSpawners);

        yield return RandomMobWave(spawners, followEnemyWeight: 0.8f, snakeEnemyWeight: 0.2f, waves: 5, timeBetweenWaves: 9f);
    }

    private Coroutine SpawnEnemyCoroutine(GameObject enemyPrefab, EnemySpawner spawner, int wave)
    {
        return StartCoroutine(spawner.SpawnEnemy(enemyPrefab, wave));
    }

    private IEnumerator PatternWave(List<EnemySpawner> followEnemySpawners, List<EnemySpawner> snakeEnemySpawners, int waves, float timeBetweenWaves, bool waitAfterLastSpawn=true)
    {
        List<Coroutine> spawnCoroutines = new List<Coroutine>();
        for (int i = 0; i < waves; i++)
        {
            // Spawn enemies at random spawn points
            foreach (var spawner in followEnemySpawners)
            {
                spawnCoroutines.Add(SpawnEnemyCoroutine(followEnemyPrefab, spawner, _currentWave));
            }
            foreach (var spawner in snakeEnemySpawners)
            {
                spawnCoroutines.Add(SpawnEnemyCoroutine(snakeEnemyPrefab, spawner, _currentWave));
            }

            // Wait for all spawns to complete
            foreach (var coroutine in spawnCoroutines) { yield return coroutine; }

            // For all but the final wave, wait before spawning again
            if (waitAfterLastSpawn || i < waves - 1)
            {
                // Wait for some time before spawning the next wave -- the spawn time plus the wave time
                yield return WaitForSecondsPausable(1.5f + timeBetweenWaves);
            }
        }
    }

    private IEnumerator RandomMobWave(List<EnemySpawner> spawnPoints, float followEnemyWeight, float snakeEnemyWeight, int waves, float timeBetweenWaves)
    {
        for (int i = 0; i < waves; i++)
        {
            // Spawn enemies at random spawn points
            List<Coroutine> spawnCoroutines = new List<Coroutine>();
            foreach (var spawner in spawnPoints)
            {
                float roll = Random.Range(0f, followEnemyWeight + snakeEnemyWeight);
                GameObject enemyPrefab = roll <= followEnemyWeight ? followEnemyPrefab : snakeEnemyPrefab;
                spawnCoroutines.Add(SpawnEnemyCoroutine(enemyPrefab, spawner, _currentWave));
            }

            // Wait for all spawns to complete
            foreach (var coroutine in spawnCoroutines) { yield return coroutine; }

            // Wait for some time before spawning the next wave
            yield return WaitForSecondsPausable(timeBetweenWaves);
        }
    }

    // *************************
    // Wave Helper Coroutines
    // *************************

    private IEnumerator WaveIntroCoroutine()
    {
        // Disable and update wave title text
        waveTitleText.gameObject.SetActive(false);
        waveTitleText.text = _currentWave == 6 ? "Final Wave" : $"Wave {_currentWave}";

        // Show center screen wave intro text
        centerScreenText.text = _currentWave == 6 ? "Final Wave" : $"Wave {_currentWave}";
        centerScreenText.gameObject.SetActive(true);

        yield return new WaitForSeconds(waveIntroTime);

        // Swap center screen message for title text
        centerScreenText.gameObject.SetActive(false);
        waveTitleText.gameObject.SetActive(true);

        yield return new WaitForSeconds(.5f);
    }

    private IEnumerator WaveCompleteCoroutine()
    {
        Debug.Log($"Wave {_currentWave} complete!");

        centerScreenText.text = _currentWave == 6 ? "Final Wave Complete!" : $"Wave {_currentWave} Complete!";
        centerScreenText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);
        centerScreenText.gameObject.SetActive(false);

        yield return new WaitForSeconds(.5f);
    }

    private IEnumerator WaveFailedCoroutine()
    {
        Debug.Log("Wave failed!");
        enemiesRemainingText.gameObject.SetActive(false);
        AudioManager.PlayClipAtPoint(playerExplosionSound, transform.position, playerExplosionSoundVolume);

        yield return new WaitForSeconds(1f);

        waveTitleText.gameObject.SetActive(false);
        centerScreenText.text = $"Returning to previous wave...";
        centerScreenText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        centerScreenText.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);

        EndWave(nextWave: Mathf.Max(1, _currentWave - 1));
    }

    private IEnumerator GameCompleteCoroutine()
    {
        // Game complete - no more waves
        Debug.Log("All waves complete!");

        centerScreenText.text = $"Congratulations! You have defeated the evil Cubists!";
        centerScreenText.gameObject.SetActive(true);

        waveTitleText.gameObject.SetActive(false);

        yield return null;
    }

    private IEnumerator WaitForSecondsPausable(float seconds, bool breakOnNoEnemies = true)
    {
        float elapsedTime = 0f;
        while (elapsedTime < seconds)
        {
            if (!gameManager.GameIsActive)
            {
                yield return null;
                continue;
            }

            if (breakOnNoEnemies && FindFirstObjectByType<FollowEnemy>() == null && FindFirstObjectByType<SnakeEnemyHead>() == null)
            {
                // No enemies remain, fast-forward the wait
                break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator WaitForEmptyField()
    {
        // Show enemies remaining text
        enemiesRemainingText.text = BuildEnemiesRemainingText();
        enemiesRemainingText.gameObject.SetActive(true);

        WaitForSeconds pollingIntervalWaitForSeconds = new WaitForSeconds(emptyFieldPollingInterval);
        while (FindFirstObjectByType<FollowEnemy>() != null || FindFirstObjectByType<SnakeEnemyHead>() != null)
        {
            yield return pollingIntervalWaitForSeconds;
            enemiesRemainingText.text = BuildEnemiesRemainingText();
        }

        // Hide enemies remaining text
        enemiesRemainingText.gameObject.SetActive(false);
    }

    private void EndWave(int nextWave)
    {
        Debug.Log($"Ending Wave {_currentWave}. Next Wave: {nextWave}");
        _currentWave = nextWave;
        _waveStarted = false;
    }

    public void FailWave()
    {
        if (_currentWaveCoroutine != null)
        {
            StopCoroutine(_currentWaveCoroutine);
            _currentWaveCoroutine = null;
        }
        StartCoroutine(WaveFailedCoroutine());
    }

    private string BuildEnemiesRemainingText()
    {
        int followEnemyCount = FindObjectsByType<FollowEnemy>(FindObjectsSortMode.None).Length;
        int snakeEnemyCount = FindObjectsByType<SnakeEnemyHead>(FindObjectsSortMode.None).Length;
        int totalEnemies = followEnemyCount + snakeEnemyCount;
        return $"Enemies remaining: {totalEnemies}";
    }
}
