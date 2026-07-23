using UnityEngine;

public class IdleAbility : BaseAbility
{
    private string idleAnimParameterName = "Idle";
    private int idleParameterInt;

    public override void Initialization()
    {
        base.Initialization();
        idleParameterInt = Animator.StringToHash(idleAnimParameterName);
    }

    public override void EnterAbility()
    {
        base.EnterAbility();
        linkedPhysicsControl.rb.linearVelocityX = 0;
    }

    public override void ProccessAbility()
    {
        if (linkedInput.horizontalInput != 0)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Run);
        }

        if (linkedPhysicsControl.grounded == false)
        {
            if (linkedPhysicsControl.wallDetected)
                linkedStateMachine.ChangeState(PlayerStates.State.WallSlide);
            else
                linkedStateMachine.ChangeState(PlayerStates.State.Jump);
        }
    }

    public override void UpdateAnimator()
    {
        linkedAnimator.SetBool(idleAnimParameterName, linkedStateMachine.currentState == thisAbilityState 
        || linkedStateMachine.currentState == PlayerStates.State.Reload);
    }
}
