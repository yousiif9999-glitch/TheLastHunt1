using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class FinalRescueController : MonoBehaviour
{
    [Header("UI")]
    public Image fadePanel;
    public TextMeshProUGUI storyText;

    [Header("Rescue Light")]
    public Light rescueLight;

    [Header("Audio")]
    public AudioSource radioAudioSource;
    public AudioSource voiceAudioSource;
    public AudioSource helicopterAudioSource;

    public AudioClip radioStaticClip;
    public AudioClip rescueVoiceClip;
    public AudioClip helicopterClip;

    [Header("Scene Settings")]
    public string mainMenuSceneName = "MainMenuScene";

    void Start()
    {
        if (fadePanel != null)
            fadePanel.transform.SetAsFirstSibling();

        if (storyText != null)
        {
            storyText.text = "";
            storyText.transform.SetAsLastSibling();
        }

        if (rescueLight != null)
            rescueLight.intensity = 0f;

        StartCoroutine(PlayRescueEnding());
    }

    IEnumerator PlayRescueEnding()
    {
        // Start black, then keep background very dark
        yield return StartCoroutine(Fade(1f, 0.92f, 2f));

        // Start radio static
        if (radioAudioSource != null && radioStaticClip != null)
        {
            radioAudioSource.clip = radioStaticClip;
            radioAudioSource.loop = true;
            radioAudioSource.Play();
        }

        yield return ShowText("Radio signal connected...", 2f);

        // Play your full voice audio
        if (voiceAudioSource != null && rescueVoiceClip != null)
        {
            voiceAudioSource.clip = rescueVoiceClip;
            voiceAudioSource.loop = false;
            voiceAudioSource.Play();
        }

        float voiceDuration = 6f;

        if (rescueVoiceClip != null)
            voiceDuration = rescueVoiceClip.length;

        yield return ShowText("Rescue transmission received.", voiceDuration);

        // Stop radio static after voice
        if (radioAudioSource != null)
            radioAudioSource.Stop();

        // Start helicopter sound
        if (helicopterAudioSource != null && helicopterClip != null)
        {
            helicopterAudioSource.clip = helicopterClip;
            helicopterAudioSource.loop = true;
            helicopterAudioSource.Play();
        }

        // Turn on rescue light
        yield return StartCoroutine(TurnOnRescueLight());

        // Fade to full black
        yield return StartCoroutine(Fade(0.92f, 1f, 2f));

        // Stop helicopter sound
        if (helicopterAudioSource != null)
            helicopterAudioSource.Stop();

        yield return ShowText("Thanks for playing The Last Hunt.", 3f);
        yield return ShowText("Hope you enjoyed.", 2.5f);

        SceneManager.LoadScene(mainMenuSceneName);
    }

    IEnumerator ShowText(string message, float duration)
    {
        if (storyText != null)
            storyText.text = message;

        yield return new WaitForSeconds(duration);

        if (storyText != null)
            storyText.text = "";
    }

    IEnumerator TurnOnRescueLight()
    {
        float timer = 0f;
        float duration = 3f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            if (rescueLight != null)
                rescueLight.intensity = Mathf.Lerp(0f, 8f, timer / duration);

            yield return null;
        }
    }

    IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        if (fadePanel == null)
            yield break;

        float timer = 0f;

        Color color = fadePanel.color;
        color.a = startAlpha;
        fadePanel.color = color;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float alpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration);
            color.a = alpha;
            fadePanel.color = color;

            yield return null;
        }
    }
}