using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FlashlightBattery : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip flashlightClickSound;

    public Light flashlightLight;

    public float battery = 100f;
    public float maxBattery = 100f;

    public float drainSpeed = 5f;
    public float rechargeSpeed = 2f;

    public Slider batteryBar;
    public TMP_Text batteryText;

    private bool isOn = false;

    void Start()
    {
        flashlightLight.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isOn = !isOn;
            flashlightLight.enabled = isOn;

            if (audioSource != null && flashlightClickSound != null)
            {
                audioSource.PlayOneShot(flashlightClickSound);
            }
        }

        if (isOn)
        {
            battery -= drainSpeed * Time.deltaTime;

            if (battery <= 0)
            {
                battery = 0;
                isOn = false;
                flashlightLight.enabled = false;
            }
        }
        else
        {
            battery += rechargeSpeed * Time.deltaTime;

            if (battery > maxBattery)
                battery = maxBattery;
        }

        float batteryPercent = battery / maxBattery;


        if (batteryBar != null)
        {
            batteryBar.value = batteryPercent;


            Image fill = batteryBar.fillRect.GetComponent<Image>();

            if (batteryPercent > 0.6f)
            {
                fill.color = Color.green;
            }
            else if (batteryPercent > 0.3f)
            {
                fill.color = Color.yellow;
            }
            else
            {
                fill.color = Color.red;
            }
        }


        if (batteryText != null)
            batteryText.text = Mathf.RoundToInt(battery) + "%";
    }
}