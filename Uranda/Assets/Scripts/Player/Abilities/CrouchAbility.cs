using UnityEngine;
using UnityEngine.InputSystem;

public class CrouchAbility : BaseAbility
{
    public InputActionReference crouchActionRef;
    [SerializeField] private float crouchSpeed;
    private bool holdingCrouch;
    
    public override void Initialization()
    {
        base.Initialization();
    }

    private void OnEnable()
    {
        crouchActionRef.action.performed += TryToCrouch;
        crouchActionRef.action.canceled += StopCrouch;
    }

    private void OnDisable()
    {
        crouchActionRef.action.performed -= TryToCrouch;
        crouchActionRef.action.canceled -= StopCrouch;
    }

    public override void ProccessAbility()
    {
        player.Flip();

        if (!linkedPhysicsControl.grounded)
            linkedStateMachine.ChangeState(PlayerStates.State.Jump);
    }

    public override void ProccessFixedAbility()
    {
        if (linkedPhysicsControl.grounded)
            linkedPhysicsControl.rb.linearVelocity = new Vector2(
                linkedInput.horizontalInput * crouchSpeed, linkedPhysicsControl.rb.linearVelocityY);

        if (holdingCrouch == false && linkedPhysicsControl.ceilingDetected == false)
        {
            if (linkedInput.horizontalInput == 0)
                linkedStateMachine.ChangeState(PlayerStates.State.Idle);
            else if (linkedInput.horizontalInput != 0)
                linkedStateMachine.ChangeState(PlayerStates.State.Run);
        }
    }

    public override void ExitAbility()
    {
        linkedPhysicsControl.StandColliders();
        holdingCrouch = false;
    }

    public override void EnterAbility()
    {
        linkedPhysicsControl.CrouchColliders();
    }

    private void TryToCrouch(InputAction.CallbackContext value)
    {
        if (!isPermitted) return;
        if (!linkedPhysicsControl.grounded || linkedStateMachine.currentState == PlayerStates.State.Dash
            || linkedStateMachine.currentState == PlayerStates.State.Ladders
            || linkedStateMachine.currentState == PlayerStates.State.KnockBack
            || linkedStateMachine.currentState == PlayerStates.State.Reload) 
            return;

        linkedStateMachine.ChangeState(PlayerStates.State.Crouch);
        holdingCrouch = true;
    }

    private void StopCrouch(InputAction.CallbackContext value)
    {
        holdingCrouch = false;
        if (!isPermitted) return;
        if (linkedStateMachine.currentState != PlayerStates.State.Crouch) return;
        if (linkedPhysicsControl.ceilingDetected) return;

        if (linkedInput.horizontalInput == 0)
            linkedStateMachine.ChangeState(PlayerStates.State.Idle);
        else if (linkedInput.horizontalInput != 0)
            linkedStateMachine.ChangeState(PlayerStates.State.Run);
    }

    public override void UpdateAnimator()
    {
        linkedAnimator.SetBool("Crouch", linkedStateMachine.currentState == thisAbilityState);
    }
}
