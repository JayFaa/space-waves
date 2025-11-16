using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [SerializeField] CinemachineCamera shipCamera;
    [SerializeField] CinemachineCamera techTreeCamera;

    private GameManager _gameManager;

    void Awake()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
    }

    public void OnSwap(InputValue value)
    {
        if (value.isPressed)
        {
            if (shipCamera.Priority > techTreeCamera.Priority)
            {
                shipCamera.Priority = 0;
                techTreeCamera.Priority = 1;
                _gameManager.PauseGame();
            }
            else
            {
                shipCamera.Priority = 1;
                techTreeCamera.Priority = 0;
                _gameManager.UnpauseGame(1f);
            }
        }
    }
}
