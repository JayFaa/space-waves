using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool GameIsActive { get; private set; } = false;

    private Canvas _treeCanvas;

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
}
