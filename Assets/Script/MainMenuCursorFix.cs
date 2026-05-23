using UnityEngine;

public class MainMenuCursorFix : MonoBehaviour
{
    void Awake()
    {
        ResetCursor();
    }

    void Start()
    {
        ResetCursor();
    }

    void Update()
    {
        // Keeps mouse unlocked while you are in the main menu
        ResetCursor();
    }

    void ResetCursor()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}