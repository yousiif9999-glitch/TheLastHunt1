using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class HelicopterCrashCutscene : MonoBehaviour
{
    [Header("Scene References")]
    public Transform helicopterRig;
    public Transform mainRotor;
    public Transform tailRotor;
    public ParticleSystem smokeEffect;
    public ParticleSystem crashBurstEffect;
    public HeliCameraShake cameraShake;
    public CanvasGroup blackoutGroup;

    [Header("Audio")]
    public AudioSource rotorLoop;
    public AudioSource maydayVoice;
    public AudioSource crashSound;

    [Header("Timing")]
    public float maydayTime = 2f;
    public float shakeTime = 4f;
    public float breakTime = 6f;
    public float crashHoldTime = 0.55f;
    public float blackoutFadeTime = 1.2f;
    public string nextSceneName = "WakeUpCrash";

    [Header("Flight")]
    public float forwardSpeed = 8f;
    public float hoverAmount = 0.35f;
    public float hoverSpeed = 1.2f;
    public float rotorSpinSpeed = 1200f;

    [Header("Crash Height")]
    public float crashY = 0f;

    [Header("Fall Motion")]
    public float startFallSpeed = 2.5f;
    public float maxFallSpeed = 13f;
    public float startForwardDuringFall = 1.5f;
    public float maxForwardDuringFall = 5f;
    public float fallBuildUpTime = 3f;

    [Header("Spin And Wobble")]
    public float startSpinSpeed = 120f;
    public float maxSpinSpeed = 360f;
    public float rollAmount = 16f;
    public float wobbleSpeed = 4.5f;
    public float pitchDownAmount = 8f;
    public float rotationSmooth = 3.5f;

    [Header("Camera Shake Amounts")]
    public float normalShake = 0.08f;
    public float dangerShake = 0.25f;
    public float fallingShake = 0.5f;
    public float impactShake = 1f;
    public float impactShakeDuration = 0.6f;

    [Header("Smoke")]
    public float startSmokeRate = 5f;
    public float brokenSmokeRate = 18f;
    public float crashSmokeRate = 40f;

    private float timer;
    private float brokenTimer;

    private bool maydayPlayed;
    private bool broken;
    private bool crashed;
    private bool loadingScene;

    private Vector3 fallDirection;
    private float baseYaw;
    private float spinYaw;
    private Quaternion startRotation;

    public bool IsFalling => broken && !crashed;
    public float FallProgress { get; private set; }

    void Start()
    {
        if (helicopterRig == null) return;

        startRotation = helicopterRig.rotation;

        if (crashBurstEffect != null)
            crashBurstEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (blackoutGroup != null)
            blackoutGroup.alpha = 0f;

        SetSmoke(startSmokeRate);

        if (rotorLoop != null)
            rotorLoop.Play();

        if (cameraShake != null)
            cameraShake.SetShake(normalShake);
    }

    void Update()
    {
        if (helicopterRig == null) return;

        timer += Time.deltaTime;

        if (!broken && !crashed)
        {
            FlyForward();
            SpinRotors();
        }

        if (!maydayPlayed && timer >= maydayTime)
        {
            maydayPlayed = true;

            if (maydayVoice != null)
                maydayVoice.Play();
        }

        if (!broken && timer >= shakeTime && timer < breakTime)
        {
            DisturbFlight();

            if (cameraShake != null)
                cameraShake.SetShake(dangerShake);
        }

        if (!broken && timer >= breakTime)
        {
            BeginFall();
        }

        if (broken && !crashed)
        {
            if (cameraShake != null)
                cameraShake.SetShake(fallingShake);

            FallAndCrash();
        }
    }

    void FlyForward()
    {
        Vector3 flatForward = Vector3.ProjectOnPlane(helicopterRig.forward, Vector3.up).normalized;
        if (flatForward.sqrMagnitude < 0.001f)
            flatForward = Vector3.forward;

        float yOffset = Mathf.Sin(timer * hoverSpeed) * hoverAmount * Time.deltaTime;

        helicopterRig.position += flatForward * forwardSpeed * Time.deltaTime;
        helicopterRig.position += Vector3.up * yOffset;
    }

    void SpinRotors()
    {
        if (!broken && mainRotor != null)
            mainRotor.Rotate(0f, rotorSpinSpeed * Time.deltaTime, 0f, Space.Self);

        if (!broken && tailRotor != null)
            tailRotor.Rotate(rotorSpinSpeed * Time.deltaTime, 0f, 0f, Space.Self);
    }

    void DisturbFlight()
    {
        float pitch = Mathf.Sin(timer * 7f) * 5f;
        float roll = Mathf.Cos(timer * 9f) * 7f;

        Quaternion targetRotation = startRotation * Quaternion.Euler(pitch, 0f, roll);
        helicopterRig.rotation = Quaternion.Slerp(
            helicopterRig.rotation,
            targetRotation,
            Time.deltaTime * 4f
        );
    }

    void BeginFall()
    {
        broken = true;
        brokenTimer = 0f;
        FallProgress = 0f;

        DetachPart(mainRotor, helicopterRig.forward * 2f + Vector3.up * 2f + helicopterRig.right * 2f);
        DetachPart(tailRotor, helicopterRig.forward * 1f + Vector3.up * 1f - helicopterRig.right * 2f);

        SetSmoke(brokenSmokeRate);

        fallDirection = Vector3.ProjectOnPlane(helicopterRig.forward, Vector3.up).normalized;

        if (fallDirection.sqrMagnitude < 0.001f)
            fallDirection = Vector3.forward;

        baseYaw = Quaternion.LookRotation(fallDirection, Vector3.up).eulerAngles.y;
        spinYaw = 0f;
    }

    void DetachPart(Transform part, Vector3 force)
    {
        if (part == null) return;

        part.SetParent(null);

        Rigidbody rb = part.GetComponent<Rigidbody>();
        if (rb == null) rb = part.gameObject.AddComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.AddForce(force, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);

        Collider col = part.GetComponent<Collider>();
        if (col == null)
            part.gameObject.AddComponent<BoxCollider>();
    }

    void FallAndCrash()
    {
        brokenTimer += Time.deltaTime;
        FallProgress = Mathf.Clamp01(brokenTimer / fallBuildUpTime);

        float currentFallSpeed = Mathf.Lerp(startFallSpeed, maxFallSpeed, FallProgress);
        float currentForwardSpeed = Mathf.Lerp(startForwardDuringFall, maxForwardDuringFall, FallProgress);
        float currentSpinSpeed = Mathf.Lerp(startSpinSpeed, maxSpinSpeed, FallProgress);

        helicopterRig.position += Vector3.down * currentFallSpeed * Time.deltaTime;
        helicopterRig.position += fallDirection * currentForwardSpeed * Time.deltaTime;

        spinYaw += currentSpinSpeed * Time.deltaTime;

        float dynamicRoll = Mathf.Sin(Time.time * wobbleSpeed) * Mathf.Lerp(6f, rollAmount, FallProgress);
        float dynamicPitch = Mathf.Lerp(0f, pitchDownAmount, FallProgress) + Mathf.Cos(Time.time * wobbleSpeed * 0.8f) * 2f;

        Quaternion targetRotation = Quaternion.Euler(dynamicPitch, baseYaw + spinYaw, dynamicRoll);

        helicopterRig.rotation = Quaternion.Slerp(
            helicopterRig.rotation,
            targetRotation,
            rotationSmooth * Time.deltaTime
        );

        if (helicopterRig.position.y <= crashY)
        {
            crashed = true;

            Vector3 p = helicopterRig.position;
            p.y = crashY;
            helicopterRig.position = p;

            helicopterRig.rotation = Quaternion.Euler(8f, helicopterRig.eulerAngles.y, 22f);

            if (!loadingScene)
                StartCoroutine(CrashSequence());
        }
    }

    IEnumerator CrashSequence()
    {
        loadingScene = true;

        SetSmoke(crashSmokeRate);

        if (rotorLoop != null)
            StartCoroutine(FadeAudio(rotorLoop, 0.35f));

        if (crashSound != null)
            crashSound.Play();

        if (crashBurstEffect != null)
            crashBurstEffect.Play();

        if (cameraShake != null)
        {
            cameraShake.StopShake();
            cameraShake.Impact(impactShake, impactShakeDuration);
        }

        yield return new WaitForSeconds(crashHoldTime);

        if (blackoutGroup != null)
            yield return StartCoroutine(FadeToBlack());
        else
            yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator FadeToBlack()
    {
        float t = 0f;

        while (t < blackoutFadeTime)
        {
            t += Time.deltaTime;
            blackoutGroup.alpha = Mathf.Clamp01(t / blackoutFadeTime);
            yield return null;
        }

        blackoutGroup.alpha = 1f;
    }

    IEnumerator FadeAudio(AudioSource source, float duration)
    {
        if (source == null) yield break;

        float startVolume = source.volume;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        source.volume = 0f;
        source.Stop();
    }

    void SetSmoke(float rate)
    {
        if (smokeEffect == null) return;

        var emission = smokeEffect.emission;
        emission.rateOverTime = rate;

        if (!smokeEffect.isPlaying)
            smokeEffect.Play();
    }
}