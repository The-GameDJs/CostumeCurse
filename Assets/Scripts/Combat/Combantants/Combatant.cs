using Assets.Scripts.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Combatant : MonoBehaviour
{
    [SerializeField]
    public int TurnPriority;
    [SerializeField]
    public int MaxHealthPoints;
    public int CurrentHealthPoints;
    protected CombatSystem CombatSystem;
    [SerializeField]
    GameObject HealthBarPrefab;
    [SerializeField]
    GameObject HealthBarUIPanel;

    private GameObject HealthBar;

    public bool isAlive = true;
    private bool inCombat;

    public void Start()
    {
        inCombat = false;
        HealthBar = Instantiate(HealthBarPrefab);
        HealthBar.transform.parent = HealthBarUIPanel.transform;
        HealthBar.SetActive(false);
    }

    public void ShowHealth()
    {
        inCombat = true;
        CombatSystem = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();

        CurrentHealthPoints = MaxHealthPoints;
        HealthBar.SetActive(true);
    }

    public void Update()
    {
        if(inCombat)
            UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        string healthText = isAlive ?
            $"{CurrentHealthPoints} / {MaxHealthPoints}" :
            "I've fallen and can't get up";

        Vector3 relativeScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        relativeScreenPosition.y += 125f;
        HealthBar.transform.position = relativeScreenPosition;
        HealthBar.GetComponentInChildren<Text>().text = healthText;
    }

    public void StartTurn()
    {
        if (isAlive)
            TakeTurnWhileAlive();
        else
            TakeTurnWhileDead();
    }
    public abstract void EndTurn();

    public abstract void Defend(Attack attack);

    protected abstract void TakeTurnWhileDead();
    protected abstract void TakeTurnWhileAlive();

    protected void TakeDamage(int damage)
    {
        CurrentHealthPoints -= damage;
        
        if (CurrentHealthPoints < 0)
            isAlive = false;
    }

    public void RemoveHealthBars()
    {
        inCombat = false;
        HealthBar.SetActive(false);
    }

}
