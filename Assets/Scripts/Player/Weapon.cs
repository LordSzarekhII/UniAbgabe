using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public int damage = 15;
    public float fireRate = 0.15f;
    public float maxRange = 100f;

    [Header("Ammo")]
    public int magazineSize = 30;
    public float reloadTime = 1.5f;

    [Header("References")]
    public Transform firePoint;
    public Camera playerCamera;

    [Header("Muzzle Flash")]
    public Light muzzleFlashLight;
    public float flashDuration = 0.05f;

    private float nextFireTime;
    private int currentAmmo;
    private bool isReloading;
    private float reloadEndTime;

    public bool IsShooting { get; private set; }
    public bool IsReloading => isReloading;

    public static event Action<int, int> OnAmmoChanged; // current, magazineSize
    public static event Action OnReloadStarted;

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
        if (muzzleFlashLight == null)
            muzzleFlashLight = GetComponentInChildren<Light>(true);

        currentAmmo = magazineSize;
        OnAmmoChanged?.Invoke(currentAmmo, magazineSize);
    }

    private void Update()
    {
        var mouse = Mouse.current;
        var keyboard = Keyboard.current;

        // Handle reload
        if (isReloading)
        {
            if (Time.time >= reloadEndTime)
            {
                FinishReload();
            }
            IsShooting = false;
            return;
        }

        // Start reload on R key or when trying to fire with empty mag
        if (keyboard != null && keyboard.rKey.wasPressedThisFrame && currentAmmo < magazineSize)
        {
            StartReload();
            IsShooting = false;
            return;
        }

        IsShooting = mouse != null && mouse.leftButton.isPressed;

        // Disable shooting while sprinting
        if (keyboard != null && keyboard.leftShiftKey.isPressed)
        {
            IsShooting = false;
        }

        if (IsShooting && Time.time >= nextFireTime)
        {
            if (currentAmmo <= 0)
            {
                StartReload();
                return;
            }

            Shoot();
            nextFireTime = Time.time + fireRate;
        }

        if (muzzleFlashLight != null && muzzleFlashLight.enabled)
        {
            if (Time.time >= nextFireTime - fireRate + flashDuration)
            {
                muzzleFlashLight.enabled = false;
            }
        }
    }

    private void StartReload()
    {
        isReloading = true;
        reloadEndTime = Time.time + reloadTime;
        OnReloadStarted?.Invoke();
    }

    private void FinishReload()
    {
        currentAmmo = magazineSize;
        isReloading = false;
        OnAmmoChanged?.Invoke(currentAmmo, magazineSize);
    }

    private void Shoot()
    {
        currentAmmo--;
        OnAmmoChanged?.Invoke(currentAmmo, magazineSize);

        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.enabled = true;
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, maxRange))
        {
            EnemyBase enemy = hit.collider.GetComponentInParent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
