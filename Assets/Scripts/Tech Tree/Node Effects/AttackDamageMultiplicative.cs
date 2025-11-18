using UnityEngine;

public class AttackDamageMultiplicative : BaseNodeEffect
{
    [SerializeField] float damageIncreaseMultiplier = 1.1f;

    public override void OnPurchase()
    {
        statsManager.IncreaseAttackDamageMultiplicative(damageIncreaseMultiplier);
    }
}
