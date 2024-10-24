﻿using Assets.Scripts.Combat;
using Combat.Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyCombatant : Combatant
{
    [SerializeField] Costume costume;

    public new void Start()
    {
        base.Start();

        costume.DisplayAbilities(false);
    }

    public new void Update()
    {
        base.Update();
    }

    public new void ExitCombat()
    {
        base.ExitCombat();
        GetComponentInChildren<Costume>().DisplayAbilities(false);
        Animator.Play("Base Layer.IdleWalk");
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
