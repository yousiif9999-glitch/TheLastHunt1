using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class RadioSwitchPuzzle : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionDistance = 3f;
    public string radioTag = "Radio";

    [Header("UI")]
    public Canvas mainCanvas;
    public GameObject radioPromptText;
    public GameObject radioPuzzlePanel;
    public TMP_Text puzzleMessageText;

    [Header("Prompt Position")]
    public float promptScreenYOffset = 80f;

    [Header("Switch Buttons")]
    public Button[] switchButtons;

    [Header("Switch Images")]
    public Image[] switchImages;
    public Image[] lightImages;

    [Header("Sprites")]
    public Sprite switchOffSprite;
    public Sprite switchOnSprite;
    public Sprite lightOffSprite;
    public Sprite lightOnSprite;

    [Header("Audio")]
    public AudioSource radioCrackleAudio;

    [Header("Hide UI While Puzzle Is Open")]
    public GameObject[] objectsToHideWhilePuzzleOpen;

    [Header("Player Movement Scripts To Disable While Puzzle Is Open")]
    public MonoBehaviour[] movementScriptsToDisable;

    [Header("Ending Scene")]
    public string rescueEndingSceneName = "RescueEndingScene";

    private Camera playerCamera;
    private RectTransform promptRect;
    private RectTransform canvasRect;

    private bool lookingAtRadio = false;
    private bool puzzleOpen = false;
    private bool puzzleSolved = false;

    private bool[] switchStates;

    void Start()
    {
        playerCamera = Camera.main;

        if (mainCanvas == null)
            mainCanvas = FindObjectOfType<Canvas>();

        if (mainCanvas != null)
            canvasRect = mainCanvas.GetComponent<RectTransform>();

        if (radioPromptText != null)
        {
            promptRect = radioPromptText.GetComponent<RectTransform>();
            radioPromptText.SetActive(false);
        }

        if (radioPuzzlePanel != null)
            radioPuzzlePanel.SetActive(false);

        if (puzzleMessageText != null)
            puzzleMessageText.text = "";

        switchStates = new bool[switchImages.Length];

        SetupButtonClicks();
        ResetSwitches();
    }

    void Update()
    {
        if (!puzzleOpen)
        {
            CheckRadioHover();

            if (lookingAtRadio && Input.GetKeyDown(KeyCode.E) && !puzzleSolved)
            {
                OpenPuzzle();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !puzzleSolved)
            {
                ClosePuzzle();
            }
        }
    }

    void CheckRadioHover()
    {
        lookingAtRadio = false;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.CompareTag(radioTag))
            {
                lookingAtRadio = true;
                MovePromptUnderRadio(hit.collider);
            }
        }

        if (radioPromptText != null)
            radioPromptText.SetActive(lookingAtRadio && !puzzleSolved);
    }

    void MovePromptUnderRadio(Collider radioCollider)
    {
        if (promptRect == null || canvasRect == null)
            return;

        Vector3 radioWorldPosition = radioCollider.bounds.center;
        Vector3 screenPosition = playerCamera.WorldToScreenPoint(radioWorldPosition);

        screenPosition.y -= promptScreenYOffset;

        Vector2 localPosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPosition,
            null,
            out localPosition
        );

        promptRect.anchoredPosition = localPosition;
    }

    void OpenPuzzle()
    {
        puzzleOpen = true;

        if (radioPromptText != null)
            radioPromptText.SetActive(false);

        if (radioPuzzlePanel != null)
        {
            radioPuzzlePanel.SetActive(true);
            radioPuzzlePanel.transform.SetAsLastSibling();
        }

        if (puzzleMessageText != null)
            puzzleMessageText.text = "Turn all switches ON to call for help.";

        if (radioCrackleAudio != null && !radioCrackleAudio.isPlaying)
            radioCrackleAudio.Play();

        SetOtherUI(false);
        SetPlayerMovement(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ClosePuzzle()
    {
        puzzleOpen = false;

        if (radioPuzzlePanel != null)
            radioPuzzlePanel.SetActive(false);

        if (radioCrackleAudio != null)
            radioCrackleAudio.Stop();

        SetOtherUI(true);
        SetPlayerMovement(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void SetupButtonClicks()
    {
        for (int i = 0; i < switchButtons.Length; i++)
        {
            int index = i;

            if (switchButtons[i] != null)
            {
                switchButtons[i].onClick.RemoveAllListeners();
                switchButtons[i].onClick.AddListener(() => ToggleSwitch(index));
            }
        }
    }

    public void ToggleSwitch(int index)
    {
        if (puzzleSolved)
            return;

        if (index < 0 || index >= switchStates.Length)
            return;

        switchStates[index] = !switchStates[index];

        UpdateSwitchVisual(index);
        CheckIfSolved();
    }

    void UpdateSwitchVisual(int index)
    {
        if (switchImages[index] != null)
            switchImages[index].sprite = switchStates[index] ? switchOnSprite : switchOffSprite;

        if (lightImages[index] != null)
            lightImages[index].sprite = switchStates[index] ? lightOnSprite : lightOffSprite;
    }

    void CheckIfSolved()
    {
        for (int i = 0; i < switchStates.Length; i++)
        {
            if (!switchStates[i])
                return;
        }

        SolvePuzzle();
    }

    void SolvePuzzle()
    {
        puzzleSolved = true;

        if (puzzleMessageText != null)
            puzzleMessageText.text = "Signal connected... Help request sent.";

        if (radioCrackleAudio != null)
            radioCrackleAudio.Stop();

        Invoke(nameof(LoadRescueEndingScene), 2.5f);
    }

    void LoadRescueEndingScene()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SceneManager.LoadScene(rescueEndingSceneName);
    }

    void ResetSwitches()
    {
        for (int i = 0; i < switchStates.Length; i++)
        {
            switchStates[i] = false;
            UpdateSwitchVisual(i);
        }
    }

    void SetOtherUI(bool show)
    {
        for (int i = 0; i < objectsToHideWhilePuzzleOpen.Length; i++)
        {
            if (objectsToHideWhilePuzzleOpen[i] != null)
                objectsToHideWhilePuzzleOpen[i].SetActive(show);
        }
    }

    void SetPlayerMovement(bool canMove)
    {
        for (int i = 0; i < movementScriptsToDisable.Length; i++)
        {
            if (movementScriptsToDisable[i] != null)
                movementScriptsToDisable[i].enabled = canMove;
        }
    }
}