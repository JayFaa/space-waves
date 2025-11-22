using UnityEngine;

public class AttackSpread : PurchaseableNode
{
    public override void OnPurchaseEffect()
    {
        statsManager.IncreaseSpreadFireShotCount(Mathf.RoundToInt(config.magnitude));
    }
}
