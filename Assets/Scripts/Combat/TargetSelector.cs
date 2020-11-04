using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    public CombatSystem CombatSystem;

    private GameObject[] CurrentTargetedCombatants;
    private Ability CallingAbility;
    private TargetSchema CurrentTargetSchema;

    public void Start()
    {
        CombatSystem = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();
    }

    public void Target(Ability callingAbility)
    {
        CallingAbility = callingAbility;
        CurrentTargetSchema = callingAbility.TargetSchema;

        if (CurrentTargetSchema.SelectorType == SelectorType.All)
            TargetAllEnemies();

        if (CurrentTargetSchema.SelectorType == SelectorType.Number)
        {
            // TODO do something else, add rest of options
        }
    }

    private void TargetSingleEnemy()
    {
        // TODO
    }

    private void TargetAllEnemies()
    {
        GameObject[] enemies = CombatSystem.Combatants.
            Where(combantant => combantant.CompareTag("Enemy")).ToArray();

        // TODO highlight each enemy, wait for user selection
        Thread.Sleep(3000); // Fake the UI selection
        CurrentTargetedCombatants = enemies;
        
        // TODO for now, pretend the UI has triggered this function
        // this function will not be here afterward
        ReplyToCallingAbility();
    }

    private void ReplyToCallingAbility()
    {
        CallingAbility.SetTargetedCombatants(CurrentTargetedCombatants);
    }

}