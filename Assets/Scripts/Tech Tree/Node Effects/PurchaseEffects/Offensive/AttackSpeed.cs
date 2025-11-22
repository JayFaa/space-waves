using UnityEngine;

public class AttackSpeed : PurchaseableNode
{
    public override void OnPurchaseEffect()
    {
        statsManager.IncreaseAttackSpeed(config.magnitude);
    }
}
