using UnityEngine;


public enum CombatantType { Enemy, Ally };
public enum SelectorType { Self, All, Number };

public struct TargetSchema
{
    public int NumberOfTargets;
    public CombatantType CombatantType;
    public SelectorType SelectorType;

    public TargetSchema(int numberOfTargets, CombatantType combatantType, SelectorType selectorType)
    {
        NumberOfTargets = numberOfTargets;
        CombatantType = combatantType;
        SelectorType = selectorType;
    }
}

// TODO let's make this virtual: one for allies; one for enemies, cuz they're pretty different
public class TargetSelector : MonoBehaviour
{
    private GameObject[] TargetedCombatants;
    private Ability CallingAbility;

    public void Target(Ability callingAbility)
    {
        CallingAbility = callingAbility;
    }

    private void TargetSingleEnemy()
    {
        // TODO
    }

    private void TargetAllEnemies()
    {
        // TODO
    }

    private void ReplyToCallingAbility(Ability callingAbility)
    {
        // TODO
        callingAbility.SetTargetedCombatants(TargetedCombatants);
    }

}