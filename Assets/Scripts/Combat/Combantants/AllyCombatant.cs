using Assets.Scripts.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyCombatant : Combatant
{
    [SerializeField] 
    Costume costume;

    new void Start()
    {
        base.Start();
        // set for now TODO
        TurnPriority = 0;
        costume.DisplayAbilities(false);
        //combatSystem = 
    }

    new void Update()
    {
        base.Update();
    }

    public override void StartTurn()
    {
        costume.DisplayAbilities(true);
    }

    public override void EndTurn()
    {
        throw new System.NotImplementedException();
    }

    public override void Defend(Attack attack)
    {
        throw new System.NotImplementedException();
    }

    public void CancelTargeting()
    {
        // TODO
    }
}