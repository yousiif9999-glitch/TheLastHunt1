using UnityEngine;
using TMPro;
using System.Collections;

public class BandageHeal : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip bandageSound;

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

        if (!gameObject.activeSelf)
            return;


        if (Input.GetMouseButtonDown(0))
        {

            if (playerHealth.currentHealth >= playerHealth.maxHealth)
            {
                ShowWarning("Health Already Full");
                return;
            }


            if (bandageUses <= 0)
                return;


            if (bandageAnimator != null)
                bandageAnimator.SetTrigger("Use");

            if (audioSource != null && bandageSound != null)
            {
                audioSource.PlayOneShot(bandageSound);
            }

            playerHealth.Heal(healAmount);


            bandageUses--;

            UpdateUI();


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