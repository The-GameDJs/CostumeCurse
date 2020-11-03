using Assets.Scripts.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyCombatant : Combatant
{
    [SerializeField] 
    Costume costume;

    void Start()
    {
        // set for now TODO
        TurnPriority = 0;
        costume.DisplayAbilities(false);
        //combatSystem = 
    }

    void Update()
    {

    }

    public override void StartTurn()
    {
        Debug.Log("Playing Turn");

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