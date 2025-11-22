using UnityEngine;

public abstract class PurchaseableNode : MonoBehaviour
{
    [SerializeField] public NodeEffectSO config;

    protected StatsManager statsManager;
    protected ResourceManager resourceManager;
    protected Destructible playerDestructible;

    public int PurchaseCount { get; private set; }

    abstract public void OnPurchaseEffect();

    protected virtual void Awake()
    {
        statsManager = FindFirstObjectByType<StatsManager>();
        resourceManager = FindFirstObjectByType<ResourceManager>();
        playerDestructible = FindFirstObjectByType<PlayerMovement>().gameObject.GetComponent<Destructible>();
        PurchaseCount = 0;
    }

    public void Purchase()
    {
        if (CanBePurchased()){
            PurchaseCount += 1;
            resourceManager.SpendGold(config.price);
            OnPurchaseEffect();
        }
    }

    public bool CanBePurchased()
    {
        return PurchaseCount < config.maxPurchases && resourceManager.HasGold(config.price);
    }

    public string BuildPurchaseCountText()
    {
        return $"{PurchaseCount}/{config.maxPurchases}";
    }

    public string BuildPriceText()
    {
        return config.price.ToString();
    }
}
