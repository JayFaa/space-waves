using UnityEngine;

public class MovementSpeed : PurchaseableNode
{
    public override void OnPurchaseEffect()
    {
        statsManager.IncreaseMovementSpeedMultiplicative(config.magnitude);
    }
}
