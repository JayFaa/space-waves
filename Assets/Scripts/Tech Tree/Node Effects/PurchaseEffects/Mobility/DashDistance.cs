using UnityEngine;

public class DashDistance : PurchaseableNode
{
    public override void OnPurchaseEffect()
    {
        statsManager.IncreaseDashDistanceBonusMultiplicative(config.magnitude);
    }
}
