using System;
using Assets.Scripts.Combat;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Confection : Ability
{
    
    private enum Phase { Brew, Bake, Inactive }

    [SerializeField] private Canvas BrewCanvas;
    private Text TimerText;
    private SliderHandle SliderScript;
    private static CanvasGroup BrewCanvasGroup;
    private static CanvasGroup BakeCanvasGroup;
    private Timer Timer;
    private Queue<InputManager.Direction> InputOrder;

    [SerializeField] private AudioSource BrewSound;
    [SerializeField] private AudioSource BakeSound;
    [SerializeField] private AudioSource SweetCollectedSound;
    [SerializeField] private AudioSource RottenCollectedSound;

    private int GoodClicks;
    private int MissedTime;
    private int PerfectClicks;
    private int Clicks;
    private int MaxClicks = 6;
    
    private readonly float BrewDuration = 8.0f;
    private readonly int BaseDamage = 40;
    private readonly float BakePerfectDamageBonus = 4.35f;
    private readonly float BakeGoodDamageBonus = 0.08f;
    private readonly int RandomDamageRangeOffset = 8;
    private int CurrentDamage;
    private Phase CookingAbilityPhase = Phase.Inactive;

    private GameObject Ganiel;
    private GameObject ConfectionMixObject;
    private ConfectionVfx ConfectionMixVfx;
    private GameObject Target;
    [SerializeField] private float ConfectionMixVfxVerticalOffset;
    private bool[] IsIngredientReserved;

    public static Action<int> ShowConfectionParticle;

    public new void Start()
    {
        base.Start();
        Ganiel = GameObject.Find("Ganiel");
        ConfectionMixObject = GameObject.Find("ConfectionPowerMove");
        ConfectionMixVfx = ConfectionMixObject.GetComponent<ConfectionVfx>();
        SliderScript = BrewCanvas.GetComponent<SliderHandle>();
        Timer = GetComponent<Timer>();
        InputOrder = new Queue<InputManager.Direction>();
        InputOrder.Enqueue(InputManager.Direction.Left);
        InputOrder.Enqueue(InputManager.Direction.Right);
        StartUI();

        TargetSchema = new TargetSchema(
            1,
            CombatantType.Enemy,
            SelectorType.Number);
    }

    private void StartUI()
    {
        StartBrewUI();
    }

    private void StartBrewUI()
    {
        // TimerText = BrewCanvas.GetComponentInChildren<Text>();
    }

    public new void StartAbility(bool userTargeting = false)
    {
        base.StartAbility();
        Debug.Log("Started Confection Ability");
    }

    protected override void ContinueAbilityAfterTargeting()
    {
        Debug.Log("Calling ContinueAbility()");

        CurrentDamage = 0;

        StartBrewPhase();
    }

    private void StartBrewPhase()
    {
        CookingAbilityPhase = Phase.Brew;
        
        BrewCanvas.transform.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        SliderScript.enabled = true;
        InputUIManager.Instance.SetRotatingInputUIButton(SliderScript.InputUIPosition.transform.position, true, "Right");

        if (BrewCanvas != null)
        {
            BrewCanvas.enabled = true;
        }
        else
        { 
            Debug.Log("Canvas is Null in StartBrewPhase()"); 
        }

        SliderScript.IsBaking = true;
        ConfectionMixVfx.ActivateVfx();
        Timer.StartTimer(BrewDuration);
        StartCoroutine(PlayPartOfCastingAnimation());
    }

    private IEnumerator PlayPartOfCastingAnimation()
    {
        ConfectionMixObject.transform.position = Ganiel.transform.position + new Vector3(0.0f, ConfectionMixVfxVerticalOffset, 0.0f);
        Animator.Play("Base Layer.Casting");
        yield return new WaitForSeconds(0.5f);
        Animator.speed = 0f;
    }

    private void Update()
    {
        if(CookingAbilityPhase == Phase.Brew)
            BrewUpdate();
    }

    private void BrewUpdate()
    {
        if (Timer.IsInProgress())
        {
            // Uncomment if we ever want to add back the timer
            // float progress = BrewDuration - Timer.GetProgress();
            // TimerText.text =  Mathf.RoundToInt(progress).ToString();
            
            GoodClicks = SliderScript.GetGoodClicks();
            MissedTime = SliderScript.GetMissedTime();
            if (PerfectClicks != SliderScript.GetPerfectClicks())
            {
                PerfectClicks = SliderScript.GetPerfectClicks();
                ShowConfectionParticle?.Invoke(PerfectClicks);
            }
            Clicks = SliderScript.GetTotalClicks();
            if(Clicks == MaxClicks)
                Timer.StopTimer();
        }
        else
        {
            SliderScript.IsBaking = false;
            EndBrewPhase();
        }
    }

    private void EndBrewPhase()
    {
        Debug.Log($"Brew Complete! Perfect Clicks: {PerfectClicks}. Good Clicks: {GoodClicks}. Missed Clicks: {MissedTime}");

        CookingAbilityPhase = Phase.Inactive;
        
        BrewSound.Play();

        InputUIManager.Instance.SetRotatingInputUIButton(SliderScript.InputUIPosition.transform.position, false);

        StartCoroutine(EndBrewUpdate());
    }

    private IEnumerator EndBrewUpdate()
    {
        Timer.ResetTimer();
        yield return new WaitForSeconds(1.0f);

        BrewCanvas.enabled = false;
        SliderScript.enabled = false;
        EndAbility();
    }

    public IEnumerator DealConfectionDamage()
    {
        Attack attack = new Attack(CalculateTotalDamage(), Element, Style);
        Target.GetComponent<Combatant>().Defend(attack);
        //ConfectionMixVfx.ExplodeConfectionMix();

        yield return new WaitForSeconds(1.5f);
        ConfectionMixVfx.TurnOffConfectionParticles();
        ConfectionMixVfx.ResetVfx();
        ResetBrewComponents();
        Animator.SetBool("IsFinishedCasting", false);
        CombatSystem.EndTurn();
    }

    private int CalculateTotalDamage()
    {
        var p = PerfectClicks;
        var g = GoodClicks;
        var m = MissedTime;
        var b = BaseDamage;
        
        var bP = BakePerfectDamageBonus * p;
        var bG = (BakeGoodDamageBonus * g) / 2;
        var bM = (int) (m / BrewDuration);
        
        int total = (int) (bP + b + bG) - bM;

        CurrentDamage = Random.Range(total, total + RandomDamageRangeOffset);
        Debug.Log($"Confection Damage total: {CurrentDamage}");

        return CurrentDamage;
    }

    private IEnumerator CastConfection()
    {
        float animationTime = 0f;
        float animationDuration = 2f;
        Animator.SetBool("IsFinishedCasting", false);
        
        CinemachineCameraRig.Instance.SetCinemachineCameraTarget(Target.transform);
        Animator.speed = 1f;
        while (animationTime < animationDuration)
        {
            animationTime += Time.deltaTime;
            Animator.Play("Base Layer.Casting");
            yield return null;
        }

        Animator.SetBool("IsFinishedCasting", true);
    }

    public void ThrowConfectionCastingAtEnemy()
    {
        ConfectionMixVfx.StartMoving();
    }

    protected override void EndAbility()
    {
        Target = TargetedCombatants[0];
        ConfectionMixVfx.SetTarget(Target);
        StartCoroutine(CastConfection());
    }

    private void ResetBrewComponents()
    {
        PerfectClicks = 0;
        GoodClicks = 0;
        MissedTime = 0;
        CurrentDamage = 0;
    }
}
