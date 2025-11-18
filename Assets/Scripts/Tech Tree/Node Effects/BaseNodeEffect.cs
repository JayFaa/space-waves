using UnityEngine;

public abstract class BaseNodeEffect : MonoBehaviour
{
    abstract public void OnPurchase();

    protected StatsManager statsManager;

    void Awake()
    {
        statsManager = FindFirstObjectByType<StatsManager>();
    }
}
