using UnityEngine;

public class AttackDamageMultiplicative : PurchaseableNode
{
    public override void OnPurchaseEffect()
    {
        statsManager.IncreaseAttackDamageMultiplicative(config.magnitude);
    }
}
