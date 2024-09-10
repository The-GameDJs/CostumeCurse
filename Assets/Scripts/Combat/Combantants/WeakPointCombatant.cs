using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Combat;
using UnityEngine;

public abstract class WeakPointCombatant : EnemyCombatant
{
    protected bool HasWeakPointBeenHit;

    protected void Start()
    {
        base.Start();
    }

    protected void Update()
    {
        base.Update();
    }

    public new void ExitCombat()
    {
        base.ExitCombat();
        Animator.Play("Base Layer.Idle");
    }
    
    protected override void TakeTurnWhileDead()
    {
        // TODO add some dead idling animation? 

        EndTurn();
    }
    
    public override void EndTurn()
    {
        // for now do nothing lmao
        CombatSystem.EndTurn();
    }

    public override void Defend(Attack attack)
    {
        RotateModel();
        Animator.Play("Base Layer.Hurt");

        TakeDamage(attack.Damage, attack.Element, attack.Style);
    }
    
    public abstract void TriggerWeakPoint();
}
