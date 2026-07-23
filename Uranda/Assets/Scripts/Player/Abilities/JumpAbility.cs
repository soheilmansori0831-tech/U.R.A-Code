using UnityEngine;
using UnityEngine.InputSystem;

public class JumpAbility : BaseAbility
{
    public InputActionReference jumpActionRef;
    [SerializeField] private float jumpForce;
    [SerializeField] private float airSpeed;
    [SerializeField] private float minimumAirTime;
    private float startMinimumAirTime;

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
    }

    public override void ProccessAbility()
    {
        player.Flip();
        minimumAirTime -= Time.deltaTime;
        if (linkedPhysicsControl.grounded && minimumAirTime <= 0)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Idle);
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
            linkedPhysicsControl.rb.linearVelocity = 
              new Vector2(airSpeed * linkedInput.horizontalInput, linkedPhysicsControl.rb.linearVelocityY);
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
        if (!isPermitted || linkedStateMachine.currentState == PlayerStates.State.KnockBack)
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
        }
    }

    private void Jump()
    {
        linkedStateMachine.ChangeState(PlayerStates.State.Jump);
        linkedPhysicsControl.rb.linearVelocity = 
          new Vector2(airSpeed * linkedInput.horizontalInput, jumpForce);
        minimumAirTime = startMinimumAirTime;
    }

    private void StopJump(InputAction.CallbackContext value)
    {
        
    }

    public override void UpdateAnimator()
    {
        linkedAnimator.SetBool(jumpAnimationParameterName,
          linkedStateMachine.currentState == thisAbilityState || 
          linkedStateMachine.currentState == PlayerStates.State.WallJump);
        linkedAnimator.SetFloat(ySpeedAnimationParameterName, linkedPhysicsControl.rb.linearVelocityY);
    }
}
