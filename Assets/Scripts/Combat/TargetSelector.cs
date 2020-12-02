using System;
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

    public void Target(Ability callingAbility, bool userTargeting = true)
    {
        CallingAbility = callingAbility;
        CurrentTargetSchema = callingAbility.TargetSchema;

        if (CurrentTargetSchema.SelectorType == SelectorType.All)
        {
            if (CurrentTargetSchema.CombatantType == CombatantType.Enemy)
                TargetAllEnemies(userTargeting);
            else if (CurrentTargetSchema.CombatantType == CombatantType.Ally)
                TargetAllAllies(userTargeting);
        }

        if (CurrentTargetSchema.SelectorType == SelectorType.Number)
        {
            // TODO do something else, add rest of options
            if (CurrentTargetSchema.CombatantType == CombatantType.Enemy)
                TargetAllEnemies(userTargeting);
            else if (CurrentTargetSchema.CombatantType == CombatantType.Ally)
                TargetAllAllies(userTargeting);
        }
    }

    private void TargetSingleEnemy(bool userTargeting = true)
    {
        // TODO
    }

    private void TargetAllEnemies(bool userTargeting = true)
    {
        GameObject[] enemies = CombatSystem.Combatants.
            Where(combatant => combatant.CompareTag("Enemy") && combatant.GetComponent<Combatant>().IsAlive).ToArray();

        if (userTargeting)
        {
            // TODO highlight each enemy, wait for user selection
            Thread.Sleep(500); // Fake the UI selection
        }

        CurrentTargetedCombatants = enemies;
        
        ReplyToCallingAbility();
    }

    private void TargetAllAllies(bool userTargeting = true)
    {
        GameObject[] allies = CombatSystem.Combatants.
          Where(combatant => combatant.CompareTag("Player") && combatant.GetComponent<Combatant>().IsAlive).ToArray();

        if (userTargeting)
        {
            // TODO highlight each enemy, wait for user selection
            Thread.Sleep(500); // Fake the UI selection
        }

        CurrentTargetedCombatants = allies;

        ReplyToCallingAbility();
    }

    private void ReplyToCallingAbility()
    {
        CallingAbility.SetTargetedCombatants(CurrentTargetedCombatants);
    }

}