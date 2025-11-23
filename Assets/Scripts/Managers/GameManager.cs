using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text tutorialText;
    [SerializeField] PlayerInput movementInput;
    [SerializeField] PlayerInput shootInput;
    [SerializeField] PlayerInput upgradeInput;
    
    public bool GameIsActive { get; private set; } = false;
    public bool GameIsResetting { get; private set; } = false;
    public bool PlayerIsInTutorial { get; private set; } = true;

    // Tutorial flags
    private bool _movementAcknowledged = false;
    private bool _shootAcknowledged = false;
    private bool _dashAcknowledged = false;
    private bool _pauseAcknowledged = false;
    private bool _unpauseAcknowledged = false;

    private Canvas _treeCanvas;
    private Destructible _playerDestructible;
    private Transform _playerTransform;
    private Coroutine _unpauseCoroutine;

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

        // Unlock controls as the tutorial is completed
        movementInput.actions.FindAction("Move").Disable();
        movementInput.actions.FindAction("Dash").Disable();
        shootInput.actions.FindAction("Shoot").Disable();
        upgradeInput.actions.FindAction("Swap").Disable();
    }

    void Start()
    {
        StartCoroutine(TutorialCoroutine());
    }


    public void PauseGame()
    {
        GameIsActive = false;
        _treeCanvas.gameObject.SetActive(true);

        if (_unpauseCoroutine != null)
        {
            StopCoroutine(_unpauseCoroutine);
            _unpauseCoroutine = null;
        }
    }

    public void UnpauseGame(float delay)
    {
        _unpauseCoroutine = StartCoroutine(UnpauseAfterDelay(delay));
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

    public void AcknowledgeTutorialMovement()
    {
        _movementAcknowledged = true;
    }

    public void AcknowledgeTutorialShooting()
    {
        _shootAcknowledged = true;
    }

    public void AcknowledgeTutorialDashing()
    {
        _dashAcknowledged = true;
    }

    public void AcknowledgeTutorialPause()
    {
        _pauseAcknowledged = true;
    }

    public void AcknowledgeTutorialUnpause()
    {
        _unpauseAcknowledged = true;
    }

    private IEnumerator TutorialCoroutine()
    {
        WaitForSeconds pauseBetweenTutorials = new WaitForSeconds(1f);

        // Movement tutorial
        movementInput.actions.FindAction("Move").Enable();
        tutorialText.text = "To move your ship, use WASD.";
        tutorialText.gameObject.SetActive(true);
        yield return new WaitUntil(() => _movementAcknowledged);
        tutorialText.gameObject.SetActive(false);
        yield return pauseBetweenTutorials;

        // Shooting tutorial
        shootInput.actions.FindAction("Shoot").Enable();
        tutorialText.text = "To shoot your weapon, click the left mouse button.";
        tutorialText.gameObject.SetActive(true);
        yield return new WaitUntil(() => _shootAcknowledged);
        tutorialText.gameObject.SetActive(false);
        yield return pauseBetweenTutorials;

        // Dashing tutorial
        movementInput.actions.FindAction("Dash").Enable();
        tutorialText.text = "To dash a short distance instantly, press Shift or the Spacebar.";
        tutorialText.gameObject.SetActive(true);
        yield return new WaitUntil(() => _dashAcknowledged);
        tutorialText.gameObject.SetActive(false);
        yield return pauseBetweenTutorials;

        // Upgrading tutorial
        upgradeInput.actions.FindAction("Swap").Enable();
        tutorialText.text = "To open and close the ship upgrades tree, press Tab.";
        tutorialText.gameObject.SetActive(true);

        Debug.Log("Waiting for upgrade acknowledgement.");
        yield return new WaitUntil(() => _pauseAcknowledged);
        tutorialText.gameObject.SetActive(false);
        yield return new WaitUntil(() => _unpauseAcknowledged);
        yield return pauseBetweenTutorials;

        PlayerIsInTutorial = false;
    }
}
