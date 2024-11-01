using System;
using Assets.Scripts.Combat;
using Combat.Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AllyCombatant : Combatant
{
    [SerializeField] Costume costume;
    [SerializeField] public Collider HurtCollider;
    
    [SerializeField] public Collider ParryCollider;
    [SerializeField] public AudioSource ParrySound;
    [SerializeField] private ParryVfx ParryEffect;
    [SerializeField] public Transform ParryVfxPlacement;

    public bool HasParried;
    public bool HasParriedCorrectly;

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
        if(IsCharging)
            EndTurn();
        
        costume.DisplayAbilities(true);
    }

    public override void EndTurn()
    {
        CombatSystem.EndTurn();
    }

    public override void Defend(Attack attack)
    {
        Animator.Play("Base Layer.Hurt");

        var animationTime = Animator.GetCurrentAnimatorStateInfo(0).length;

        var damage = HasParriedCorrectly ? attack.Damage / 2 : attack.Damage;
        
        if(HasParriedCorrectly)
            ParrySound.Play();
        
        TakeDamage(damage, attack.Element, attack.Style);

        HasParriedCorrectly = false;
        HasParried = false;
    }

    public void PlayParticleVfx()
    {
        ParryEffect.ActivateVfx(this);
    }
}
