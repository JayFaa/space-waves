using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public int GoldCount { get; private set; } = 0;

    void Awake()
    {
        if (FindAnyObjectByType<ResourceManager>() != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void AddGold(int amount)
    {
        GoldCount += amount;
    }

    public bool SpendGold(int amount)
    {
        if (GoldCount >= amount)
        {
            GoldCount -= amount;
            return true;
        }
        return false;
    }
}
