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
        
        costume.DisplayAbilities(false);
    }

    new void Update()
    {
        base.Update();
    }

    new public void ExitCombat()
    {
        base.ExitCombat();
        GetComponentInChildren<Costume>().DisplayAbilities(false);
    }

    protected override void TakeTurnWhileDead()
    {
        // TODO add some dead idling animation? 

        EndTurn();
    }

    protected override void TakeTurnWhileAlive()
    {
        costume.DisplayAbilities(true);
    }

    public override void EndTurn()
    {
        throw new System.NotImplementedException();
    }

    public override void Defend(Attack attack)
    {
        TakeDamage(attack.Damage);
    }

    public void CancelTargeting()
    {
        // TODO
    }
}