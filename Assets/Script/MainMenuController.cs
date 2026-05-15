using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string gameSceneName = "HeliCrashCutscene";

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}