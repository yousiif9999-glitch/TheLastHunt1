using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public GameObject itemToEnable;

    private bool canPickup = false;

    void Update()
    {
        if (canPickup && Input.GetKeyDown(KeyCode.E))
        {
            itemToEnable.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canPickup = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canPickup = false;
        }
    }
}