using UnityEngine;
using TMPro;
using System.Collections;

public class PickupTextTimer : MonoBehaviour
{
    private Coroutine currentRoutine;

    public void ShowForSeconds(float seconds)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(HideAfter(seconds));
    }

    IEnumerator HideAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        TMP_Text text = GetComponent<TMP_Text>();
        if (text != null)
            text.text = "";
    }
}