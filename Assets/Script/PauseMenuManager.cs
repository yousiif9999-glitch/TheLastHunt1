using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Pause Menu UI")]
    public GameObject pauseMenuPanel;

    [Header("Objects To Hide While Paused")]
    public GameObject[] objectsToHideOnPause;
    // Drag HealthBar, Hotbar, Gun, Ammo text here

    [Header("Scripts To Disable While Paused")]
    public Behaviour[] scriptsToDisableOnPause;
    // Drag PlayerMovement, MouseLook, Gun/Shooting script here

    [Header("Pause Music")]
    public AudioSource pauseMusicSource;

    [Header("Scene Names")]
    public string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    private bool[] objectPreviousStates;
    private bool[] scriptPreviousStates;

    void Start()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (pauseMusicSource != null)
        {
            pauseMusicSource.playOnAwake = false;
            pauseMusicSource.loop = true;
            pauseMusicSource.ignoreListenerPause = true;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ContinueGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        SaveCurrentStates();

        HidePauseObjects();
        DisablePauseScripts();

        Time.timeScale = 0f;

        AudioListener.pause = true;

        if (pauseMusicSource != null && !pauseMusicSource.isPlaying)
            pauseMusicSource.Play();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ContinueGame()
    {
        isPaused = false;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        RestorePauseObjects();
        RestorePauseScripts();

        Time.timeScale = 1f;

        AudioListener.pause = false;

        if (pauseMusicSource != null && pauseMusicSource.isPlaying)
            pauseMusicSource.Stop();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (pauseMusicSource != null && pauseMusicSource.isPlaying)
            pauseMusicSource.Stop();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void SaveCurrentStates()
    {
        objectPreviousStates = new bool[objectsToHideOnPause.Length];

        for (int i = 0; i < objectsToHideOnPause.Length; i++)
        {
            if (objectsToHideOnPause[i] != null)
                objectPreviousStates[i] = objectsToHideOnPause[i].activeSelf;
        }

        scriptPreviousStates = new bool[scriptsToDisableOnPause.Length];

        for (int i = 0; i < scriptsToDisableOnPause.Length; i++)
        {
            if (scriptsToDisableOnPause[i] != null)
                scriptPreviousStates[i] = scriptsToDisableOnPause[i].enabled;
        }
    }

    private void HidePauseObjects()
    {
        foreach (GameObject obj in objectsToHideOnPause)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }

    private void RestorePauseObjects()
    {
        if (objectPreviousStates == null) return;

        for (int i = 0; i < objectsToHideOnPause.Length; i++)
        {
            if (objectsToHideOnPause[i] != null)
                objectsToHideOnPause[i].SetActive(objectPreviousStates[i]);
        }
    }

    private void DisablePauseScripts()
    {
        foreach (Behaviour script in scriptsToDisableOnPause)
        {
            if (script != null)
                script.enabled = false;
        }
    }

    private void RestorePauseScripts()
    {
        if (scriptPreviousStates == null) return;

        for (int i = 0; i < scriptsToDisableOnPause.Length; i++)
        {
            if (scriptsToDisableOnPause[i] != null)
                scriptsToDisableOnPause[i].enabled = scriptPreviousStates[i];
        }
    }
}