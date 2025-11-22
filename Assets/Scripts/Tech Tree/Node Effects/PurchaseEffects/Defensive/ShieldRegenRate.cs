using UnityEngine;

public class ShieldRegenRate : PurchaseableNode
{
    public override void OnPurchaseEffect()
    {
        statsManager.IncreaseShieldRegenRate(config.magnitude);
    }
}
