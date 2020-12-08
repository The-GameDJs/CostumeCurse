using Assets.Scripts.Combat;
using Combat.Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Combatant : MonoBehaviour
{
    [SerializeField] public int TurnPriority;
    [SerializeField] private int MaxHealthPoints;
    private int CurrentHealthPoints;
    public int MaxShieldPoints;
    private int CurrentShieldPoints;
    protected CombatSystem CombatSystem;
    
    protected static GameObject HealthBarPrefab;
    protected static GameObject ShieldBarPrefab;
    protected static GameObject HealthBarUIPanel;
    
    private static GameObject Shield;
    private GameObject HealthBar;
    private PointsBar RedBar;
    private Text HealthText;

    private bool IsShieldSpawned;
    private GameObject ShieldBar;
    private PointsBar BlueBar;
    private Text ShieldText;

    private const float HealthBarYOffsetScaler = 1.25f;
    private const float ShieldBarYOffsetScaler = 1.45f;

    float CharacterHeight;

    public bool IsAlive = true;
    private bool IsInCombat;

    public void Start()
    {
        CombatSystem = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();
        if (Shield == null)
        {
            Shield = GameObject.Find("Magic Shield");
            HealthBarPrefab = GameObject.Find("HealthBarPrefab");
            HealthBarUIPanel = GameObject.Find("HealthBarsUI");
        }
        Shield.SetActive(false);
        IsShieldSpawned = false;
        IsInCombat = false;

        HealthBar = Instantiate(HealthBarPrefab);
        HealthBar.transform.SetParent(HealthBarUIPanel.transform);
        RedBar = HealthBar.GetComponentInChildren<PointsBar>();
        HealthText = HealthBar.GetComponentInChildren<Text>();
        RedBar.MaxValue = MaxHealthPoints;
        HealthBar.SetActive(false);
        HealthBarPrefab.SetActive(false);

        if (GetComponent<CharacterController>() != null)
        {
            CharacterHeight = GetComponent<CharacterController>().height / 2.0f;
        }
        else
        {
            CharacterHeight = GetComponent<Collider>().bounds.size.y;
        }
        
        CurrentHealthPoints = MaxHealthPoints;
    }

    public void Update()
    {
        if(IsInCombat)
        {
            UpdateHealthBar();
            UpdateShieldBar();
        }
    }

    private void UpdateHealthBar()
    {
        RedBar.NewValue = CurrentHealthPoints;
        HealthText.text = $"{CurrentHealthPoints} / {MaxHealthPoints}";
        
        UpdateHealthBarPosition();
    }

    private void UpdateShieldBar()
    {
        if (IsShieldSpawned && CurrentShieldPoints > 0)
        {
            ShieldBar.SetActive(true);
            BlueBar.NewValue = CurrentShieldPoints;
            ShieldText.text = $"{CurrentShieldPoints} / {MaxShieldPoints}";
        
            UpdateShieldBarPosition();
        }
        else if (CurrentShieldPoints <= 0 && IsShieldSpawned)
        {
            ShieldBar.SetActive(false);
        }
        // Only instantiates a shield if ability is called
        else if (CurrentShieldPoints > 0)
        {
            IsShieldSpawned = true;
            ShieldBar = Instantiate(ShieldBarPrefab);
            ShieldBar.transform.SetParent(HealthBarUIPanel.transform);
            BlueBar = ShieldBar.GetComponentInChildren<PointsBar>();
            ShieldText = ShieldBar.GetComponentInChildren<Text>();
            BlueBar.NewValue = CurrentHealthPoints;
            BlueBar.MaxValue = MaxShieldPoints;
            ShieldText.text = $"{CurrentShieldPoints} / {MaxShieldPoints}";
        }

    }

    private void UpdateHealthBarPosition()
    {
        Vector3 yOffset = new Vector3 (0, CharacterHeight * HealthBarYOffsetScaler, 0);
        Vector3 offsetPos = gameObject.transform.position + yOffset;
        Vector3 relativeScreenPosition = Camera.main.WorldToScreenPoint(offsetPos);
        HealthBar.transform.position = relativeScreenPosition;
    }

    private void UpdateShieldBarPosition()
    {
        Vector3 yOffset = new Vector3 (0, CharacterHeight * ShieldBarYOffsetScaler, 0);
        Vector3 offsetPos = gameObject.transform.position + yOffset;
        Vector3 relativeScreenPosition = Camera.main.WorldToScreenPoint(offsetPos);
        ShieldBar.transform.position = relativeScreenPosition;
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
                MaxShieldPoints = 0;
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
        if (MaxShieldPoints == 0)
        {
            Shield.transform.position = GameObject.FindGameObjectWithTag("Player").transform.position;
            Shield.SetActive(true);
            MaxShieldPoints = shieldHealth;
            CurrentShieldPoints = shieldHealth;
        }

        // If the Shield is already up (and its a worse sequence, but max shield health is higher), add to the current shield health. 
        // If they end up getting a better sequence, replace the max shield health and add that health to the current shield health
        else
        {
            if (shieldHealth > MaxShieldPoints)
                MaxShieldPoints = shieldHealth;

            CurrentShieldPoints += shieldHealth;
            if (CurrentShieldPoints > MaxShieldPoints)
                CurrentShieldPoints = MaxShieldPoints;
        }
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

    // Due to how the animator event system work, we have no choice but to broadcast this event down :( 
    public void DealBonkDamage()
    {
        GetComponentInChildren<Bonk>().DealBonkDamage();
    }
}
