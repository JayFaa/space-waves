using UnityEngine;

public class DamageReduction : PurchaseableNode
{
    public override void OnPurchaseEffect()
    {
        statsManager.IncreaseDamageReduction(config.magnitude);
    }
}
