using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public int GoldCount { get; private set; } = 0;

    private UIManager uiManager;

    void Awake()
    {
        if (FindAnyObjectByType<ResourceManager>() != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        uiManager = FindFirstObjectByType<UIManager>();
    }

    void Start()
    {
        uiManager.UpdateResourceCount(GoldCount);
    }

    public void AddGold(int amount)
    {
        GoldCount += amount;
        uiManager.UpdateResourceCount(GoldCount);
    }

    public bool SpendGold(int amount)
    {
        if (GoldCount >= amount)
        {
            GoldCount -= amount;
            uiManager.UpdateResourceCount(GoldCount);
            return true;
        }
        return false;
    }
}
