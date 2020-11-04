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
    [SerializeField]
    public int CurrentHealthPoints;
    protected CombatSystem CombatSystem;
    [SerializeField]
    GameObject HealthBarPrefab;
    [SerializeField]
    GameObject HealthBarUIPanel;

    private GameObject HealthBar;
    private bool displayHealthBar;

    public bool isAlive = true;

    public void Start()
    {
        CombatSystem = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();

        HealthBar = Instantiate(HealthBarPrefab);
        HealthBar.transform.parent = HealthBarUIPanel.transform;
        displayHealthBar = true; // TODO show only when combat start, finish when it ends!
    }

    public void Update()
    {
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        Vector3 relativeScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        relativeScreenPosition.y += 125f;
        HealthBar.transform.position = relativeScreenPosition;
        HealthBar.GetComponentInChildren<Text>().text = $"{CurrentHealthPoints} / {MaxHealthPoints}";
    }

    public abstract void StartTurn();
    public abstract void EndTurn();

    public abstract void Defend(Attack attack);

    protected void TakeDamage(int damage)
    {
        CurrentHealthPoints -= damage;
    }

}
