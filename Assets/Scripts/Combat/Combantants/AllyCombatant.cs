using Assets.Scripts.Combat;
using Combat.Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyCombatant : Combatant
{
    [SerializeField] Costume costume;

    new public void Start()
    {
        base.Start();

        costume.DisplayAbilities(false);
    }

    new public void Update()
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
        CombatSystem.EndTurn(this.gameObject);
    }

    public override void Defend(Attack attack)
    {
        Animator.Play("Base Layer.Hurt");

        TakeDamage(attack.Damage);
    }
}
