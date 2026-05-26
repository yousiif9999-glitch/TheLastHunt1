using UnityEngine;
using TMPro;
using System.Collections;

public class BandageHeal : MonoBehaviour
{
    public TMP_Text warningText;

    public Animator bandageAnimator;

    public PlayerHealth playerHealth;

    public int healAmount = 25;

    public int bandageUses = 3;

    public TMP_Text usesText;

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        // لازم الباندج يكون ظاهر
        if (!gameObject.activeSelf)
            return;

        // عند الضغط
        if (Input.GetMouseButtonDown(0))
        {
            // اذا الهيلث فل
            if (playerHealth.currentHealth >= playerHealth.maxHealth)
            {
                ShowWarning("Health Already Full");
                return;
            }

            // اذا مافي استخدامات
            if (bandageUses <= 0)
                return;

            // تشغيل الانميشن
            if (bandageAnimator != null)
                bandageAnimator.SetTrigger("Use");

            playerHealth.Heal(healAmount);

            // تقليل الاستخدامات
            bandageUses--;

            UpdateUI();

            // اذا خلص الباندج
            if (bandageUses <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }

    void UpdateUI()
    {
        if (usesText != null)
        {
            usesText.text = "x" + bandageUses;
        }
    }

    void ShowWarning(string message)
    {
        if (warningText != null)
        {
            warningText.text = message;

            CancelInvoke(nameof(HideWarning));
            Invoke(nameof(HideWarning), 2f);
        }
    }

    void HideWarning()
    {
        if (warningText != null)
        {
            warningText.text = "";
        }
    }

}