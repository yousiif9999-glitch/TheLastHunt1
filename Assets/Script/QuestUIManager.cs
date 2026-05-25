using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestUIManager : MonoBehaviour
{
    public static bool IsQuestUIOpen { get; private set; }
    public static int LastClosedFrame { get; private set; } = -1;

    [System.Serializable]
    public class QuestData
    {
        [TextArea(3, 6)]
        public string questDescription;

        public bool completed;
    }

    [System.Serializable]
    public class QuestRowUI
    {
        public GameObject rowObject;
        public Button rowButton;
        public TMP_Text questTitleText;
        public TMP_Text statusText;
        public Image statusBox;
    }

    [Header("Main UI")]
    public GameObject questCanvas;
    public TMP_Text totalClearedText;
    public TMP_Text descriptionText;
    public TMP_Text bottomPageText;

    [Header("Gameplay Hint")]
    public GameObject questHintText;

    [Header("Quest Sound")]
    public AudioSource questAudioSource;
    public AudioClip questOpenSound;

    [Header("Quest Rows")]
    public QuestRowUI[] questRows;

    [Header("Quest Data")]
    public QuestData[] quests;

    [Header("Objects To Hide While Quest UI Is Open")]
    public GameObject[] objectsToHide;

    [Header("Scripts To Disable While Quest UI Is Open")]
    public Behaviour[] scriptsToDisable;

    [Header("Pause Menu Block")]
    public Behaviour pauseMenuManager;
    public GameObject pauseMenuPanel;

    private int selectedQuestIndex = 0;

    private bool[] previousObjectStates;
    private bool[] previousScriptStates;
    private bool previousPauseState;

    private float previousTimeScale;
    private CursorLockMode previousCursorLockMode;
    private bool previousCursorVisible;

    private Color normalTextColor = new Color32(242, 230, 199, 255);
    private Color selectedTextColor = new Color32(255, 215, 100, 255);
    private Color completeColor = new Color32(116, 204, 255, 255);
    private Color unknownColor = new Color32(242, 214, 107, 255);

    void Start()
    {
        if (quests == null || quests.Length == 0)
        {
            quests = new QuestData[3];

            for (int i = 0; i < quests.Length; i++)
            {
                quests[i] = new QuestData
                {
                    questDescription = "This quest is still unknown. The real task will be added later.",
                    completed = false
                };
            }
        }

        if (questCanvas != null)
            questCanvas.SetActive(false);

        if (questHintText != null)
            questHintText.SetActive(true);

        if (questAudioSource != null)
        {
            questAudioSource.playOnAwake = false;
            questAudioSource.loop = false;
            questAudioSource.ignoreListenerPause = true;
        }

        IsQuestUIOpen = false;

        SetupQuestButtons();
        RefreshQuestUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleQuestUI();
        }

        if (IsQuestUIOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseQuestUI();
        }
    }

    private void SetupQuestButtons()
    {
        for (int i = 0; i < questRows.Length; i++)
        {
            int index = i;

            if (questRows[i].rowButton != null)
            {
                questRows[i].rowButton.onClick.RemoveAllListeners();
                questRows[i].rowButton.onClick.AddListener(() => SelectQuest(index));
            }
        }
    }

    public void ToggleQuestUI()
    {
        if (IsQuestUIOpen)
            CloseQuestUI();
        else
            OpenQuestUI();
    }

    public void OpenQuestUI()
    {
        IsQuestUIOpen = true;

        previousTimeScale = Time.timeScale;
        previousCursorLockMode = Cursor.lockState;
        previousCursorVisible = Cursor.visible;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (questCanvas != null)
            questCanvas.SetActive(true);

        if (questHintText != null)
            questHintText.SetActive(false);

        PlayOpenSound();

        HideGameplayObjects();
        DisableGameplayScripts();
        DisablePauseMenu();

        RefreshQuestUI();
    }

    public void CloseQuestUI()
    {
        IsQuestUIOpen = false;
        LastClosedFrame = Time.frameCount;

        Time.timeScale = previousTimeScale;
        Cursor.lockState = previousCursorLockMode;
        Cursor.visible = previousCursorVisible;

        if (questCanvas != null)
            questCanvas.SetActive(false);

        if (questHintText != null)
            questHintText.SetActive(true);

        RestoreGameplayObjects();
        RestoreGameplayScripts();
        RestorePauseMenu();
    }

    private void PlayOpenSound()
    {
        if (questAudioSource != null && questOpenSound != null)
            questAudioSource.PlayOneShot(questOpenSound);
    }

    public void SelectQuest(int questIndex)
    {
        if (questIndex < 0 || questIndex >= quests.Length)
            return;

        selectedQuestIndex = questIndex;
        RefreshQuestUI();
    }

    public void CompleteQuestByIndex(int questIndex)
    {
        if (questIndex < 0 || questIndex >= quests.Length)
            return;

        quests[questIndex].completed = true;
        RefreshQuestUI();
    }

    private void RefreshQuestUI()
    {
        int completedCount = 0;

        for (int i = 0; i < quests.Length; i++)
        {
            if (quests[i].completed)
                completedCount++;
        }

        if (totalClearedText != null)
            totalClearedText.text = completedCount + " / " + quests.Length;

        if (bottomPageText != null)
            bottomPageText.text = "1 / 1";

        for (int i = 0; i < questRows.Length; i++)
        {
            if (i >= quests.Length)
            {
                if (questRows[i].rowObject != null)
                    questRows[i].rowObject.SetActive(false);

                continue;
            }

            QuestData quest = quests[i];
            QuestRowUI row = questRows[i];

            if (row.rowObject != null)
                row.rowObject.SetActive(true);

            if (row.questTitleText != null)
            {
                row.questTitleText.text = "???";

                if (i == selectedQuestIndex)
                    row.questTitleText.color = selectedTextColor;
                else
                    row.questTitleText.color = normalTextColor;
            }

            if (row.statusText != null)
            {
                if (quest.completed)
                {
                    row.statusText.text = "Complete!";
                    row.statusText.color = completeColor;
                }
                else
                {
                    row.statusText.text = "???";
                    row.statusText.color = unknownColor;
                }
            }

            if (row.statusBox != null)
                row.statusBox.gameObject.SetActive(true);
        }

        if (descriptionText != null)
        {
            if (selectedQuestIndex >= 0 && selectedQuestIndex < quests.Length)
                descriptionText.text = quests[selectedQuestIndex].questDescription;
        }
    }

    private void HideGameplayObjects()
    {
        if (objectsToHide == null)
            objectsToHide = new GameObject[0];

        previousObjectStates = new bool[objectsToHide.Length];

        for (int i = 0; i < objectsToHide.Length; i++)
        {
            if (objectsToHide[i] != null)
            {
                previousObjectStates[i] = objectsToHide[i].activeSelf;
                objectsToHide[i].SetActive(false);
            }
        }
    }

    private void RestoreGameplayObjects()
    {
        if (previousObjectStates == null || objectsToHide == null)
            return;

        for (int i = 0; i < objectsToHide.Length; i++)
        {
            if (objectsToHide[i] != null)
                objectsToHide[i].SetActive(previousObjectStates[i]);
        }
    }

    private void DisableGameplayScripts()
    {
        if (scriptsToDisable == null)
            scriptsToDisable = new Behaviour[0];

        previousScriptStates = new bool[scriptsToDisable.Length];

        for (int i = 0; i < scriptsToDisable.Length; i++)
        {
            if (scriptsToDisable[i] != null && scriptsToDisable[i] != this)
            {
                previousScriptStates[i] = scriptsToDisable[i].enabled;
                scriptsToDisable[i].enabled = false;
            }
        }
    }

    private void RestoreGameplayScripts()
    {
        if (previousScriptStates == null || scriptsToDisable == null)
            return;

        for (int i = 0; i < scriptsToDisable.Length; i++)
        {
            if (scriptsToDisable[i] != null && scriptsToDisable[i] != this)
                scriptsToDisable[i].enabled = previousScriptStates[i];
        }
    }

    private void DisablePauseMenu()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        if (pauseMenuManager != null)
        {
            previousPauseState = pauseMenuManager.enabled;
            pauseMenuManager.enabled = false;
        }
    }

    private void RestorePauseMenu()
    {
        if (pauseMenuManager != null)
            pauseMenuManager.enabled = previousPauseState;
    }
}