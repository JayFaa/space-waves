using UnityEngine;
using UnityEngine.UI;

public class ShipUIManager : MonoBehaviour
{
    [SerializeField] Slider healthBar;
    [SerializeField] Slider shieldBar;
    [SerializeField] TMPro.TextMeshProUGUI resourceText;

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        healthBar.value = currentHealth / maxHealth;
    }

    public void UpdateShield(float currentShield, float maxShield)
    {
        shieldBar.value = currentShield / maxShield;
    }

    public void UpdateResourceCount(int count)
    {
        resourceText.text = count.ToString();
    }
}
