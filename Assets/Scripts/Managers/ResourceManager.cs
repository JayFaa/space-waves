using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public int GoldCount { get; private set; } = 500;

    private ShipUIManager uiManager;

    void Awake()
    {
        if (FindAnyObjectByType<ResourceManager>() != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        uiManager = FindFirstObjectByType<ShipUIManager>();
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

    public bool HasGold(int amount)
    {
        return GoldCount >= amount;
    }

    public void SpendGold(int amount)
    {
        if (GoldCount >= amount)
        {
            GoldCount -= amount;
            uiManager.UpdateResourceCount(GoldCount);
        }
    }
}
