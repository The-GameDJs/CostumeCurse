using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Combat;
using UnityEngine;

public class BulkerCombatant : WeakPointCombatant
{
    [SerializeField] private GameObject ShieldBulker;
    
    new void Start()
    {
        base.Start();
    }

    new void Update()
    {
        base.Update();
    }

    public override void TriggerWeakState()
    {
        // Remove shield
        ShieldBulker.SetActive(false);
    }

    protected override void TakeDamage(int damage, ElementType element, AttackStyle style)
    {
        if (HasWeakPointBeenHit)
        {
            base.TakeDamage(damage, element, style);
            return;
        }

        if (ElementWeakness.Contains(element) && !HasWeakPointBeenHit)
        {
            HasWeakPointBeenHit = true;
            TakeWeakpointDamage("Shield Burned!", HasWeakPointBeenHit);
            TriggerWeakState();
        }
        else
        {
            Animator.Play("Block");
            TakeWeakpointDamage("Blocked", HasWeakPointBeenHit);
        }
    }

    public override IEnumerator ResetWeakState()
    {
        throw new System.NotImplementedException();
    }
}
