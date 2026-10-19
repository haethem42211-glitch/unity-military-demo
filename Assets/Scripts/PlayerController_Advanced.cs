// PlayerController_Advanced.cs
// نسخة محسنة من تحكم اللاعب لأول شخص
// الميزات: حركة سلسة، جري، انحناء (crouch)، ميزة التصويب (ADS)، تأثير ارتداد سلاح بسيط، تماشي مع CharacterController

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController_Advanced : MonoBehaviour
{
    [Header("References")]
    public Transform cameraHolder; // فرع الكاميرا
    public Weapon_Advanced weapon; // السلاح المزود
    public HUD_Advanced hud;

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float acceleration = 10f;
    public float crouchHeight = 1f;
    public float standHeight = 2f;
    public float crouchSpeedMultiplier = 0.5f;

    [Header("Look")]
    public float mouseSensitivity = 1.5f;
    public float mouseSmoothTime = 0.05f;
    public float maxPitch = 85f;

    [Header("Jump & Gravity")]
    public float gravity = -9.81f;
    public float jumpHeight = 1.2f;

    CharacterController cc;
    Vector3 velocity;
    float currentSpeed;
    float targetSpeed;

    // mouse smoothing
    Vector2 currentMouseDelta;
    Vector2 currentMouseDeltaVelocity;
    float pitch = 0f;

    bool isCrouching = false;

    [HideInInspector]
    public int maxHealth = 100;
    [HideInInspector]
    public int currentHealth;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        currentHealth = maxHealth;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();

        if (weapon != null) weapon.HandleInput();
    }

    void HandleMouseLook()
    {
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity;
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        // yaw
        transform.Rotate(Vector3.up * currentMouseDelta.x);

        // pitch
        pitch -= currentMouseDelta.y;
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);
        if (cameraHolder) cameraHolder.localEulerAngles = Vector3.right * pitch;
    }

    void HandleMovement()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");

        bool wantRun = Input.GetKey(KeyCode.LeftShift);
        bool wantCrouch = Input.GetKey(KeyCode.LeftControl);

        targetSpeed = wantRun ? runSpeed : walkSpeed;
        if (wantCrouch) targetSpeed *= crouchSpeedMultiplier;

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

        Vector3 desiredMove = transform.right * inputX + transform.forward * inputZ;
        cc.Move(desiredMove.normalized * currentSpeed * Time.deltaTime);

        // jump
        if (cc.isGrounded && velocity.y < 0) velocity.y = -2f;
        if (Input.GetButtonDown("Jump") && cc.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // gravity
        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);

        // crouch (height lerp)
        if (wantCrouch && !isCrouching) StartCoroutine(SetCrouch(true));
        if (!wantCrouch && isCrouching) StartCoroutine(SetCrouch(false));
    }

    IEnumerator SetCrouch(bool crouch)
    {
        isCrouching = crouch;
        float start = cc.height;
        float end = crouch ? crouchHeight : standHeight;
        float t = 0f;
        float dur = 0.15f;
        Vector3 camStart = cameraHolder.localPosition;
        Vector3 camEnd = new Vector3(camStart.x, end - 0.3f, camStart.z);

        while (t < dur)
        {
            t += Time.deltaTime;
            float f = t / dur;
            cc.height = Mathf.Lerp(start, end, f);
            cameraHolder.localPosition = Vector3.Lerp(camStart, camEnd, f);
            yield return null;
        }

        cc.height = end;
        cameraHolder.localPosition = camEnd;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;
        if (hud != null) hud.UpdateHealth(currentHealth, maxHealth);

        // damage feedback
        if (hud != null) hud.FlashDamage();

        if (currentHealth == 0) Die();
    }

    void Die()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // disable player control
        this.enabled = false;
        if (hud != null) hud.ShowMessage("لقد هُزمت");
    }
}
