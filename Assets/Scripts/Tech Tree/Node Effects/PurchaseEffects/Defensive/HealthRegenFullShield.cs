using UnityEngine;

public class HealthRegenFullShield : PurchaseableNode
{
    public override void OnPurchaseEffect()
    {
        statsManager.IncreaseHealthRegenAtFullShield(config.magnitude);
    }
}
