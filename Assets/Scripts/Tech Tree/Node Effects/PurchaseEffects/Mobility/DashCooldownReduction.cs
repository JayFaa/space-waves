using UnityEngine;

public class DashCooldownReduction : PurchaseableNode
{
    public override void OnPurchaseEffect()
    {
        statsManager.IncreaseDashCooldownReductionMultiplicative(config.magnitude);
    }
}
