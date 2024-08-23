using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Combat;
using Combat.Abilities;
using Combat.Enemy_Abilities;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public abstract class Combatant : MonoBehaviour
{
    [SerializeField] public int TurnPriority;
    [SerializeField] private int MaxHealthPoints;

    [SerializeField] protected ElementType ElementResistance;
    [SerializeField] protected ElementType ElementWeakness;

    [SerializeField] private CombatantType Type;
    
    [SerializeField] private ParticleSystem[] FireBurns;

    public enum FireType
    {
        eOrangeFire,
        eRedFire,
        ePurpleFire
    }

    public enum CombatantType
    {
        Ground,
        Flying
    }
    
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
    private const float ShieldPositionVerticalOffset = 2.0f;

    float CharacterHeight;

    public bool IsAlive = true;
    private bool IsInCombat;

    [SerializeField] protected Animator Animator;
    [SerializeField] protected AudioSource DeathAudioSource;
    public bool isBoss;

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

        if (!Animator)
        {
            // For combatants that have their animator in their parent object
            Animator = GetComponent<Animator>();
            // For combatants that have it in their FBX model, attach reference to serialized object
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
            BlueBar.NewValue = CurrentShieldPoints;
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

    protected void TakeDamage(int damage, ElementType element, AttackStyle style)
    {
        // Attack doesn't hit the enemy due to the combatant type
        if (style == AttackStyle.Melee && Type == CombatantType.Flying)
        {
            Debug.Log($"Couldn't hit the enemy, the combatant type is resistant to that attack style!\n Attack Style: {style}, Combatant Type: {Type}");
            damage = 0;
        }

        Debug.Log($"Attack Element: {element}");
        if (element == ElementResistance)
        {
            Debug.Log($"Resisted some damage, since resistance is {ElementResistance}");
            damage /= 2;
        }
        else if (element == ElementWeakness)
        {
            Debug.Log($"Combatant took more damage, since weakness is {ElementWeakness}, it's super effective!");
            damage *= 2;
        }

        if (CurrentShieldPoints > 0)
        {
            CurrentShieldPoints -= damage;
            if (CurrentShieldPoints <= 0) {
                Shield.SetActive(false);
                MaxShieldPoints = 0;
                int leftoverDmg = -1 * CurrentShieldPoints;
                CurrentHealthPoints -= leftoverDmg;
                RedBar.PlayDamageTextField(damage);
            }
            BlueBar.PlayDamageTextField(damage);
        }

        else
        {
            CurrentHealthPoints -= damage;
            RedBar.PlayDamageTextField(damage);
        }

        if (CurrentHealthPoints <= 0)
            Die();

        if (CurrentHealthPoints <= 0 && isBoss && DeathAudioSource)
        {
            DeathAudioSource.Play();
            return;
        }
        HurtSound.Play();
    }

    private void Die()
    {
        IsAlive = false;
        if (isBoss)
        {
            CombatSystem.IsBossDead = isBoss;
            CombatSystem.EndTurn(gameObject);
            SetFire(true, FireType.ePurpleFire);
        }
        Animator.Play("Base Layer.Death");
    }

    public void ApplyShield(int shieldHealth, ElementType element)
    {
        ElementResistance = element;
        if (MaxShieldPoints == 0)
        {
            Shield.SetActive(true);
            Shield.transform.position = gameObject.transform.position + new Vector3(0.0f, ShieldPositionVerticalOffset, 0.0f);
            Shield.transform.SetParent(gameObject.transform);
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
        ResetPoints();
        IsInCombat = false;
        ElementResistance = ElementType.Normal;
        ElementWeakness = ElementType.Normal;
    }

    public void EnterCombat()
    {
        CreateUIInstances();
        IsInCombat = true;
    }

    public void TurnToFaceInCombat(Transform other)
    {
        var lookPos = other.position - gameObject.transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        gameObject.transform.rotation = rotation;

        //transform.LookAt(other, Vector3.up);
    }

    // Due to how the animator event system work, we have no choice but to broadcast this event down :( 
    public void DealBonkDamage()
    {
        GetComponentInChildren<Bonk>().DealBonkDamage();
    }

    public void DealConfectionDamage()
    {
        GetComponentInChildren<Confection>().ThrowConfectionCastingAtEnemy();
    }

    public void DealRevolverDamage()
    {
        GetComponentInChildren<Revolver>().ShootBulletFromRevolver();
    }

    public void DealSuperchargeDamage()
    {
        GetComponentInChildren<Supercharge>().ThrowChargeAtTarget();
    }

    public void DealMusicalNotesDamage()
    {
        GetComponentInChildren<Skelemusic>().ThrowMusicalNotesAtTarget();
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

    public void CreateUIInstances()
    {
        if (gameObject.CompareTag("Player"))
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
            IsShieldSpawned = false;
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

    public void ResetPoints()
    {
        IsAlive = true;
        CurrentHealthPoints = MaxHealthPoints;
        CurrentShieldPoints = 0;
        MaxShieldPoints = 0;
    }
    
    public void SetFire(bool isOnFire, FireType fire)
    {
        var flame = SelectFlame(fire);
        switch (isOnFire)
        {
            case true:
                flame.Play();
                break;
            default:
                flame.Stop();
                break;
        }
    }

    ParticleSystem SelectFlame(FireType fire)
    {
        return fire switch
        {
            // FireBurns are in alphabetical order, so do the same for the conditions!
            FireType.eOrangeFire => FireBurns[0],
            FireType.ePurpleFire => FireBurns[1],
            FireType.eRedFire => FireBurns[2],
            _ => throw new ArgumentOutOfRangeException(nameof(fire), fire, null)
        };
    }
}
