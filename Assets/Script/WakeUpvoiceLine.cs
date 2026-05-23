using System.Collections;
using UnityEngine;
using TMPro;

public class WakeUpVoiceLine : MonoBehaviour
{
    [Header("Voice Line")]
    public AudioSource voiceSource;
    public AudioClip wakeUpVoiceClip;

    [Header("Subtitle")]
    public TextMeshProUGUI subtitleText;
    public string wakeUpLine = "What happened...? Where am I?";

    [Header("Timing")]
    public float delayBeforeVoice = 2.5f;
    public float subtitleDuration = 4f;

    private bool hasPlayed = false;

    void Start()
    {
        if (subtitleText != null)
            subtitleText.text = "";

        StartCoroutine(PlayWakeUpLine());
    }

    IEnumerator PlayWakeUpLine()
    {
        if (hasPlayed)
            yield break;

        hasPlayed = true;

        yield return new WaitForSeconds(delayBeforeVoice);

        if (subtitleText != null)
            subtitleText.text = wakeUpLine;

        if (voiceSource != null && wakeUpVoiceClip != null)
            voiceSource.PlayOneShot(wakeUpVoiceClip);

        yield return new WaitForSeconds(subtitleDuration);

        if (subtitleText != null)
            subtitleText.text = "";
    }
}