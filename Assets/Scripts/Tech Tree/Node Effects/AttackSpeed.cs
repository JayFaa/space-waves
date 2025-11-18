using UnityEngine;

public class AttackSpeed : BaseNodeEffect
{
    [SerializeField] float attackSpeedIncreaseAmount = 1.1f;

    public override void OnPurchase()
    {
        statsManager.IncreaseAttackSpeed(attackSpeedIncreaseAmount);
    }
}
