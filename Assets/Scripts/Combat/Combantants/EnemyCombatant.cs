using Assets.Scripts.Combat;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCombatant : Combatant
{
    [SerializeField] private int CandyCornValue;

    private bool DisplayMessage;

    [SerializeField]
    private Ability[] Abilities;

    private CandyCornManager CandyCornManager;

    new void Start()
    {
        base.Start();
        Abilities = GetComponentsInChildren<Ability>();

        CandyCornManager = GameObject.FindObjectOfType<CandyCornManager>();
    }

    private new void Update()
    {
        base.Update();
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

        Attack attack = new Attack(100);

        GameObject targetedAlly = CombatSystem.Combatants
            .Where(combatant => combatant.CompareTag("Player"))
            .ToArray()[0];
        targetedAlly.GetComponent<AllyCombatant>().Defend(attack);

        EndTurn();
    }


    public override void EndTurn()
    {
        // for now do nothing lmao
        CombatSystem.EndTurn(this.gameObject);
    }

    public override void Defend(Attack attack)
    {
        Animator.Play("Base Layer.Hurt");

        TakeDamage(attack.Damage);
    }

    private void OnDestroy()
    {
        CandyCornManager.AddCandyCorn(CandyCornValue);
    }
}
