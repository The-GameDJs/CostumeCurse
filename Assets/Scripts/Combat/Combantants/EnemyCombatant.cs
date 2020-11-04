using Assets.Scripts.Combat;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCombatant : Combatant
{
    [SerializeField]
    private GameObject enemyPanel;
    [SerializeField]
    private GameObject enemyUITemplate;
    private GameObject enemyUI;

    private bool displayUI;

    private new void Update()
    {
        base.Update();

        UpdateUIPosition();
    }

    private void UpdateUIPosition()
    {
        if (displayUI && isAlive)
        {
            Vector3 relativeScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
            enemyUI.transform.position = relativeScreenPosition;
        }
    }

    private void DisplayUI()
    {
        displayUI = true;
        enemyUI.SetActive(displayUI);
    }

    private void HideUI()
    {
        displayUI = false;
        enemyUI.SetActive(displayUI);
    }


    new void Start()
    {
        base.Start();
        // For now TODO
        TurnPriority = 10;

        enemyUI = Instantiate(enemyUITemplate);
        enemyUI.transform.parent = enemyPanel.transform;

        HideUI();
    }

    protected override void TakeTurnWhileDead()
    {
        // TODO add some dead idling animation? 

        EndTurn();
    }

    protected override void TakeTurnWhileAlive()
    {
        DisplayUI();
        enemyUI.GetComponent<Text>().text = "I Attak! But actually i slep";

        StartCoroutine(SimpleEnemyAttack());
    }

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

        HideUI();
    }

    public override void Defend(Attack attack)
    {
        TakeDamage(attack.Damage);
    }
}
