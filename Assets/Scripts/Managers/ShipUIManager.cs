using UnityEngine;
using UnityEngine.UI;

public class ShipUIManager : MonoBehaviour
{
    [SerializeField] Slider healthBar;
    [SerializeField] TMPro.TextMeshProUGUI healthText;
    [SerializeField] Slider shieldBar;
    [SerializeField] TMPro.TextMeshProUGUI shieldText;
    [SerializeField] TMPro.TextMeshProUGUI resourceText;

    void Awake()
    {
        if (FindAnyObjectByType<ShipUIManager>() != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    } 

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        healthBar.value = currentHealth / maxHealth;
        healthText.text = $"{currentHealth} / {maxHealth}";
    }

    public void UpdateShield(float currentShield, float maxShield)
    {
        shieldBar.value = currentShield / maxShield;
        shieldText.text = $"{currentShield} / {maxShield}";
    }

    public void HideText()
    {
        healthText.gameObject.SetActive(false);
        shieldText.gameObject.SetActive(false);
    }

    public void ShowText()
    {
        healthText.gameObject.SetActive(true);
        shieldText.gameObject.SetActive(true);
    }

    public void UpdateResourceCount(int count)
    {
        resourceText.text = count.ToString();
    }
}
