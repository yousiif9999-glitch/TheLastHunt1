using UnityEngine;

public class HeliCameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    public float positionAmount = 0.08f;
    public float rotationAmount = 1.2f;
    public float speed = 18f;
    public float smooth = 8f;

    [Range(0f, 1f)] public float currentShake = 0f;

    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;

    private float targetShake = 0f;

    private float impactAmount = 0f;
    private float impactTimer = 0f;
    private float impactDuration = 0.01f;

    void Start()
    {
        originalLocalPosition = transform.localPosition;
        originalLocalRotation = transform.localRotation;
    }

    void LateUpdate()
    {
        currentShake = Mathf.Lerp(currentShake, targetShake, Time.deltaTime * smooth);

        float impactShake = 0f;
        if (impactTimer > 0f)
        {
            impactTimer -= Time.deltaTime;
            impactShake = impactAmount * Mathf.Clamp01(impactTimer / impactDuration);
        }

        float finalShake = Mathf.Clamp01(currentShake + impactShake);

        float time = Time.time * speed;

        float x = (Mathf.PerlinNoise(time, 0f) - 0.5f) * 2f;
        float y = (Mathf.PerlinNoise(0f, time) - 0.5f) * 2f;
        float z = (Mathf.PerlinNoise(time, time) - 0.5f) * 2f;

        Vector3 shakePos = new Vector3(x, y, z) * positionAmount * finalShake;
        transform.localPosition = originalLocalPosition + shakePos;

        Vector3 shakeRot = new Vector3(y, x, z) * rotationAmount * finalShake;
        transform.localRotation = originalLocalRotation * Quaternion.Euler(shakeRot);
    }

    public void SetShake(float amount)
    {
        targetShake = Mathf.Clamp01(amount);
    }

    public void StopShake()
    {
        targetShake = 0f;
    }

    public void Impact(float amount, float duration)
    {
        impactAmount = Mathf.Clamp01(amount);
        impactDuration = Mathf.Max(0.01f, duration);
        impactTimer = impactDuration;
    }
}