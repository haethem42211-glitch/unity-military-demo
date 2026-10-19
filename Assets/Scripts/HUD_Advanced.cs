// HUD_Advanced.cs
// واجهة مستخدم محسنة: Health bar (Slider)، Ammo text، Crosshair، Damage flash

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Advanced : MonoBehaviour
{
    [Header("UI References")]
    public Slider healthBar;
    public Text healthText;
    public Text ammoText;
    public Image crosshair;
    public Image damageFlash;

    [Header("Damage Flash")]
    public float flashDuration = 0.25f;

    void Start()
    {
        if (damageFlash != null) damageFlash.color = new Color(1,0,0,0);
    }

    public void UpdateHealth(int current, int max)
    {
        if (healthBar != null) { healthBar.maxValue = max; healthBar.value = current; }
        if (healthText != null) healthText.text = current + " / " + max;
    }

    public void UpdateAmmo(int current, int magSize, int spare)
    {
        if (ammoText != null) ammoText.text = current + " / " + magSize + " (" + spare + ")";
    }

    public void ShowMessage(string msg)
    {
        // يمكن إضافة نص مؤقت هنا
        Debug.Log(msg);
    }

    public void FlashDamage()
    {
        if (damageFlash != null) StartCoroutine(DoFlash());
    }

    IEnumerator DoFlash()
    {
        float t = 0f;
        while (t < flashDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(0.6f, 0f, t/flashDuration);
            damageFlash.color = new Color(1,0,0,a);
            yield return null;
        }
        damageFlash.color = new Color(1,0,0,0);
    }
}
