using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [SerializeField] CinemachineCamera shipCamera;
    [SerializeField] CinemachineCamera techTreeCamera;

    private GameManager _gameManager;
    private ShipUIManager _shipUIManager;

    void Awake()
    {
        if (FindAnyObjectByType<CameraManager>() != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        _gameManager = FindFirstObjectByType<GameManager>();
        _shipUIManager = FindFirstObjectByType<ShipUIManager>();
    }

    public void OnSwap(InputValue value)
    {
        // Don't allow camera swapping while resetting
        if (_gameManager.GameIsResetting) return;

        if (value.isPressed)
        {
            if (shipCamera.Priority > techTreeCamera.Priority)
            {
                shipCamera.Priority = 0;
                techTreeCamera.Priority = 1;
                _gameManager.PauseGame();
                _shipUIManager.HideText();
                _gameManager.AcknowledgeTutorialPause();
            }
            else
            {
                shipCamera.Priority = 1;
                techTreeCamera.Priority = 0;
                _gameManager.UnpauseGame(1f);
                _shipUIManager.ShowText();
                _gameManager.AcknowledgeTutorialUnpause();
            }
        }
    }
}
