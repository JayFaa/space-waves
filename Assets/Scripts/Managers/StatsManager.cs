using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public float AttackDamageFlatModifier { get; private set; } = 0f;
    public float AttackDamageMultiplicativeModifier { get; private set; } = 1f;
    public float AttackSpeedMultiplier { get; private set; } = 1f;

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
}
