using UnityEngine;
using TMPro;

public class AmmoPickup : MonoBehaviour
{
    public int ammoAmount = 10;
    public float pickupRange = 5f;
    public float collectDistance = 1.5f;
    public float moveSpeed = 12f;

    private Transform player;
    private GunShoot gunShoot;
    private AmmoFloat ammoFloat;

    public static TMP_Text pickupText;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
            gunShoot = playerObj.GetComponent<GunShoot>();
        }

        ammoFloat = GetComponent<AmmoFloat>();

        GameObject textObj = GameObject.Find("PickupText");
        if (textObj != null)
            pickupText = textObj.GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (player == null || gunShoot == null) return;

        Vector3 targetPos = player.position + Vector3.up * 1.5f;
        float distance = Vector3.Distance(transform.position, targetPos);

        if (distance <= pickupRange)
        {
            if (ammoFloat != null)
                ammoFloat.enabled = false;

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );
        }

        if (distance <= collectDistance)
        {
            gunShoot.maxAmmo += ammoAmount;

            if (pickupText != null)
                pickupText.text = "+" + ammoAmount + " Ammo";

            Destroy(gameObject);
        }
    }
}