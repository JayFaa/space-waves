using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool GameIsActive { get; private set; } = false;

    void Awake()
    {
        if (FindAnyObjectByType<GameManager>() != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        GameIsActive = true;
    }

    public void PauseGame()
    {
        GameIsActive = false;
    }

    public void UnpauseGame(float delay)
    {
        StartCoroutine(UnpauseAfterDelay(delay));
    }

    IEnumerator UnpauseAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        GameIsActive = true;
    }
}
