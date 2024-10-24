using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Combat;
using UnityEngine;

public class WoodLogsCombatant : ObjectCombatant
{
    [SerializeField] private CapsuleCollider ExplosionHitbox;
    
    new void Start()
    {
        base.Start();
    }

    protected override void TakeDamage(int damage, ElementType element, AttackStyle style)
    {
        base.TakeDamage(damage, element, style);
        if (CurrentHealthPoints <= 0)
        {
            Die();
            ExplosionHitbox.isTrigger = true;
        }
    }

    protected override void Die()
    {
        StartCoroutine(StartObjectReaction());
    }

    protected override IEnumerator StartObjectReaction()
    {
        yield return new WaitForSeconds(1f);
        ExplosionHitbox.radius *= 30f;
        yield return new WaitForSeconds(0.2f);
        ExplosionParticles.transform.parent = null;
        ExplosionParticles.Play();
        gameObject.SetActive(false);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<EnemyCombatant>(out var enemy) && enemy.IsCombatantInCombat && enemy.IsAlive)
        {
            Attack attack = new Attack(Damage, ElementDamage, AttackType);
            enemy.Defend(attack);
            enemy.SetFireTimed();
        }
    }
}
