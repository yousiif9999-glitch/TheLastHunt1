using UnityEngine;

public class LetterInteraction : MonoBehaviour
{
    public GameObject letterPanel;
    public GameObject pressEText;
    public float interactionDistance = 3f;

    [Header("Canvas")]
    public Canvas mainCanvas;

    [Header("Text Position")]
    public float promptScreenYOffset = 80f;

    [Header("Paper Sound")]
    public AudioSource paperAudioSource;
    public AudioClip paperOpenSound;

    [Header("Player Movement Scripts To Disable While Letter Is Open")]
    public MonoBehaviour[] movementScriptsToDisable;

    private Camera playerCamera;
    private bool letterOpen = false;

    private RectTransform pressETextRect;
    private RectTransform canvasRect;

    void Start()
    {
        playerCamera = Camera.main;

        if (mainCanvas == null)
            mainCanvas = FindObjectOfType<Canvas>();

        if (mainCanvas != null)
            canvasRect = mainCanvas.GetComponent<RectTransform>();

        if (pressEText != null)
        {
            pressETextRect = pressEText.GetComponent<RectTransform>();
            pressEText.SetActive(false);
        }

        if (letterPanel != null)
            letterPanel.SetActive(false);
    }

    void Update()
    {
        if (!letterOpen)
        {
            CheckForLetter();
        }

        if (letterOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseLetter();
        }
    }

    void CheckForLetter()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.CompareTag("Letter"))
            {
                if (pressEText != null)
                {
                    MoveTextUnderLetter(hit.collider);
                    pressEText.SetActive(true);
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    OpenLetter();
                }

                return;
            }
        }

        if (pressEText != null)
            pressEText.SetActive(false);
    }

    void MoveTextUnderLetter(Collider letterCollider)
    {
        if (pressETextRect == null || canvasRect == null)
            return;

        Vector3 letterWorldPosition = letterCollider.bounds.center;
        Vector3 screenPosition = playerCamera.WorldToScreenPoint(letterWorldPosition);

        screenPosition.y -= promptScreenYOffset;

        Vector2 localPosition;

        Camera canvasCamera = null;

        if (mainCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            canvasCamera = mainCanvas.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPosition,
            canvasCamera,
            out localPosition
        );

        pressETextRect.anchoredPosition = localPosition;
    }

    void OpenLetter()
    {
        letterOpen = true;

        if (letterPanel != null)
            letterPanel.SetActive(true);

        if (pressEText != null)
            pressEText.SetActive(false);

        if (paperAudioSource != null && paperOpenSound != null)
        {
            paperAudioSource.PlayOneShot(paperOpenSound);
        }

        SetPlayerMovement(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void CloseLetter()
    {
        letterOpen = false;

        if (letterPanel != null)
            letterPanel.SetActive(false);

        if (pressEText != null)
            pressEText.SetActive(false);

        SetPlayerMovement(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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