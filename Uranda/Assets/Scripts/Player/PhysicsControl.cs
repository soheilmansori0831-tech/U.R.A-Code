using Unity.VisualScripting;
using UnityEngine;

public class PhysicsControl : MonoBehaviour
{
    public Rigidbody2D rb;
    private Player player;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteSetTime;
    public float coyoteTimer;

    [Header("Ground")]
    [SerializeField] private float groundRayLength;
    [SerializeField] private Transform leftPint;
    [SerializeField] private Transform rightPint;    
    [SerializeField] private LayerMask whatToDetect;
    public bool grounded;
    private RaycastHit2D hitInfoLeft;
    private RaycastHit2D hitInfoRight;

    [Header("Wall")]
    [SerializeField] private float wallRayLength;
    [SerializeField] private Transform upperPint;
    [SerializeField] private Transform lowerPint;
    public bool wallDetected;
    private RaycastHit2D hitInfoUpper;
    private RaycastHit2D hitInfoLower;
    public float gravityValue;

    [Header("Ceiling")]
    [SerializeField] private float ceilingRayLength;
    [SerializeField] private Transform leftCeilingPint;
    [SerializeField] private Transform righCeilingtPint;   
    public bool ceilingDetected;
    private RaycastHit2D hitInfoCeilingLeft;
    private RaycastHit2D hitInfoCeilingRight;

    [Header("Colliders")]
    [SerializeField] private BoxCollider2D standColl;
    [SerializeField] private BoxCollider2D crouchColl;
    [SerializeField] private BoxCollider2D standDamageColl;
    [SerializeField] private BoxCollider2D crouchDamageColl;

    [Header("Weapon Points")]
    [SerializeField] private Transform weaponStandPos;
    [SerializeField] private Transform weaponCrouchPos;

    private void Start()
    {
        player = GetComponent<Player>();
        gravityValue = rb.gravityScale;
        StandColliders();
        coyoteTimer = coyoteSetTime;
    }

    private bool CheckGround()
    {
        hitInfoLeft = Physics2D.Raycast(leftPint.position, Vector2.down, groundRayLength, whatToDetect);
        hitInfoRight = Physics2D.Raycast(rightPint.position, Vector2.down, groundRayLength, whatToDetect);

        if (hitInfoLeft || hitInfoRight) 
            return true;

        return false;
    }

    private bool CheckWall()
    {
        hitInfoUpper = Physics2D.Raycast(upperPint.position,
         player.facingRight ? Vector2.right : Vector2.left, wallRayLength, whatToDetect);
        hitInfoLower = Physics2D.Raycast(lowerPint.position,
         player.facingRight ? Vector2.right : Vector2.left, wallRayLength, whatToDetect);

        if (hitInfoUpper && hitInfoLower) 
            return true;

        return false;
    }

    private bool CheckCeiling()
    {
        hitInfoCeilingLeft = Physics2D.Raycast(leftCeilingPint.position, Vector2.up, ceilingRayLength, whatToDetect);
        hitInfoCeilingRight = Physics2D.Raycast(righCeilingtPint.position, Vector2.up, ceilingRayLength, whatToDetect);

        if (hitInfoCeilingLeft || hitInfoCeilingRight) 
            return true;

        return false;
    }

    private void Update()
    {
        if (!grounded)
        {
            coyoteTimer -= Time.deltaTime;
        } else
        {
            coyoteTimer = coyoteSetTime;
        }
    }

    private void FixedUpdate()
    {
        grounded = CheckGround();
        wallDetected = CheckWall();
        ceilingDetected = CheckCeiling();
    }

    public void DisableGravity()
    {
        rb.gravityScale = 0;
    }

    public void EnableGravity()
    {
        rb.gravityScale = gravityValue;
    }

    public void ResetVelocity()
    {
        rb.linearVelocity = Vector2.zero;
    }

    public void StandColliders() {
        standColl.enabled = true;
        standDamageColl.enabled = true;

        crouchDamageColl.enabled = false;
        crouchColl.enabled = false;

        // Player.instance.currentWeaponPrefab.transform.position = weaponStandPos.position;
    }

    public void CrouchColliders()
    {
        standColl.enabled = false;
        standDamageColl.enabled = false;
        
        crouchDamageColl.enabled = true;
        crouchColl.enabled = true;

        // Player.instance.currentWeaponPrefab.transform.position = weaponCrouchPos.position;
    }

    public void DisableDamageColliders() {
        standDamageColl.enabled = false;
        crouchDamageColl.enabled = false;
    }

    public void EnableDamageColliders() {
        standDamageColl.enabled = true;
        crouchDamageColl.enabled = true;
    }

    public void SetInterpolate()
    {
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    public void SetExtrapolate()
    {
        rb.interpolation = RigidbodyInterpolation2D.Extrapolate;
    }
}
