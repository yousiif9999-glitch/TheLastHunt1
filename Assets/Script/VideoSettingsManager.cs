using UnityEngine;

public class VideoSettingsManager : MonoBehaviour
{
    void Start()
    {
        LoadSettings();
    }

    public void FullscreenOn()
    {
        Screen.fullScreen = true;
        PlayerPrefs.SetInt("Fullscreen", 1);
        PlayerPrefs.Save();
    }

    public void FullscreenOff()
    {
        Screen.fullScreen = false;
        PlayerPrefs.SetInt("Fullscreen", 0);
        PlayerPrefs.Save();
    }

    public void QualityLow()
    {
        QualitySettings.SetQualityLevel(0);
        PlayerPrefs.SetInt("Quality", 0);
        PlayerPrefs.Save();
    }

    public void QualityMedium()
    {
        QualitySettings.SetQualityLevel(1);
        PlayerPrefs.SetInt("Quality", 1);
        PlayerPrefs.Save();
    }

    public void QualityHigh()
    {
        QualitySettings.SetQualityLevel(2);
        PlayerPrefs.SetInt("Quality", 2);
        PlayerPrefs.Save();
    }

    public void VSyncOn()
    {
        QualitySettings.vSyncCount = 1;
        PlayerPrefs.SetInt("VSync", 1);
        PlayerPrefs.Save();
    }

    public void VSyncOff()
    {
        QualitySettings.vSyncCount = 0;
        PlayerPrefs.SetInt("VSync", 0);
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        Screen.fullScreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        int quality = PlayerPrefs.GetInt("Quality", 2);
        QualitySettings.SetQualityLevel(quality);

        QualitySettings.vSyncCount = PlayerPrefs.GetInt("VSync", 1) == 1 ? 1 : 0;
    }
}