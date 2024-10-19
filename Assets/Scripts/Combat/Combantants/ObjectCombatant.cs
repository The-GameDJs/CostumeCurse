using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Combat;
using UnityEngine;

// Used for objects that are part of combat that produce environmental reactions
// Ex: Logs that catch on fire
public abstract class ObjectCombatant : Combatant
{
    [SerializeField] protected int Damage;
    [SerializeField] protected ElementType ElementDamage;
    [SerializeField] protected AttackStyle AttackType;
    
    new void Start()
    {
        base.Start();
    }

    public override void EndTurn()
    {
        CombatSystem.EndTurn();
    }

    public override void Defend(Attack attack)
    {
        TakeDamage(attack.Damage, attack.Element, attack.Style);
    }

    protected override void TakeDamage(int damage, ElementType element, AttackStyle style)
    {
        if (ElementWeakness.Contains(element))
        {
            CurrentHealthPoints -= damage;
            RedBar.PlayAttackResultTextField("EXPLOSION", true);

            if (CurrentHealthPoints <= 0 && DeathAudioSource)
            {
                DeathAudioSource.Play();
            }
        }
        else
        {
            RedBar.PlayAttackResultTextField("Nothing", false);
        }
    }
    
    protected override void TakeTurnWhileDead()
    {
        EndTurn();
    }

    protected override void TakeTurnWhileAlive()
    {
        // It's an object, just end the turn
        EndTurn();
    }

    protected override void Die()
    {
        
    }

    protected abstract IEnumerator StartObjectReaction();
}
