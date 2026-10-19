using UnityEngine;
using UnityEngine.UI;

// HUDController.cs
// يعرض الصحة والذخيرة ونصوص مختصرة للمستخدم
public class HUDController : MonoBehaviour
{
    public Text healthText;
    public Text ammoText;
    public Text messageText;

    void Start()
    {
        if (messageText != null) messageText.text = "";
    }

    public void UpdateHealth(int current, int max)
    {
        if (healthText != null) healthText.text = "Health: " + current + " / " + max;
    }

    public void UpdateAmmo(int current, int max)
    {
        if (ammoText != null) ammoText.text = "Ammo: " + current + " / " + max;
    }

    public void ShowMessage(string msg)
    {
        if (messageText != null) messageText.text = msg;
    }

    public void HideMessage()
    {
        if (messageText != null) messageText.text = "";
    }
}
