using Assets.Scripts.Combat;
using Combat.Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class Combatant : MonoBehaviour
{
    [SerializeField] public int TurnPriority;
    [SerializeField] private int MaxHealthPoints;
    private int CurrentHealthPoints;
    private int MaxShieldPoints;
    private int CurrentShieldPoints;
    protected CombatSystem CombatSystem;
    
    protected static GameObject HealthBarPrefab;
    protected static GameObject ShieldBarPrefab;
    protected static GameObject HealthBarUIPanel;
    protected static GameObject ShieldPrefab;
    
    private GameObject Shield;
    private GameObject HealthBar;
    private PointsBar RedBar;
    private TMP_Text HealthText;

    private bool IsShieldSpawned;
    private GameObject ShieldBar;
    private PointsBar BlueBar;
    private TMP_Text ShieldText;

    [SerializeField] private AudioSource HurtSound;

    private const float HealthBarYOffsetScale = 1.25f;
    private const float ShieldBarYOffsetScale = 1.45f;

    float CharacterHeight;

    public bool IsAlive = true;
    private bool IsInCombat;

    protected Animator Animator;

    public void Start()
    {
        CombatSystem = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();
        ShieldPrefab = GameObject.Find("MagicShieldPrefab");
        ShieldBarPrefab = GameObject.Find("ShieldBarPrefab");
        HealthBarPrefab = GameObject.Find("HealthBarPrefab");
        HealthBarUIPanel = GameObject.Find("HealthBarsUI");

        IsShieldSpawned = false;
        IsInCombat = false;

        if (GetComponent<CharacterController>() != null)
        {
            CharacterHeight = GetComponent<CharacterController>().height / 2.0f;
        }
        else
        {
            CharacterHeight = GetComponent<Collider>().bounds.size.y;
        }

        Animator = GetComponent<Animator>();

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
        if (CurrentHealthPoints < 0)
            CurrentHealthPoints = 0;
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
            ShieldText = ShieldBar.GetComponentInChildren<TMP_Text>();
            BlueBar.NewValue = CurrentHealthPoints;
            BlueBar.MaxValue = MaxShieldPoints;
            ShieldText.text = $"{CurrentShieldPoints} / {MaxShieldPoints}";
        }

    }

    private void UpdateHealthBarPosition()
    {
        Vector3 yOffset = new Vector3 (0, CharacterHeight * HealthBarYOffsetScale, 0);
        Vector3 offsetPos = gameObject.transform.position + yOffset;
        Vector3 relativeScreenPosition = Camera.main.WorldToScreenPoint(offsetPos);
        HealthBar.transform.position = relativeScreenPosition;
    }

    private void UpdateShieldBarPosition()
    {
        Vector3 yOffset = new Vector3 (0, CharacterHeight * ShieldBarYOffsetScale, 0);
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
        HurtSound.Play();

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
            Die();
    }

    private void Die()
    {
        IsAlive = false;
        Animator.Play("Base Layer.Death");
    }

    public void ApplyShield(int shieldHealth)
    {
        if (MaxShieldPoints == 0)
        {
            Shield.SetActive(true);
            Shield.transform.position = transform.position;
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
        DestroyUIInstances();
        IsInCombat = false;
    }

    public void EnterCombat()
    {
        CreateUIInstances();
        IsInCombat = true;
    }

    public void TurnToFaceInCombat(Transform other)
    {
        this.transform.LookAt(other, Vector3.up);
    }

    // Due to how the animator event system work, we have no choice but to broadcast this event down :( 
    public void DealBonkDamage()
    {
        GetComponentInChildren<Bonk>().DealBonkDamage();
    }

    public void OnDeathAnimationFinish()
    {

    }

    public void OnHurtFinish()
    {
        // Nothing for now....
    }

    public void OnCastFinish()
    {

    }

    public void OnShoot()
    {

    }

    public void CreateUIInstances()
    {
        if (gameObject.tag == "Player")
        {
            Shield = Instantiate(ShieldPrefab);
            Shield.name = gameObject.name + " Shield";
            Shield.transform.SetParent(GameObject.Find("CombatEffects").transform);
            Shield.SetActive(false);
        }

        HealthBar = Instantiate(HealthBarPrefab);
        HealthBar.transform.SetParent(HealthBarUIPanel.transform);
        RedBar = HealthBar.GetComponentInChildren<PointsBar>();
        HealthText = HealthBar.GetComponentInChildren<TMP_Text>();
        RedBar.MaxValue = MaxHealthPoints;

        HideBarsUI();
    }

    public void DestroyUIInstances()
    {
        Destroy(HealthBar);
        HealthBar = null;
        if (IsShieldSpawned)
        {
            Destroy(Shield);
            Shield = null;
            Destroy(ShieldBar);
            ShieldBar = null;
        }
    }

    public void HideBarsUI() 
    {
        HealthBarUIPanel.GetComponent<CanvasGroup>().alpha = 0f;
        HealthBarUIPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void ShowBarsUI() 
    {
        HealthBarUIPanel.GetComponent<CanvasGroup>().alpha = 1f;
        HealthBarUIPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
