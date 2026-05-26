using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI")]
    public Slider healthBar;
    public TMP_Text healthText;
    public Image fillImage;

    [Header("Colors")]
    public Color highHealthColor = Color.green;
    public Color mediumHealthColor = Color.yellow;
    public Color lowHealthColor = Color.red;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateUI();

        if (currentHealth <= 0)
        {
            Debug.Log("Player Dead");
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateUI();
    }

    public void UpdateUI()
    {
        float healthPercent = (float)currentHealth / maxHealth;

        if (healthBar != null)
            healthBar.value = healthPercent;

        if (healthText != null)
            healthText.text = "HEALTH: " + currentHealth;

        if (fillImage != null)
        {
            if (healthPercent > 0.6f)
                fillImage.color = highHealthColor;
            else if (healthPercent > 0.3f)
                fillImage.color = mediumHealthColor;
            else
                fillImage.color = lowHealthColor;
        }
    }
}