using UnityEngine;

public class MaxShieldFlat : PurchaseableNode
{
    public override void OnPurchaseEffect()
    {
        statsManager.IncreaseMaxShieldFlat(Mathf.RoundToInt(config.magnitude));
        playerDestructible.UpdateUI();
    }
}
