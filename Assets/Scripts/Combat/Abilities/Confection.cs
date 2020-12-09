using Assets.Scripts.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Confection : Ability
{
    
    private enum Phase { Brew, Bake, Inactive }
    [SerializeField] private Canvas BrewCanvas;
    [SerializeField] private Canvas BakeCanvas;
    private Text TimerText;
    private static CanvasGroup BrewCanvasGroup;
    private static CanvasGroup BakeCanvasGroup;
    private ItemSlot CookingPot;
    private SliderHandle SliderScript;
    private Timer Timer;
    
    private int SweetsDropped;
    private int RotsDropped;
    private int GoodClicks;
    private int PerfectClicks;
    private int Clicks;
    private int MaxClicks = 3;
    private readonly float VisibleAlpha = 1.0f;
    private readonly float InvisibleAlpha = 0.0f;
    private readonly float BrewDuration = 5.0f;
    private readonly float BakeDuration = 10.0f;
    private readonly int BaseDamage = 20;
    private readonly int RottenDamage = -10;
    private readonly int SweetsDamage = 30;
    private readonly float BakePerfectDamageBonus = 1.15f;
    private readonly float BakeGoodDamageBonus = 1.05f;
    private int CurrentDamage;
    private Phase CookingAbilityPhase = Phase.Inactive;

    public new void Start()
    {
        base.Start();
        Timer = GetComponent<Timer>();
        
        StartUI();

        TargetSchema = new TargetSchema(
            0,
            CombatantType.Enemy,
            SelectorType.All);
    }

    private void StartUI()
    {
        StartBrewUI();
        StartBakeUI();
    }

    private void StartBrewUI()
    {
        CookingPot = BrewCanvas.GetComponentInChildren<ItemSlot>();
        TimerText = BrewCanvas.GetComponentInChildren<Text>();
        
        if (BrewCanvasGroup == null)
            BrewCanvasGroup = BrewCanvas.GetComponent<CanvasGroup>();
        if(BrewCanvasGroup != null)
            EnableCanvas(BrewCanvas, false); //this disables both the canvas and canvasgroup
    }

    private void StartBakeUI()
    {
        SliderScript = BakeCanvas.GetComponent<SliderHandle>();
        MaxClicks = SliderScript.GetMaxClicks();
        if (BakeCanvasGroup == null)
            BakeCanvasGroup = BakeCanvas.GetComponent<CanvasGroup>();
        if(BakeCanvasGroup != null)
            EnableCanvas(BakeCanvas, false); //this disables both the canvas and canvasgroup
        SliderScript.enabled = false;
    }

    private void EnableCanvas(Canvas canvas, bool enabled)
    {
        canvas.gameObject.SetActive(enabled);
        //canvas.enabled = enabled;
        EnableCanvasGroup(canvas, enabled);
    }

    private void EnableCanvasGroup(Canvas canvas, bool enabled)
    {
        if(enabled)
        {
            if(canvas.name == "BrewCanvas")
                BrewCanvasGroup.alpha = VisibleAlpha;
            else if(canvas.name == "BakeCanvas")
            {
                SliderScript.enabled = true;
                BakeCanvasGroup.alpha = VisibleAlpha;
            }
        }
        else
        {
            if(canvas.name == "BrewCanvas")
            {
                BrewCanvasGroup.alpha = InvisibleAlpha;
            }
            else if(canvas.name == "BakeCanvas")
            {
                SliderScript.enabled = false;
                BakeCanvasGroup.alpha = InvisibleAlpha;
            }
        }
    }

    public new void StartAbility(bool userTargeting = false)
    {
        base.StartAbility();
        Debug.Log("Started Confection Ability");
        EnableCanvas(BrewCanvas, true);
        Animator.SetBool("IsFinishedCasting", false);
        Animator.Play("Base Layer.Casting");

        CurrentDamage = 0;
    }

    private void Update()
    {
        if(CookingAbilityPhase == Phase.Brew)
            BrewUpdate();

        if (CookingAbilityPhase == Phase.Bake)
            BakeUpdate();
    }

    private void BrewUpdate()
    {
        if (Timer.IsInProgress())
        {
            float progress = BrewDuration - Timer.GetProgress();
            TimerText.text =  Mathf.RoundToInt(progress).ToString();
            SweetsDropped = CookingPot.GetSweetsDropped();
            RotsDropped = CookingPot.GetRotsDropped();
        }
        else
        {
            EndBrewPhase();
        }
    }
    
    private void BakeUpdate()
    {
        if (Timer.IsInProgress())
        {
            GoodClicks = SliderScript.GetGoodClicks();
            PerfectClicks = SliderScript.GetPerfectClicks();
            Clicks = SliderScript.GetTotalClicks();
            if(Clicks == MaxClicks) //TODO!!
                Timer.StopTimer();
        }
        else
        {
            EnableCanvas(BakeCanvas, false);
            CookingAbilityPhase = Phase.Inactive;
            CalculateBakeDamage();
            EndAbility();
        }
    }

    private void EndBrewPhase()
    {
        Debug.Log($"Brew Complete with candy: {SweetsDropped}");
        Debug.Log($"Brew Complete with rots: {RotsDropped}");

        CurrentDamage = (int) CalculateBrewDamage();
        ResetBrewComponents();
        EnableCanvas(BrewCanvas, false);

        Debug.Log($"Brewing Build Up Damage: {CurrentDamage}");

        Thread.Sleep(500);
        StartBakePhase();
    }

    protected override void EndAbility()
    {
        EnableCanvas(BrewCanvas, false);
        EnableCanvas(BakeCanvas, false);
        Debug.Log($"Confection Damage total: {CurrentDamage}");

        //Only deals damage to one enemy
        Attack attack = new Attack(CurrentDamage);
        TargetedCombatants[Random.Range(0, TargetedCombatants.Length)].GetComponent<Combatant>().Defend(attack);

        Animator.SetBool("IsFinishedCasting", true);

        CombatSystem.EndTurn(this.GetComponentInParent<Combatant>().gameObject);
    }

    protected override void ContinueAbilityAfterTargeting()
    {
        Debug.Log("Calling ContinueAbility()"); //TODO: This doesn't get called the second time around
        StartBrewPhase();
    }

    private void StartBrewPhase()
    {
        CookingAbilityPhase = Phase.Brew;
        SweetsDropped = 0;
        RotsDropped = 0;

        if(BrewCanvas != null)
        {
            //Move canvas to middle of the screen
            Vector3 relativeScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
            BrewCanvas.transform.position = relativeScreenPosition;
            EnableCanvas(BrewCanvas, true);
        }
        else
        { 
            Debug.Log("Canvas is Null in StartBrewPhase()"); 
        }

        Timer.StartTimer(BrewDuration);
    }

    private void StartBakePhase()
    {
        CookingAbilityPhase = Phase.Bake;
        SliderScript.enabled = true;
        Clicks = 0;

        if(BakeCanvas != null)
        {
            //Move canvas to middle of the screen
            Vector3 relativeScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
            BakeCanvas.transform.position = relativeScreenPosition;
            EnableCanvas(BakeCanvas, true);
            SliderScript.StartSlider();
        }
        else
        { 
            Debug.Log("Canvas is Null in StartBakePhase()"); 
        }

        Timer.StartTimer(BakeDuration);
    }

    private float CalculateBrewDamage()
    {
        CurrentDamage = (SweetsDropped * SweetsDamage) + (RotsDropped * RottenDamage);
        Mathf.Max(BaseDamage, CurrentDamage);

        Debug.Log("Damage after Brew " + CurrentDamage);
        return CurrentDamage;
    }

    private void CalculateBakeDamage()
    {
        if(GoodClicks > 0 || PerfectClicks > 0)
        {
            float temp = (GoodClicks * BakeGoodDamageBonus * CurrentDamage) + (PerfectClicks * BakePerfectDamageBonus * CurrentDamage);
            CurrentDamage = (int) temp;
        }

        Debug.Log("Damage after Bake " + CurrentDamage);
    }

    private void ResetBrewComponents()
    {
        DragAndDrop[] allChildren = BrewCanvas.GetComponentsInChildren<DragAndDrop>();
        foreach (DragAndDrop child in allChildren)
        {
            child.ResetPosition();
        }

        SweetsDropped = 0;
        RotsDropped = 0;
        CookingPot.ResetValues();
    }
}
