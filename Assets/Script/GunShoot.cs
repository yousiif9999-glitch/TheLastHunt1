using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GunShoot : MonoBehaviour
{
    [Header("Weapon")]
    public GameObject pistolObject;
    public Animator gunAnimator;

    [Header("Camera")]
    public Camera playerCamera;

    [Header("Damage")]
    public float range = 100f;
    public int damage = 25;

    [Header("Ammo")]
    public int magazineSize = 10;
    public int currentAmmo = 10;
    public int maxAmmo = 30;
    public float reloadTime = 1.5f;

    [Header("Shoot")]
    public float fireRate = 0.25f;
    private float nextFireTime;

    [Header("Effects")]
    public GameObject muzzleFlashObject;
    public float flashTime = 0.05f;

    public GameObject hitEffect;

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
    public TMP_Text ammoSlotText;

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
        if (pistolObject == null || !pistolObject.activeSelf)
            return;

        if (isReloading)
            return;

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

                return;
            }

            Shoot();
        }

        UpdateAmmoUI();
    }

    void Shoot()
    {
        currentAmmo--;

        // animation
        if (gunAnimator != null)
            gunAnimator.Play("PistolShoot", 0, 0f);

        // sound
        if (audioSource != null && shootSound != null)
            audioSource.PlayOneShot(shootSound);

        // muzzle flash
        StartCoroutine(ShowMuzzleFlash());

        // raycast
        Ray ray = playerCamera.ScreenPointToRay(
            new Vector3(Screen.width / 2f, Screen.height / 2f)
        );

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            if (hitEffect != null)
            {
                Instantiate(
                    hitEffect,
                    hit.point,
                    Quaternion.LookRotation(hit.normal)
                );
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

    IEnumerator ShowMuzzleFlash()
    {
        if (muzzleFlashObject == null)
            yield break;

        muzzleFlashObject.SetActive(true);

        yield return new WaitForSeconds(flashTime);

        muzzleFlashObject.SetActive(false);
    }

    IEnumerator CrosshairHitEffect()
    {
        SetCrosshairColor(hitCrosshairColor);

        yield return new WaitForSeconds(hitCrosshairTime);

        SetCrosshairColor(normalCrosshairColor);
    }

    void SetCrosshairColor(Color color)
    {
        if (crosshairLines == null)
            return;

        foreach (Image line in crosshairLines)
        {
            if (line != null)
                line.color = color;
        }
    }

    IEnumerator Reload()
    {
        if (currentAmmo == magazineSize || maxAmmo <= 0)
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
        string finalText =
            customText != ""
            ? customText
            : currentAmmo + " / " + maxAmmo;

        if (ammoText != null)
            ammoText.text = finalText;

        if (ammoSlotText != null)
            ammoSlotText.text = finalText;
    }
}