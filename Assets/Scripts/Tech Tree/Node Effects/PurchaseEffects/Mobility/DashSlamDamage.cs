using UnityEngine;

public class DashSlamDamage : PurchaseableNode
{
    public override void OnPurchaseEffect()
    {
        statsManager.IncreaseDashSlamDamageMultiplicative(config.magnitude);
    }
}
