using UnityEngine;

public class LetterInteraction : MonoBehaviour
{
    public GameObject letterPanel;
    public GameObject pressEText;
    public float interactionDistance = 3f;

    private Camera playerCamera;
    private bool letterOpen = false;

    void Start()
    {
        playerCamera = Camera.main;

        if (letterPanel != null)
            letterPanel.SetActive(false);

        if (pressEText != null)
            pressEText.SetActive(false);
    }

    void Update()
    {
        CheckForLetter();

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
                if (pressEText != null && !letterOpen)
                    pressEText.SetActive(true);

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

    void OpenLetter()
    {
        letterOpen = true;

        if (letterPanel != null)
            letterPanel.SetActive(true);

        if (pressEText != null)
            pressEText.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void CloseLetter()
    {
        letterOpen = false;

        if (letterPanel != null)
            letterPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
