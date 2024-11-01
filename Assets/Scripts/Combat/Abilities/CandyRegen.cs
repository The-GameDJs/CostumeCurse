using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CandyRegen : Ability
{
    [SerializeField] private int CandyGain;
    [SerializeField] private int TurnsToRegen;
    [SerializeField] private float CastTime;
    
    private enum CandyRegenPhase
    {
        Inactive,
        Charging,
        Activated
    };

    private CandyRegenPhase CurrentPhase { get; set; }
    private int _currentTurnCount;
    private AllyCombatant _allyCombatant;
    private Timer _timer;
    
    public new void Start()
    {
        base.Start();

        _timer = GetComponent<Timer>();
        _allyCombatant = transform.GetComponentInParent<AllyCombatant>();
        
        TargetSchema = new TargetSchema(
            1,
            CombatantType.Ally,
            SelectorType.Self);
    }
    
    void Update()
    {
        if (CurrentPhase == CandyRegenPhase.Charging && _timer.IsFinished())
        {
            EndAbility();
        }
    }

    public new void StartAbility(bool userTargeting = false)
    {
        Debug.Log("Starting fireball ability");

        base.StartAbility();
    }

    protected override void ContinueAbilityAfterTargeting()
    {
        StartCharging();
    }

    private void StartCharging()
    {
        _timer.StartTimer(CastTime);
        Animator.SetBool("IsFinishedCasting", false);
        Animator.Play("Base Layer.Casting");

        if (_allyCombatant.IsCharging)
            CurrentPhase = CandyRegenPhase.Charging;

        if (CurrentPhase == CandyRegenPhase.Inactive && _currentTurnCount == 0)
        {
            CurrentPhase = CandyRegenPhase.Charging;
            _allyCombatant.IsCharging = true;
        }
        else if (CurrentPhase == CandyRegenPhase.Charging && _currentTurnCount > TurnsToRegen)
        {
            CurrentPhase = CandyRegenPhase.Activated;
            GrantCandyCorn();
            _allyCombatant.IsCharging = false;
        }
        else
        {
            CurrentPhase = CandyRegenPhase.Charging;
        }
    }

    private void GrantCandyCorn()
    {
        Debug.Log($"Charge complete! Adding some candy corn: {CandyGain}");
        CandyCornManager.AddCandyCorn(CandyGain);
        
        Animator.SetBool("IsFinishedCasting", true);
        Animator.Play("Base Layer.Idle");
        
        _timer.ResetTimer();
        CurrentPhase = CandyRegenPhase.Inactive;
        
        _currentTurnCount = 0;
    }

    protected override void EndAbility()
    {
        CurrentPhase = CandyRegenPhase.Inactive;
        
        if(_allyCombatant.gameObject.name == "Ganiel")
            Animator.SetBool("IsFinishedCasting", true);
        
        _currentTurnCount++;

        CombatSystem.EndTurn();
    }
}
