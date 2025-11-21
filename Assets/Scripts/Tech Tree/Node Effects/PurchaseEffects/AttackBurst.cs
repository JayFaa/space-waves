using UnityEngine;

public class AttackBurst : PurchaseableNode
{
    public override void OnPurchaseEffect()
    {
        statsManager.IncreaseBurstFireShotCount(Mathf.RoundToInt(config.magnitude));
    }
}
