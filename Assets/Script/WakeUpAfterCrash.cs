using System.Collections;
using UnityEngine;

public class WakeUpAfterCrash : MonoBehaviour
{
    [Header("UI")]
    public CanvasGroup blackScreen;

    [Header("Wake Up Timing")]
    public float startDelay = 1.5f;
    public float firstOpenTime = 1.2f;
    public float closeAgainTime = 0.6f;
    public float secondOpenTime = 1.5f;
    public float finalOpenTime = 2f;

    [Header("Optional Player Control")]
    public MonoBehaviour[] scriptsToDisableDuringWakeUp;

    void Start()
    {
        StartCoroutine(WakeUpRoutine());
    }

    IEnumerator WakeUpRoutine()
    {
        // Disable player movement/camera look during wake up
        foreach (MonoBehaviour script in scriptsToDisableDuringWakeUp)
        {
            if (script != null)
                script.enabled = false;
        }

        if (blackScreen != null)
            blackScreen.alpha = 1f;

        yield return new WaitForSeconds(startDelay);

        // First small eye opening
        yield return FadeBlackScreen(1f, 0.45f, firstOpenTime);

        // Eyes close again
        yield return FadeBlackScreen(0.45f, 1f, closeAgainTime);

        yield return new WaitForSeconds(0.4f);

        // Second opening
        yield return FadeBlackScreen(1f, 0.25f, secondOpenTime);

        yield return new WaitForSeconds(0.5f);

        // Fully wake up
        yield return FadeBlackScreen(0.25f, 0f, finalOpenTime);

        // Enable player movement/camera look again
        foreach (MonoBehaviour script in scriptsToDisableDuringWakeUp)
        {
            if (script != null)
                script.enabled = true;
        }
    }

    IEnumerator FadeBlackScreen(float startAlpha, float endAlpha, float duration)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t / duration);

            if (blackScreen != null)
                blackScreen.alpha = alpha;

            yield return null;
        }

        if (blackScreen != null)
            blackScreen.alpha = endAlpha;
    }
}