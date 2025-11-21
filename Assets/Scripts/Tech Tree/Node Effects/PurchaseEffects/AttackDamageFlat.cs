using UnityEngine;

public class AttackDamage : PurchaseableNode
{
    public override void OnPurchaseEffect()
    {
        statsManager.IncreaseAttackDamageFlat(config.magnitude);
    }
}
