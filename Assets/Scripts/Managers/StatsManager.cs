using UnityEngine;

public class StatsManager : MonoBehaviour
{
    // Attack-related stats
    public float AttackDamageFlatModifier { get; private set; } = 0f;
    public float AttackDamageMultiplicativeModifier { get; private set; } = 1f;
    public float AttackSpeedMultiplier { get; private set; } = 1f;
    public int BurstFireShotCount { get; private set; } = 1;
    public int SpreadFireShotCount { get; private set; } = 1;

    // Defense-related stats
    public int MaxHealthBonusFlat { get; private set; } = 0;
    public int ShieldBonusFlat { get; private set; } = 0;
    public float DamageReductionMultiplicative { get; private set; } = 1f;
    public float ShieldRegenRateMultiplicative { get; private set; } = 1f;
    public float HealthRegenAtFullShield { get; private set; } = 0f;

    // Mobility-related stats
    public float DashInvincibilityDuration { get; private set; } = 0f;
    public float DashSlamDamageMultiplicative { get; private set; } = 1f;
    public float DashCooldownReductionMultiplicative { get; private set; } = 1f;
    public float DashDistanceBonusMultiplicative { get; private set; } = 1f;

    void Awake()
    {
        if (FindAnyObjectByType<StatsManager>() != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    // *********************
    // Attack Node Effects
    // *********************

    public void IncreaseAttackDamageFlat(float amount)
    {
        AttackDamageFlatModifier += amount;
        Debug.Log($"Increased attack damage by {amount}. New flat modifier: {AttackDamageFlatModifier}");
    }

    public void IncreaseAttackDamageMultiplicative(float multiplier)
    {
        AttackDamageMultiplicativeModifier *= multiplier;
        Debug.Log($"Increased attack damage multiplicatively by {multiplier}. New multiplicative modifier: {AttackDamageMultiplicativeModifier}");
    }

    public void IncreaseAttackSpeed(float multiplier)
    {
        AttackSpeedMultiplier *= multiplier;
        Debug.Log($"Increased attack speed by {multiplier}. New attack speed multiplier: {AttackSpeedMultiplier}");
    }

    public void IncreaseBurstFireShotCount(int additionalShots)
    {
        BurstFireShotCount += additionalShots;
        Debug.Log($"Increased burst fire shot count by {additionalShots}. New burst fire shot count: {BurstFireShotCount}");
    }

    public void IncreaseSpreadFireShotCount(int additionalShots)
    {
        SpreadFireShotCount += additionalShots;
        Debug.Log($"Increased spread fire shot count by {additionalShots}. New spread fire shot count: {SpreadFireShotCount}");
    }

    // *********************
    // Defense Node Effects
    // *********************

    public void IncreaseMaxHealthFlat(int amount)
    {
        MaxHealthBonusFlat += amount;
        Debug.Log($"Increased health by {amount}. New health bonus flat: {MaxHealthBonusFlat}");
    }

    public void IncreaseMaxShieldFlat(int amount)
    {
        ShieldBonusFlat += amount;
        Debug.Log($"Increased shield by {amount}. New shield bonus flat: {ShieldBonusFlat}");
    }

    public void IncreaseDamageReduction(float multiplier)
    {
        DamageReductionMultiplicative -= (multiplier - 1) * DamageReductionMultiplicative;
        Debug.Log($"Increased damage reduction by {multiplier}. New damage reduction: {DamageReductionMultiplicative}");
    }

    public void IncreaseShieldRegenRate(float multiplier)
    {
        ShieldRegenRateMultiplicative *= multiplier;
        Debug.Log($"Increased shield regen rate by {multiplier}. New shield regen rate multiplier: {ShieldRegenRateMultiplicative}");
    }

    public void IncreaseHealthRegenAtFullShield(float amount)
    {
        HealthRegenAtFullShield += amount;
        Debug.Log($"Increased health regen at full shield by {amount}. New health regen at full shield: {HealthRegenAtFullShield}");
    }

    // *********************
    // Mobility Node Effects
    // *********************

    public void IncreaseDashInvincibilityDuration(float amount)
    {
        DashInvincibilityDuration += amount;
        Debug.Log($"Increased dash invincibility duration by {amount}. New dash invincibility duration: {DashInvincibilityDuration}");
    }

    public void IncreaseDashSlamDamageMultiplicative(float multiplier)
    {
        DashSlamDamageMultiplicative *= multiplier;
        Debug.Log($"Increased dash slam damage multiplicatively by {multiplier}. New dash slam damage multiplicative: {DashSlamDamageMultiplicative}");
    }

    public void IncreaseDashCooldownReductionMultiplicative(float multiplier)
    {
        DashCooldownReductionMultiplicative -= (multiplier - 1) * DashCooldownReductionMultiplicative;
        Debug.Log($"Increased dash cooldown reduction multiplicatively by {multiplier}. New dash cooldown reduction multiplicative: {DashCooldownReductionMultiplicative}");
    }

    public void IncreaseDashDistanceBonusMultiplicative(float multiplier)
    {
        DashDistanceBonusMultiplicative *= multiplier;
        Debug.Log($"Increased dash distance bonus multiplicatively by {multiplier}. New dash distance bonus multiplicative: {DashDistanceBonusMultiplicative}");
    }
}