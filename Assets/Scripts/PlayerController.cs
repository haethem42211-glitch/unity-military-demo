using UnityEngine;

// PlayerController.cs
// تحكم أساسي للاعب بنمط FPS مع CharacterController
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpHeight = 1.2f;
    public float gravity = -9.81f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;
    public Transform cameraHolder;
    float pitch = 0f;

    CharacterController cc;
    Vector3 velocity;

    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Weapon")]
    public Weapon equippedWeapon;

    [Header("UI")]
    public HUDController hud;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        currentHealth = maxHealth;
        if (hud != null) hud.UpdateHealth(currentHealth, maxHealth);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        if (equippedWeapon != null) equippedWeapon.HandleInput();
    }

    void HandleMouseLook()
    {
        float mx = Input.GetAxis("Mouse X") * mouseSensitivity;
        float my = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mx);

        pitch -= my;
        pitch = Mathf.Clamp(pitch, -85f, 85f);
        if (cameraHolder) cameraHolder.localEulerAngles = Vector3.right * pitch;
    }

    void HandleMovement()
    {
        float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        cc.Move(move.normalized * targetSpeed * Time.deltaTime);

        if (cc.isGrounded && velocity.y < 0) velocity.y = -2f; // small grounded snap

        if (Input.GetButtonDown("Jump") && cc.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;
        if (hud != null) hud.UpdateHealth(currentHealth, maxHealth);

        if (currentHealth == 0) Die();
    }

    void Die()
    {
        // سلوك الموت البسيط: إظهار مؤشر الماوس ووقف تحكم اللاعب
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        this.enabled = false;
        if (hud != null) hud.ShowMessage("You died");
    }
}
