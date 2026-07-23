using UnityEngine;

public enum PlayerState
{
    Idle = 0,
    Walk = 1,
    Run = 2,
    Sit = 3,
    SitWalk = 4,
    Jump = 5,
    Fall = 6,
    Attack = 7
}

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float sitWalkSpeed;
    [SerializeField] private float jumpMoveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCD;

    [Header("Joystick Thresholds")]
    [SerializeField] private float walkThreshold = 0.3f;
    [SerializeField] private float jumpSitThreshold = 0.5f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    // [Header("sit collider")]
    [Header("Refrences")]
    public Rigidbody2D rb;
    public Joystick joystick;
    public Animator spriteAnimator;

    private static readonly int StateHash =
    Animator.StringToHash("State");

    private PlayerState state;
    private bool canJump = true;
    private bool isSitting = false;
    private bool isGrounded;
    private bool canMove = true;

    public static PlayerMovement instance;
    void Awake() { instance = this; }

    private void FixedUpdate()
    {
        GroundCheck();

        float h = joystick.Horizontal;
        float v = joystick.Vertical;

        float absH = Mathf.Abs(h);

        isSitting = v < -jumpSitThreshold;

        // Jump
        if (v > jumpSitThreshold && isGrounded && canJump)
        {
            Jump();
        }

        // Movement
        if (absH < walkThreshold)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        else if (canMove)
        {
            float speed;

            if (!isGrounded)
                speed = jumpMoveSpeed;
            else if (isSitting)
                speed = sitWalkSpeed;
            else
                speed = absH < 0.7f ? walkSpeed : runSpeed;

            rb.linearVelocity = new Vector2(Mathf.Sign(h) * speed, rb.linearVelocity.y);

            Flip(h);
        }

        UpdateState(absH);
    }

    private void GroundCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            groundCheckPoint.position,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        isGrounded = hit.collider != null;
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        SetState(PlayerState.Jump);

        canJump = false;
        Invoke(nameof(ResetJumpCD), jumpCD);
    }

    private void Flip(float h)
    {
        if (h > 0.05f)
            transform.localScale = new Vector3(2.5f, transform.localScale.y, transform.localScale.z);
        else if (h < -0.05f)
            transform.localScale = new Vector3(-2.5f, transform.localScale.y, transform.localScale.z);
    }

    private void ResetJumpCD()
    {
        canJump = true;
    }

    private void UpdateState(float absH)
    {
        if (!isGrounded)
        {
            if (rb.linearVelocity.y > 0.1f)
                SetState(PlayerState.Jump);
            else
                SetState(PlayerState.Fall);

            return;
        }

        if (isSitting)
        {
            if (absH < walkThreshold)
                SetState(PlayerState.Sit);
            else
                SetState(PlayerState.SitWalk);

            return;
        }

        if (absH < walkThreshold)
        {
            SetState(PlayerState.Idle);
        }
        else if (absH < 0.7f)
        {
            SetState(PlayerState.Walk);
        }
        else
        {
            SetState(PlayerState.Run);
        }
    }

    private void SetState(PlayerState newState)
    {
        if (state == newState)
            return;

        state = newState;
        spriteAnimator.SetInteger(StateHash, (int)state);
    }

    private void OnDrawGizmos()
    {
        if (groundCheckPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            groundCheckPoint.position,
            groundCheckPoint.position + Vector3.down * groundCheckDistance
        );
    }

}
