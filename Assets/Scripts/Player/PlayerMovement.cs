using Mono.Cecil;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float walkSpeed = 4f;
    [SerializeField] float runSpeed = 7f;
    [SerializeField] float acceleration = 30f;
    [SerializeField] float deceleration = 40f;

    [Header("Jump")]
    [SerializeField] float jumpSpeed = 15f;
    [SerializeField] float fallGravityMultiplier = 1.5f;
    [SerializeField] float jumpCutMultiplier = 0.5f;

    [Header("Ground")]
    [SerializeField] LayerMask groundLayer;

    Rigidbody2D rb;
    BoxCollider2D col;
    SpriteRenderer sprite;

    float baseGravity;
    float moveInput;
    bool running;
    bool jumpPressed;
    bool jumpReleased;

    public bool IsGrounded {  get; private set; }
    public float HorizontalSpeed => rb.linearVelocity.x;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        baseGravity = rb.gravityScale;
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        moveInput = 0f;
        if (kb.leftArrowKey.isPressed || kb.qKey.isPressed) moveInput -= 1f;
        if (kb.rightArrowKey.isPressed || kb.dKey.isPressed) moveInput += 1f;

        running = kb.leftShiftKey.isPressed;

        if (kb.spaceKey.wasPressedThisFrame) jumpPressed = true;
        if (kb.spaceKey.wasReleasedThisFrame) jumpReleased = true;

        if (moveInput > 0f) sprite.flipX = false;
        else if (moveInput < 0f) sprite.flipX = true;
    }

    private void FixedUpdate()
    {
        CheckGround();
        Move();
        Jump();
        ApplyGravity();
    }

    void CheckGround()
    {
        Bounds b = col.bounds;
        Vector2 size = new (b.size.x * 0.9f, b.size.y);
        IsGrounded = Physics2D.BoxCast(b.center, size, 0f, Vector2.down, 0.05f, groundLayer);
    }

    void Move()
    {
        float target = moveInput * (running ? runSpeed : walkSpeed);
        float rate = Mathf.Abs(target) > 0.01f ? acceleration : deceleration;
        float vx = Mathf.MoveTowards(rb.linearVelocity.x, target, rate * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(vx, rb.linearVelocity.y);
    }

    void Jump()
    {
        if (jumpPressed && IsGrounded)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);

        if (jumpReleased && rb.linearVelocity.y > 0f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);

        jumpPressed = false;
        jumpReleased = false;
    }

    void ApplyGravity()
    {
        rb.gravityScale = rb.linearVelocity.y < 0f ? baseGravity * fallGravityMultiplier : baseGravity;
    }
}
