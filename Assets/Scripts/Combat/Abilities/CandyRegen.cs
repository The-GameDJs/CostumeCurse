using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CandyRegen : Ability
{
    [SerializeField] private int CandyGain;
    [SerializeField] private int TurnsToRegen;
    [SerializeField] private float CastTime;

    [Header("Candy Corn Vfx Object")]
    [SerializeField] private ParticleSystem CandyCornVfx;
    [SerializeField] private ParticleSystem CandySprinklesVfx;
    [SerializeField] private GameObject CandyMixObject;
    [SerializeField] private AudioSource CandyMixSound;
    
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

        Combatant.EndCandyRegenAbility += OnBattleEnd;
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
        CandyMixSound.Play();
        _timer.StartTimer(CastTime);
        Animator.SetBool("IsFinishedCasting", false);
        Animator.Play("Base Layer.Casting");

        CurrentPhase = CandyRegenPhase.Charging;

        if (_currentTurnCount == 0)
        {
            CurrentPhase = CandyRegenPhase.Charging;
            _allyCombatant.IsCharging = true;
            CandyMixObject.SetActive(true);
            CandyMixObject.transform.position = Combatant.transform.position + new Vector3(0.0f, 6.0f, 0.0f);
            CandyCornVfx.Play();
            CandySprinklesVfx.Play();
            CandyMixSound.Play();
        }
        else if (_currentTurnCount > TurnsToRegen)
        {
            CurrentPhase = CandyRegenPhase.Activated;
            StartCoroutine(DelayEndOfCandyRegenCycle());
            _allyCombatant.IsCharging = false;
        }
    }

    private IEnumerator DelayEndOfCandyRegenCycle()
    {
        StopCandyRegenVfx();

        yield return new WaitForSeconds(1.0f);
        
        EndCandyRegenCycle();
    }

    private void OnBattleEnd()
    {
        StopCandyRegenVfx();
        EndCandyRegenCycle();
    }

    private void EndCandyRegenCycle()
    {
        CandyMixObject.SetActive(false);
        CandyMixObject.transform.position = Vector3.zero;
        
        Debug.Log($"Charge complete! Adding some candy corn: {CandyGain}");
        CandyCornManager.AddCandyCorn(CandyGain);
        
        Animator.SetBool("IsFinishedCasting", true);
        Animator.Play("Base Layer.Idle");
        
        _timer.ResetTimer();
        CurrentPhase = CandyRegenPhase.Inactive;
        
        _currentTurnCount = 0;
    }

    private void StopCandyRegenVfx()
    {
        CandyCornVfx.Stop();
        CandySprinklesVfx.Stop();
    }

    protected override void EndAbility()
    {
        CurrentPhase = CandyRegenPhase.Inactive;
        
        if(_allyCombatant.gameObject.name == "Ganiel")
            Animator.SetBool("IsFinishedCasting", true);
        
        _currentTurnCount++;

        CombatSystem.EndTurn();
    }

    private void OnDestroy()
    {
        Combatant.EndCandyRegenAbility -= OnBattleEnd;
    }
}
