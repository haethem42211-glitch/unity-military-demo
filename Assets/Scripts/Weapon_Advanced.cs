// Weapon_Advanced.cs
// سلاح متقدم: يدعم أوضاع إطلاق (آلي، نصف آلي)، ارتداد بصري، تأثير الطلقات، صوت، وقواعد ذخيرية أفضل

using System.Collections;
using UnityEngine;

public enum FireMode { Auto, Semi }

public class Weapon_Advanced : MonoBehaviour
{
    [Header("References")]
    public Camera fpsCamera;
    public ParticleSystem muzzleFlash;
    public GameObject impactPrefab;
    public AudioSource audioSource;
    public AudioClip shotClip;
    public AudioClip reloadClip;

    [Header("Stats")]
    public FireMode fireMode = FireMode.Auto;
    public int damage = 25;
    public float range = 100f;
    public float fireRate = 10f; // rounds per second for auto
    public int magazineSize = 30;
    public int ammoInMag;
    public int spareAmmo = 90;
    public float reloadTime = 2f;
    public LayerMask hitMask = ~0; // what layers can be hit

    [Header("Recoil")]
    public float recoilPitch = 1.2f;
    public float recoilYaw = 0.6f;
    public float recoilRecovery = 8f;

    [Header("References")]
    public HUD_Advanced hud;
    public PlayerController_Advanced player;

    bool isReloading = false;
    float nextTimeToFire = 0f;

    Vector2 recoilVelocity;
    Vector2 recoilAngles;

    void Start()
    {
        if (fpsCamera == null) fpsCamera = Camera.main;
        ammoInMag = magazineSize;
        if (hud != null) hud.UpdateAmmo(ammoInMag, magazineSize, spareAmmo);
    }

    public void HandleInput()
    {
        if (isReloading) return;

        if (Input.GetKeyDown(KeyCode.R)) StartCoroutine(Reload());

        bool firePressed = Input.GetButton("Fire1");
        bool fireDown = Input.GetButtonDown("Fire1");

        if (fireMode == FireMode.Auto && firePressed && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }
        else if (fireMode == FireMode.Semi && fireDown)
        {
            Shoot();
        }

        // apply recoil recovery each frame
        recoilAngles = Vector2.Lerp(recoilAngles, Vector2.zero, recoilRecovery * Time.deltaTime);
        if (player != null && player.cameraHolder != null)
        {
            player.cameraHolder.localEulerAngles = new Vector3(-player.cameraHolder.localEulerAngles.x + recoilAngles.x, 0, 0);
        }
    }

    void Shoot()
    {
        if (ammoInMag <= 0) { if (hud != null) hud.ShowMessage("لا توجد ذخيرة - أعد التحميل"); return; }

        ammoInMag--;
        if (hud != null) hud.UpdateAmmo(ammoInMag, magazineSize, spareAmmo);

        if (muzzleFlash != null) muzzleFlash.Play();
        if (audioSource != null && shotClip != null) audioSource.PlayOneShot(shotClip);

        // recoil
        recoilAngles.x += Random.Range(recoilPitch * 0.8f, recoilPitch * 1.2f);
        recoilAngles.y += Random.Range(-recoilYaw, recoilYaw);

        RaycastHit hit;
        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, range, hitMask))
        {
            // apply damage
            var enemy = hit.transform.GetComponent<EnemyAI_Advanced>();
            if (enemy != null) enemy.TakeDamage(damage);

            if (impactPrefab != null)
            {
                var go = Instantiate(impactPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(go, 5f);
            }
        }
    }

    IEnumerator Reload()
    {
        if (isReloading) yield break;
        if (spareAmmo <= 0 && ammoInMag == 0) { if (hud != null) hud.ShowMessage("خالية من الذخيرة"); yield break; }

        isReloading = true;
        if (audioSource != null && reloadClip != null) audioSource.PlayOneShot(reloadClip);
        if (hud != null) hud.ShowMessage("إعادة تحميل...");

        yield return new WaitForSeconds(reloadTime);

        int needed = magazineSize - ammoInMag;
        int taken = Mathf.Min(needed, spareAmmo);
        ammoInMag += taken;
        spareAmmo -= taken;

        isReloading = false;
        if (hud != null) { hud.UpdateAmmo(ammoInMag, magazineSize, spareAmmo); hud.HideMessage(); }
    }
}
