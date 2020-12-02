using Assets.Scripts.Combat;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCombatant : Combatant
{
    private static GameObject EnemyUIPanel;
    private static GameObject EnemyUITemplate;
    private GameObject EnemyUI;

    private bool DisplayMessage;

    [SerializeField]
    private Ability[] Abilities;

    new void Start()
    {
        base.Start();
        if(EnemyUIPanel == null)
        {
            EnemyUIPanel = GameObject.Find("Enemy UI");
            EnemyUITemplate = GameObject.Find("DefaultEnemyText");
            EnemyUITemplate.SetActive(false);
        }
        
        EnemyUI = Instantiate(EnemyUITemplate);
        EnemyUI.transform.SetParent(EnemyUIPanel.transform);

        Abilities = GetComponentsInChildren<Ability>();

        HideUI();
    }

    private new void Update()
    {
        base.Update();

        UpdateUIPosition();
    }

    private void UpdateUIPosition()
    {
        if (DisplayMessage)
        {
            Vector3 relativeScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
            EnemyUI.transform.position = relativeScreenPosition;
        }
    }

    private void DisplayUI()
    {
        DisplayMessage = true;
        EnemyUI.SetActive(DisplayMessage);
    }

    private void HideUI()
    {
        DisplayMessage = false;
        EnemyUI.SetActive(DisplayMessage);
    }

    protected override void TakeTurnWhileDead()
    {
        // TODO add some dead idling animation? 

        EndTurn();
    }

    protected override void TakeTurnWhileAlive()
    {
        DisplayUI();
        EnemyUI.GetComponent<Text>().text = "I Attak!";

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

        HideUI();
    }

    public override void Defend(Attack attack)
    {
        TakeDamage(attack.Damage);
    }
}
