using UnityEngine;

public class BaseAbility : MonoBehaviour
{
    protected Player player;
    protected GatherInput linkedInput;
    protected StateMachine linkedStateMachine;
    protected Animator linkedAnimator;
    protected PhysicsControl linkedPhysicsControl;

    public PlayerStates.State thisAbilityState;
    public bool isPermitted = true;
    [HideInInspector]
    public int stateParameterHsh;

    string stateAnimParameterName = "State";

    protected virtual void Start()
    {
        Initialization();
    }

    public virtual void EnterAbility() { }

    public virtual void ExitAbility() { }

    public virtual void ProccessAbility() { }

    public virtual void ProccessFixedAbility() { }

    public virtual void UpdateAnimator() { }

    public virtual void Initialization()
    {
        player = GetComponent<Player>();
        if (player != null)
        {
            linkedInput = player.gatherInput;
            linkedStateMachine = player.stateMachine;
            linkedAnimator = player.anim;
            linkedPhysicsControl = player.physicsControl;
            stateParameterHsh = Animator.StringToHash(stateAnimParameterName);
        }
    }
}
