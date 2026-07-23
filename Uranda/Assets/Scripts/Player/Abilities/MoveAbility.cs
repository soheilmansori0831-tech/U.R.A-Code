using UnityEngine;

public class MoveAbility : BaseAbility
{
    [SerializeField] private float speed;

    public override void Initialization()
    {
        base.Initialization();
    }

    public override void ProccessAbility()
    {
        if (linkedPhysicsControl.grounded && linkedInput.horizontalInput == 0)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Idle);
        } else if (!linkedPhysicsControl.grounded)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Jump);
        }
    }

    public override void ProccessFixedAbility()
    {
        player.Flip();
        linkedPhysicsControl.rb.linearVelocity = 
          new Vector2(speed * linkedInput.horizontalInput, linkedPhysicsControl.rb.linearVelocity.y);
    }

    public override void UpdateAnimator()
    {
        linkedAnimator.SetInteger(stateParameterHsh, 1);
    }
}
