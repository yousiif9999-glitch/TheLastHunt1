using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestUIManager : MonoBehaviour
{
    public static QuestUIManager Instance;

    // These are needed by PauseMenuManager
    public static bool IsQuestUIOpen = false;
    public static int LastClosedFrame = -1;

    [Serializable]
    public class QuestRow
    {
        public GameObject rowObject;
        public Button rowButton;
        public TextMeshProUGUI questTitleText;
        public TextMeshProUGUI statusText;
        public Image statusBox;
    }

    [Serializable]
    public class QuestData
    {
        public string questTitle;

        [TextArea(3, 6)]
        public string questDescription;

        public bool completed;
    }

    [Header("Quest UI")]
    public GameObject questPanel;
    public TextMeshProUGUI totalValueText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI bottomPageText;

    [Header("Quest Rows")]
    public QuestRow[] questRows = new QuestRow[4];

    [Header("Quest Data")]
    public QuestData[] quests = new QuestData[4];

    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip openSound;
    public AudioClip clickSound;
    public AudioClip completeSound;

    [Header("Objects To Hide While Quest UI Is Open")]
    public GameObject[] objectsToHideWhileQuestUIIsOpen;

    [Header("Scripts To Disable While Quest UI Is Open")]
    public MonoBehaviour[] scriptsToDisableWhileQuestUIIsOpen;

    [Header("Pause Menu Block")]
    public MonoBehaviour pauseMenuManager;
    public GameObject pauseMenuPanel;

    private bool questOpen = false;
    private int currentQuestIndex = 0;

    private bool[] objectPreviousStates;
    private bool[] scriptPreviousStates;
    private bool pauseMenuManagerPreviousState = true;

    void Awake()
    {
        Instance = this;

        IsQuestUIOpen = false;
        LastClosedFrame = -1;

        EnsureDefaultQuestData();
        EnsureQuestRows();
    }

    void Start()
    {
        EnsureDefaultQuestData();
        EnsureQuestRows();

        // Start all quests as not completed
        for (int i = 0; i < quests.Length; i++)
        {
            if (quests[i] != null)
                quests[i].completed = false;
        }

        currentQuestIndex = 0;

        if (questPanel != null)
            questPanel.SetActive(false);

        if (bottomPageText != null)
            bottomPageText.text = "Quest Progress";

        SetupButtons();
        UpdateQuestUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (pauseMenuPanel != null && pauseMenuPanel.activeSelf && !questOpen)
                return;

            ToggleQuestPanel();
        }

        if (questOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseQuestPanel();
        }
    }

    void SetupButtons()
    {
        for (int i = 0; i < questRows.Length; i++)
        {
            int index = i;

            if (questRows[i] != null && questRows[i].rowButton != null)
            {
                questRows[i].rowButton.onClick.RemoveAllListeners();
                questRows[i].rowButton.onClick.AddListener(() => SelectQuest(index));
            }
        }
    }

    public void ToggleQuestPanel()
    {
        if (questOpen)
            CloseQuestPanel();
        else
            OpenQuestPanel();
    }

    public void OpenQuestPanel()
    {
        questOpen = true;
        IsQuestUIOpen = true;

        if (questPanel != null)
            questPanel.SetActive(true);

        ApplyQuestUIState(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (audioSource != null && openSound != null)
            audioSource.PlayOneShot(openSound);

        UpdateQuestUI();
    }

    public void CloseQuestPanel()
    {
        questOpen = false;
        IsQuestUIOpen = false;
        LastClosedFrame = Time.frameCount;

        // Stop quest sound when quest tab closes
        if (audioSource != null)
            audioSource.Stop();

        if (questPanel != null)
            questPanel.SetActive(false);

        ApplyQuestUIState(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void ApplyQuestUIState(bool isOpen)
    {
        if (isOpen)
        {
            if (objectsToHideWhileQuestUIIsOpen != null)
            {
                objectPreviousStates = new bool[objectsToHideWhileQuestUIIsOpen.Length];

                for (int i = 0; i < objectsToHideWhileQuestUIIsOpen.Length; i++)
                {
                    GameObject obj = objectsToHideWhileQuestUIIsOpen[i];

                    if (obj == null)
                        continue;

                    objectPreviousStates[i] = obj.activeSelf;

                    if (obj != questPanel && obj != gameObject)
                        obj.SetActive(false);
                }
            }

            if (scriptsToDisableWhileQuestUIIsOpen != null)
            {
                scriptPreviousStates = new bool[scriptsToDisableWhileQuestUIIsOpen.Length];

                for (int i = 0; i < scriptsToDisableWhileQuestUIIsOpen.Length; i++)
                {
                    MonoBehaviour script = scriptsToDisableWhileQuestUIIsOpen[i];

                    if (script == null)
                        continue;

                    scriptPreviousStates[i] = script.enabled;

                    if (script != this && script != pauseMenuManager)
                        script.enabled = false;
                }
            }

            if (pauseMenuManager != null)
            {
                pauseMenuManagerPreviousState = pauseMenuManager.enabled;
                pauseMenuManager.enabled = false;
            }
        }
        else
        {
            if (objectsToHideWhileQuestUIIsOpen != null && objectPreviousStates != null)
            {
                for (int i = 0; i < objectsToHideWhileQuestUIIsOpen.Length; i++)
                {
                    if (objectsToHideWhileQuestUIIsOpen[i] != null && i < objectPreviousStates.Length)
                    {
                        objectsToHideWhileQuestUIIsOpen[i].SetActive(objectPreviousStates[i]);
                    }
                }
            }

            if (scriptsToDisableWhileQuestUIIsOpen != null && scriptPreviousStates != null)
            {
                for (int i = 0; i < scriptsToDisableWhileQuestUIIsOpen.Length; i++)
                {
                    MonoBehaviour script = scriptsToDisableWhileQuestUIIsOpen[i];

                    if (script != null && i < scriptPreviousStates.Length)
                    {
                        if (script != this && script != pauseMenuManager)
                            script.enabled = scriptPreviousStates[i];
                    }
                }
            }

            if (pauseMenuManager != null)
            {
                pauseMenuManager.enabled = pauseMenuManagerPreviousState;
            }
        }
    }

    public void SelectQuest(int index)
    {
        if (index < 0 || index >= quests.Length)
            return;

        if (quests[index] == null)
            return;

        bool isLocked = !quests[index].completed && index > currentQuestIndex;

        if (isLocked)
        {
            if (descriptionText != null)
                descriptionText.text = "This quest is still locked. Complete the current quest first.";

            return;
        }

        if (descriptionText != null)
            descriptionText.text = quests[index].questDescription;

        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }

    public void CompleteQuest(int questIndex)
    {
        if (questIndex < 0 || questIndex >= quests.Length)
            return;

        if (quests[questIndex] == null)
            return;

        if (quests[questIndex].completed)
            return;

        if (questIndex != currentQuestIndex)
            return;

        quests[questIndex].completed = true;

        if (audioSource != null && completeSound != null)
            audioSource.PlayOneShot(completeSound);

        currentQuestIndex = FindFirstIncompleteQuestIndex();

        UpdateQuestUI();
    }

    int FindFirstIncompleteQuestIndex()
    {
        for (int i = 0; i < quests.Length; i++)
        {
            if (quests[i] != null && !quests[i].completed)
                return i;
        }

        return quests.Length;
    }

    int CountCompletedQuests()
    {
        int count = 0;

        for (int i = 0; i < quests.Length; i++)
        {
            if (quests[i] != null && quests[i].completed)
                count++;
        }

        return count;
    }

    void UpdateQuestUI()
    {
        EnsureDefaultQuestData();

        int completedCount = CountCompletedQuests();

        if (totalValueText != null)
            totalValueText.text = completedCount + " / " + quests.Length;

        for (int i = 0; i < questRows.Length; i++)
        {
            if (questRows[i] == null)
                continue;

            if (questRows[i].rowObject != null)
                questRows[i].rowObject.SetActive(true);

            if (i >= quests.Length || quests[i] == null)
                continue;

            if (quests[i].completed)
            {
                SetQuestRow(i, quests[i].questTitle, "COMPLETE",
                    new Color32(230, 210, 130, 255),
                    new Color32(120, 255, 120, 255),
                    new Color32(45, 70, 35, 255));
            }
            else if (i == currentQuestIndex)
            {
                SetQuestRow(i, quests[i].questTitle, "ACTIVE",
                    new Color32(255, 220, 120, 255),
                    new Color32(255, 220, 120, 255),
                    new Color32(75, 55, 25, 255));
            }
            else
            {
                SetQuestRow(i, "???", "LOCKED",
                    new Color32(150, 135, 100, 255),
                    new Color32(150, 135, 100, 255),
                    new Color32(55, 45, 25, 255));
            }
        }

        if (descriptionText != null)
        {
            if (currentQuestIndex >= 0 && currentQuestIndex < quests.Length)
            {
                descriptionText.text = quests[currentQuestIndex].questDescription;
            }
            else
            {
                descriptionText.text = "All quests are complete. The rescue signal has been sent.";
            }
        }
    }

    void SetQuestRow(int index, string title, string status, Color titleColor, Color statusColor, Color boxColor)
    {
        if (index < 0 || index >= questRows.Length)
            return;

        QuestRow row = questRows[index];

        if (row == null)
            return;

        if (row.questTitleText != null)
        {
            row.questTitleText.text = title;
            row.questTitleText.color = titleColor;
        }

        if (row.statusText != null)
        {
            row.statusText.text = status;
            row.statusText.color = statusColor;
        }

        if (row.statusBox != null)
        {
            row.statusBox.color = boxColor;
        }
    }

    void EnsureQuestRows()
    {
        if (questRows == null || questRows.Length != 4)
        {
            QuestRow[] newRows = new QuestRow[4];

            if (questRows != null)
            {
                for (int i = 0; i < Mathf.Min(questRows.Length, newRows.Length); i++)
                    newRows[i] = questRows[i];
            }

            for (int i = 0; i < newRows.Length; i++)
            {
                if (newRows[i] == null)
                    newRows[i] = new QuestRow();
            }

            questRows = newRows;
        }
    }

    void EnsureDefaultQuestData()
    {
        string[] defaultTitles =
        {
            "Search the ruins for the letter",
            "Repair the broken key",
            "Gather radio repair parts",
            "Fix the radio and call for help"
        };

        string[] defaultDescriptions =
        {
            "A strange letter is hidden somewhere in the old ruins. Search the ruins carefully and read the letter.",
            "Go to the shed and search for the broken key and a hammer. Collect both items, then repair the key so you can open the way forward.",
            "Enter the house and collect the missing repair parts: battery, fuses, and wires. These items are needed before the radio can be fixed.",
            "You now have the parts needed to repair the radio. Go to the radio room, fix the radio system, and send a signal to call for rescue."
        };

        if (quests == null || quests.Length != 4)
        {
            quests = new QuestData[4];
        }

        for (int i = 0; i < quests.Length; i++)
        {
            if (quests[i] == null)
                quests[i] = new QuestData();

            if (string.IsNullOrWhiteSpace(quests[i].questTitle) ||
                quests[i].questTitle.Contains("unknown") ||
                quests[i].questTitle.Contains("???"))
            {
                quests[i].questTitle = defaultTitles[i];
            }

            if (string.IsNullOrWhiteSpace(quests[i].questDescription) ||
                quests[i].questDescription.Contains("unknown") ||
                quests[i].questDescription.Contains("???"))
            {
                quests[i].questDescription = defaultDescriptions[i];
            }
        }
    }

    void OnValidate()
    {
        EnsureQuestRows();
        EnsureDefaultQuestData();
    }
}