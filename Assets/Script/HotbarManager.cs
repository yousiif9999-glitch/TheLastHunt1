using UnityEngine;
using UnityEngine.UI;

public class HotbarManager : MonoBehaviour
{
    [Header("Hotbar Slot Outlines")]
    public Outline[] slotOutlines;

    [Header("Settings")]
    public int defaultSelectedSlot = 0;

    private int selectedSlot;

    void Start()
    {
        SelectSlot(defaultSelectedSlot);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectSlot(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectSlot(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectSlot(2);
        }
    }

    public void SelectSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slotOutlines.Length)
        {
            return;
        }

        selectedSlot = slotIndex;

        for (int i = 0; i < slotOutlines.Length; i++)
        {
            if (slotOutlines[i] != null)
            {
                slotOutlines[i].enabled = i == selectedSlot;
            }
        }

        Debug.Log("Selected hotbar slot: " + (selectedSlot + 1));
    }

    public int GetSelectedSlot()
    {
        return selectedSlot;
    }
}