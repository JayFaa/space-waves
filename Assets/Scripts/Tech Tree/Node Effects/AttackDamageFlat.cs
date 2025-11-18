using UnityEngine;

public class AttackDamage : BaseNodeEffect
{
    [SerializeField] int damageIncreaseAmount = 5;

    public override void OnPurchase()
    {
        statsManager.IncreaseAttackDamageFlat(damageIncreaseAmount);
    }
}
