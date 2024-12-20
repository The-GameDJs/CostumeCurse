﻿using Assets.Scripts.Combat;
using System.Collections;
using System.Linq;
using UnityEngine;

public class EnemyCombatant : Combatant
{
    [SerializeField] private int CandyCornValue;

    private bool DisplayMessage;

    [SerializeField]
    protected Ability[] Abilities;

    [SerializeField] private GameObject Model;

    protected void Start()
    {
        base.Start();
        if (Abilities.Length == 0)
        {
            Abilities = GetComponentsInChildren<Ability>();
        }
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

    protected override void TakeTurnWhileAlive()
    {
        Debug.Log(Abilities.Length);
        Abilities[Random.Range(0, Abilities.Length)].StartAbility(false);
    }

    // Deprecated
    IEnumerator SimpleEnemyAttack()
    {
        yield return new WaitForSeconds(2);

        Attack attack = new Attack(100, ElementType.Normal, AttackStyle.Melee);

        GameObject targetedAlly = CombatSystem.Combatants
            .Where(combatant => combatant.CompareTag("Player"))
            .ToArray()[0];
        targetedAlly.GetComponent<AllyCombatant>().Defend(attack);

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

    public int GetCandyCornValue()
    {
        return CandyCornValue;
    }

    public void RotateModel()
    {
        if (Model)
        {
            Model.transform.Rotate(-90f, 0f, 0f);
        }
    }
}
