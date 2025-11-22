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
    public float DamageReduction { get; private set; } = 1f;

    void Awake()
    {
        if (FindAnyObjectByType<StatsManager>() != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

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

    public void IncreaseMaxHealthFlat(int amount)
    {
        MaxHealthBonusFlat += amount;
        Debug.Log($"Increased health by {amount}. New health bonus flat: {MaxHealthBonusFlat}");
    }
}