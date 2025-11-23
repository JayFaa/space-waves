using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool GameIsActive { get; private set; } = false;
    public bool GameIsResetting { get; private set; } = false;
    public bool PlayerIsInTutorial { get; private set; } = false;

    private Canvas _treeCanvas;
    private Destructible _playerDestructible;
    private Transform _playerTransform;

    void Awake()
    {
        if (FindAnyObjectByType<GameManager>() != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        GameIsActive = true;

        _treeCanvas = FindFirstObjectByType<Canvas>(FindObjectsInactive.Include);

        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        _playerTransform = player.transform;
        _playerDestructible = player.GetComponent<Destructible>();
    }

    public void PauseGame()
    {
        GameIsActive = false;
        _treeCanvas.gameObject.SetActive(true);
        StopAllCoroutines();
    }

    public void UnpauseGame(float delay)
    {
        StartCoroutine(UnpauseAfterDelay(delay));
    }

    IEnumerator UnpauseAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        GameIsActive = true;
        _treeCanvas.gameObject.SetActive(false);
    }

    public void ResetGame()
    {
        GameIsResetting = true;
        StartCoroutine(ResetGameCoroutine());
    }

    private IEnumerator ResetGameCoroutine()
    {
        // Clean up all interactable game objects except the player
        foreach (var enemy in FindObjectsByType<FollowEnemy>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            Destroy(enemy.gameObject);
        }
        foreach (var enemy in FindObjectsByType<SnakeEnemyHead>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            Destroy(enemy.gameObject);
        }
        foreach (var loot in FindObjectsByType<LootChunk>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            Destroy(loot.gameObject);
        }

        yield return new WaitForSeconds(3f);

        _playerDestructible.UpdateUI();
        _playerTransform.position = Vector3.zero;
        GameIsResetting = false;
    }
}
