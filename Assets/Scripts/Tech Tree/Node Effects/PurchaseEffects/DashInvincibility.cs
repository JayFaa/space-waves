using UnityEngine;

public class DashInvincibility : PurchaseableNode
{
    public override void OnPurchaseEffect()
    {
        statsManager.IncreaseDashInvincibilityDuration(config.magnitude);
    }
}
