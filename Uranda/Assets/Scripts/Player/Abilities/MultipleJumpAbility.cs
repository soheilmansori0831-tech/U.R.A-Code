using UnityEngine;
using UnityEngine.InputSystem;

public class MultipleJumpAbility : BaseAbility
{
    public InputActionReference jumpActionRef;
    [SerializeField] private float jumpForce;

    [SerializeField] private int maxNumberOfJumps;
    private int numberOfJumps;
    private bool canActivateAditionalJumps;

    [SerializeField] private float airSpeed;
    [SerializeField] private float minimumAirTime;
    private float startMinimumAirTime;

    [SerializeField] private float setMaxJumpTime;
    [SerializeField] private float gravityDivider;
    private float jumpTimer;
    private bool jumping;

    private string jumpAnimationParameterName = "Jump";
    private string ySpeedAnimationParameterName = "Yspeed";
    private int jumpParameterID;
    private int ySpeedParameterID;

    public override void Initialization()
    {
        base.Initialization();
        startMinimumAirTime = minimumAirTime;
        jumpParameterID = Animator.StringToHash(jumpAnimationParameterName);
        ySpeedParameterID = Animator.StringToHash(ySpeedAnimationParameterName);
        numberOfJumps = maxNumberOfJumps;
    }

    public override void ExitAbility()
    {
        linkedPhysicsControl.EnableGravity();
        canActivateAditionalJumps = false;
        // numberOfJumps = maxNumberOfJumps;
    }

    public override void ProccessAbility()
    {
        if (jumping)
        {
            jumpTimer -= Time.deltaTime;
            if (jumpTimer <= 0)
            {
                jumping = false;
            }
        }

        player.Flip();
        minimumAirTime -= Time.deltaTime;
        if (linkedPhysicsControl.grounded && minimumAirTime <= 0)
        {
            if (linkedInput.horizontalInput == 0)
                linkedStateMachine.ChangeState(PlayerStates.State.Idle);
            else
                linkedStateMachine.ChangeState(PlayerStates.State.Run);
        }
        if (!linkedPhysicsControl.grounded && linkedPhysicsControl.wallDetected)
        {
            if (linkedPhysicsControl.rb.linearVelocityY < 0)
            {
                linkedStateMachine.ChangeState(PlayerStates.State.WallSlide);
            }
        }
    }

    public override void ProccessFixedAbility()
    {
        if (!linkedPhysicsControl.grounded)
        {
            if (jumping) 
                linkedPhysicsControl.rb.linearVelocity = 
                  new Vector2(airSpeed * linkedInput.horizontalInput, jumpForce);
            else
                linkedPhysicsControl.rb.linearVelocity = 
                  new Vector2(airSpeed * linkedInput.horizontalInput, linkedPhysicsControl.rb.linearVelocityY);
        }

        if (linkedPhysicsControl.rb.linearVelocityY < 0)
        {
            linkedPhysicsControl.rb.gravityScale = linkedPhysicsControl.gravityValue / gravityDivider;
        }
    }

    private void OnEnable()
    {
        jumpActionRef.action.performed += TryToJump;
        jumpActionRef.action.canceled += StopJump;
    }

    private void OnDisable()
    {
        jumpActionRef.action.performed -= TryToJump;
        jumpActionRef.action.canceled -= StopJump;
    }

    private void TryToJump(InputAction.CallbackContext value)
    {
        if (!isPermitted || linkedStateMachine.currentState == PlayerStates.State.Reload
          || linkedStateMachine.currentState == PlayerStates.State.Death
          || linkedStateMachine.currentState == PlayerStates.State.KnockBack)
        {
            return;
        }

        if (linkedStateMachine.currentState == PlayerStates.State.Ladders)
        {
            Jump();
            return;
        }

        if (linkedPhysicsControl.coyoteTimer > 0)
        {
            Jump();
            linkedPhysicsControl.coyoteTimer = -1;
            return;
        }

        if (numberOfJumps > 0 && canActivateAditionalJumps)
        {
            linkedPhysicsControl.EnableGravity();
            jumping = true;
            jumpTimer = setMaxJumpTime;

            numberOfJumps -= 1;
            linkedPhysicsControl.coyoteTimer = -1;

            linkedPhysicsControl.rb.linearVelocity = 
            new Vector2(airSpeed * linkedInput.horizontalInput, jumpForce);
            minimumAirTime = startMinimumAirTime;
        } else
        {
            canActivateAditionalJumps = false;
        }
    }

    private void Jump()
    {
        jumping = true;
        jumpTimer = setMaxJumpTime;

        canActivateAditionalJumps = true;
        numberOfJumps = maxNumberOfJumps;
        numberOfJumps -= 1;

        linkedStateMachine.ChangeState(PlayerStates.State.Jump);
        linkedPhysicsControl.rb.linearVelocity = 
          new Vector2(airSpeed * linkedInput.horizontalInput, jumpForce);
        minimumAirTime = startMinimumAirTime;
    }

    private void StopJump(InputAction.CallbackContext value)
    {
        jumping = false;
    }

    public override void UpdateAnimator()
    {
        linkedAnimator.SetBool(jumpAnimationParameterName,
          linkedStateMachine.currentState == thisAbilityState || 
          linkedStateMachine.currentState == PlayerStates.State.WallJump);
        linkedAnimator.SetFloat(ySpeedAnimationParameterName, linkedPhysicsControl.rb.linearVelocityY);
    }

    public void SetMaxJumpNumber(int maxJumps)
    {
        maxNumberOfJumps = maxJumps;
    }
}
