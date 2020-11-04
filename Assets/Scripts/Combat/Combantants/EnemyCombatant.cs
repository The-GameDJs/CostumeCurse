using Assets.Scripts.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        if (displayUI)
        {
            Vector3 relativeScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
            enemyUI.transform.position = relativeScreenPosition;
        }
    }

    public void DisplayUI(bool displayUI)
    {
        this.displayUI = displayUI;
        enemyUITemplate.SetActive(displayUI);
    }


    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        // For now TODO
        TurnPriority = 10;

        enemyUI = Instantiate(enemyUITemplate);
        enemyUI.transform.parent = enemyPanel.transform;

        DisplayUI(true); // for now
    }

    public override void StartTurn()
    {
        // for now do nothing
        EndTurn();
    }

    public override void EndTurn()
    {
        // for now do nothing lmao
        CombatSystem.EndTurn(this.gameObject);
    }

    public override void Defend(Attack attack)
    {
        TakeDamage(attack.Damage);
    }
}
