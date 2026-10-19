using System.Collections;
using UnityEngine;

// Weapon.cs
// سلاح بسيط يعتمد على Raycast: يدعم ذخيرة، إطلاق بمعدل، وإعادة تحميل
public class Weapon : MonoBehaviour
{
    public int damage = 20;
    public float range = 100f;
    public float fireRate = 5f; // طلقة في الثانية
    public int magazineSize = 30;
    public int ammoInMag;
    public float reloadTime = 1.5f;

    public ParticleSystem muzzleFlash;
    public Camera fpsCamera;

    bool isReloading = false;
    float nextTimeToFire = 0f;

    public HUDController hud;
    public PlayerController player;

    void Start()
    {
        ammoInMag = magazineSize;
        if (hud != null) hud.UpdateAmmo(ammoInMag, magazineSize);
    }

    public void HandleInput()
    {
        if (isReloading) return;

        if (Input.GetKeyDown(KeyCode.R)) StartCoroutine(Reload());

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (ammoInMag <= 0)
        {
            // صوت نيران فارغة أو تلميح للمستخدم
            return;
        }

        ammoInMag--;
        if (hud != null) hud.UpdateAmmo(ammoInMag, magazineSize);

        if (muzzleFlash != null) muzzleFlash.Play();

        RaycastHit hit;
        if (fpsCamera == null) fpsCamera = Camera.main;
        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, range))
        {
            // حاول الوصول إلى مكوّن يستجيب للضرر
            var enemy = hit.transform.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            else
            {
                var player = hit.transform.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(damage);
                }
            }
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        if (hud != null) hud.ShowMessage("Reloading...");
        yield return new WaitForSeconds(reloadTime);
        ammoInMag = magazineSize;
        isReloading = false;
        if (hud != null) hud.UpdateAmmo(ammoInMag, magazineSize);
        if (hud != null) hud.HideMessage();
    }
}
