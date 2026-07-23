using UnityEngine;

public class Player : MonoBehaviour
{
    public GatherInput gatherInput;
    public StateMachine stateMachine;
    public PhysicsControl physicsControl;
    public PlayerStats playerStats;
    public Animator anim;

    private BaseAbility[] playerAbilities;
    public bool facingRight = true;

    [Header("Current Weapon")]
    // public GameObject currentWeaponPrefab;
    // public ItemType currentWeaponItemType;

    [Header("Primary Weapon")]
    public GameObject primaryWeaponPrefab;

    public static Player instance;
    private void Awake()
    {
        stateMachine = new StateMachine();
        playerAbilities = GetComponents<BaseAbility>();
        stateMachine.arrayOfAbilities = playerAbilities;
        instance = this;
    }

    private void Update()
    {
        foreach (BaseAbility ability in playerAbilities)
        {
            if (ability.thisAbilityState == stateMachine.currentState)
            {
                ability.ProccessAbility();
            }
            ability.UpdateAnimator();
        }

        // print(stateMachine.currentState);
    }

    private void FixedUpdate()
    {
        foreach (BaseAbility ability in playerAbilities)
        {
            if (ability.thisAbilityState == stateMachine.currentState)
            {
                ability.ProccessFixedAbility();
            }
        }
    }

    public void Flip()
    {
        if (facingRight == true && gatherInput.horizontalInput < 0)
        {
            transform.Rotate(0, 180, 0);
            facingRight = !facingRight;
        } else if (facingRight == false && gatherInput.horizontalInput > 0)
        {
            transform.Rotate(0, 180, 0);
            facingRight = !facingRight;
        }
    }

    public void ForseFlip()
    {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
    }
}
