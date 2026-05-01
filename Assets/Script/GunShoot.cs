using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GunShoot : MonoBehaviour
{
    public Camera playerCamera;
    public float range = 100f;
    public int damage = 25;

    [Header("Ammo")]
    public int magazineSize = 10;
    public int currentAmmo = 10;
    public int maxAmmo = 30;
    public float reloadTime = 1.5f;

    [Header("Shooting")]
    public float fireRate = 0.25f;
    private float nextFireTime = 0f;

    [Header("Recoil")]
    public float recoilAmount = 2f;

    [Header("Effects")]
    public GameObject hitEffect;

    [Header("Simple Muzzle Flash")]
    public GameObject muzzleFlashObject;
    public float flashTime = 0.05f;

    [Header("Crosshair")]
    public Image[] crosshairLines;
    public Color normalCrosshairColor = Color.white;
    public Color hitCrosshairColor = Color.red;
    public float hitCrosshairTime = 0.1f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip emptyAmmoSound;

    [Header("UI")]
    public TMP_Text ammoText;

    private bool isReloading = false;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        currentAmmo = magazineSize;

        if (muzzleFlashObject != null)
            muzzleFlashObject.SetActive(false);

        SetCrosshairColor(normalCrosshairColor);
        UpdateAmmoUI();
    }

    void Update()
    {
        if (isReloading)
        {
            UpdateAmmoUI("Reloading...");
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;

            if (currentAmmo <= 0)
            {
                if (audioSource != null && emptyAmmoSound != null)
                    audioSource.PlayOneShot(emptyAmmoSound);

                UpdateAmmoUI();
                return;
            }

            Shoot();
        }

        UpdateAmmoUI();
    }

    void Shoot()
    {
        currentAmmo--;

        StartCoroutine(ShowMuzzleFlash());

        if (audioSource != null && shootSound != null)
            audioSource.PlayOneShot(shootSound);

        ApplyRecoil();

        Ray ray = playerCamera.ScreenPointToRay(
            new Vector3(Screen.width / 2f, Screen.height / 2f)
        );

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            Debug.Log("Hit: " + hit.collider.name);

            if (hitEffect != null)
            {
                Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }

            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                StartCoroutine(CrosshairHitEffect());
            }
        }

        UpdateAmmoUI();
    }

    System.Collections.IEnumerator ShowMuzzleFlash()
    {
        if (muzzleFlashObject == null) yield break;

        muzzleFlashObject.SetActive(true);
        yield return new WaitForSeconds(flashTime);
        muzzleFlashObject.SetActive(false);
    }

    System.Collections.IEnumerator CrosshairHitEffect()
    {
        SetCrosshairColor(hitCrosshairColor);
        yield return new WaitForSeconds(hitCrosshairTime);
        SetCrosshairColor(normalCrosshairColor);
    }

    void SetCrosshairColor(Color color)
    {
        if (crosshairLines == null) return;

        foreach (Image line in crosshairLines)
        {
            if (line != null)
                line.color = color;
        }
    }

    void ApplyRecoil()
    {
        if (playerCamera == null) return;

        playerCamera.transform.localRotation *= Quaternion.Euler(-recoilAmount, 0f, 0f);
    }

    System.Collections.IEnumerator Reload()
    {
        if (currentAmmo == magazineSize)
            yield break;

        if (maxAmmo <= 0)
            yield break;

        isReloading = true;
        UpdateAmmoUI("Reloading...");

        if (audioSource != null && reloadSound != null)
            audioSource.PlayOneShot(reloadSound);

        yield return new WaitForSeconds(reloadTime);

        int neededAmmo = magazineSize - currentAmmo;
        int ammoToLoad = Mathf.Min(neededAmmo, maxAmmo);

        currentAmmo += ammoToLoad;
        maxAmmo -= ammoToLoad;

        isReloading = false;
        UpdateAmmoUI();
    }

    void UpdateAmmoUI(string customText = "")
    {
        if (ammoText == null) return;

        ammoText.text = customText != "" ? customText : currentAmmo + " / " + maxAmmo;
    }
}