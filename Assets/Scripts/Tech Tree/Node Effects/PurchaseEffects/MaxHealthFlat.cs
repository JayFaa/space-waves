using UnityEngine;

public class MaxHealthFlat : PurchaseableNode
{
    public override void OnPurchaseEffect()
    {
        int amount = Mathf.RoundToInt(config.magnitude);
        
        statsManager.IncreaseMaxHealthFlat(amount);
        playerDestructible.Heal(amount);
    }
}
