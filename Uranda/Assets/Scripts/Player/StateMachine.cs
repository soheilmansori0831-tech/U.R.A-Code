public class StateMachine
{
    public PlayerStates.State previousState;
    public PlayerStates.State currentState;
    public BaseAbility[] arrayOfAbilities;

    public void ChangeState(PlayerStates.State newState)
    {
        // we dont call any function if the ability is not permitted
        foreach (BaseAbility ability in arrayOfAbilities)
        {
            if (ability.thisAbilityState == newState)
            {
                if (!ability.isPermitted)
                {
                    return;
                }
            }
        }

        // call ExitAbility on the current ability
        foreach (BaseAbility ability in arrayOfAbilities)
        {
            if (ability.thisAbilityState == currentState)
            {
                ability.ExitAbility();
                previousState = currentState;
            }
        }

        // change the current ability and call EnterAbility on it
        foreach (BaseAbility ability in arrayOfAbilities)
        {
            if (ability.thisAbilityState == newState)
            {
                if (ability.isPermitted)
                {
                    currentState = newState;
                    ability.EnterAbility();
                }
                break;
            }
        }
    }

    public void ForceChange(PlayerStates.State newState)
    {
        previousState = currentState;
        currentState = newState;
    }
}
