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
    public int MaxShieldPoints;
    public int CurrentShieldPoints;
    private readonly float HealthBarPosition = 125f;
    protected CombatSystem CombatSystem;
    [SerializeField]
    GameObject HealthBarPrefab;
    [SerializeField]
    GameObject HealthBarUIPanel;
    [SerializeField]
    GameObject Shield;

    private GameObject HealthBar;

    public bool IsAlive = true;
    private bool IsInCombat;

    public void Start()
    {
        CombatSystem = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();
        Shield.SetActive(false);
        IsInCombat = false;

        HealthBar = Instantiate(HealthBarPrefab);
        HealthBar.transform.SetParent(HealthBarUIPanel.transform);
        HealthBar.SetActive(false);

        CurrentHealthPoints = MaxHealthPoints;
    }

    public void Update()
    {
        if(IsInCombat)
            UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        string healthText;

        if (CurrentShieldPoints > 0)
            healthText = IsAlive ?
                        $"{CurrentHealthPoints} / {MaxHealthPoints} Shield: {CurrentShieldPoints} / {MaxShieldPoints}" :
                        "I've fallen and can't get up";

        else
            healthText = IsAlive ?
                $"{CurrentHealthPoints} / {MaxHealthPoints}" :
                "I've fallen and can't get up";

        Vector3 relativeScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        relativeScreenPosition.y += HealthBarPosition;
        HealthBar.transform.position = relativeScreenPosition;
        HealthBar.GetComponentInChildren<Text>().text = healthText;
    }

    public void StartTurn()
    {
        if (IsAlive)
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
        if (CurrentShieldPoints > 0)
        {
            CurrentShieldPoints -= damage;
            if (CurrentShieldPoints <= 0) {
                Shield.SetActive(false);
                int leftoverDmg = -1 * CurrentShieldPoints;
                CurrentHealthPoints -= leftoverDmg;
            }
        }

        else
            CurrentHealthPoints -= damage;
        
        if (CurrentHealthPoints <= 0)
            IsAlive = false;
    }

    public void ApplyShield(int shieldHealth)
    {
        Shield.transform.position = GameObject.FindGameObjectWithTag("Player").transform.position;
        Shield.transform.position -= new Vector3(0, 5, 0);
        Shield.transform.Translate(new Vector3(0, 5, 0));
        Shield.SetActive(true);
        MaxShieldPoints = shieldHealth;
        CurrentShieldPoints = shieldHealth;
    }

    public void ExitCombat()
    {
        IsInCombat = false;
        HealthBar.SetActive(false);
    }

    public void EnterCombat()
    {
        IsInCombat = true;

        HealthBar.SetActive(true);
    }
}
