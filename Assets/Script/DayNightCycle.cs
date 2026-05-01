using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Light sunLight;

    [Header("Cycle Settings")]
    public float fullDayLength = 300f;
    [Range(0f, 1f)] public float startTime = 0.25f;

    [Header("Light Intensity")]
    public float dayIntensity = 50000f;
    public float nightIntensity = 0f;

    private float timeOfDay;

    void Start()
    {
        timeOfDay = startTime;
    }

    void Update()
    {
        if (sunLight == null) return;

        timeOfDay += Time.deltaTime / fullDayLength;
        if (timeOfDay >= 1f)
            timeOfDay = 0f;

        float xAngle = Mathf.Lerp(-90f, 270f, timeOfDay);
        transform.localRotation = Quaternion.Euler(xAngle, 0f, 0f);

        float sunHeight = Vector3.Dot(sunLight.transform.forward, Vector3.down);
        float t = Mathf.Clamp01(sunHeight);

        sunLight.intensity = Mathf.Lerp(nightIntensity, dayIntensity, t);
    }
}