using UnityEngine;

public abstract class BaseNodeEffect : MonoBehaviour
{
    abstract public void OnPurchase();

    [SerializeField] public Sprite iconSprite;
    [SerializeField] public string description;

    protected StatsManager statsManager;

    void Awake()
    {
        statsManager = FindFirstObjectByType<StatsManager>();
    }
}
