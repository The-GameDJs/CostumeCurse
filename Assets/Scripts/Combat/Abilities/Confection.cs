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
    [SerializeField] private Canvas BakeCanvas;
    private Text TimerText;
    private static CanvasGroup BrewCanvasGroup;
    private static CanvasGroup BakeCanvasGroup;
    private ItemsBeingDropped CookingPot;
    private SliderHandle SliderScript;
    private Timer Timer;

    [SerializeField] private AudioSource BrewSound;
    [SerializeField] private AudioSource BakeSound;
    [SerializeField] private AudioSource SweetCollectedSound;
    [SerializeField] private AudioSource RottenCollectedSound;

    private int SweetsDropped;
    private int RotsDropped;
    private int GoodClicks;
    private int PerfectClicks;
    private int Clicks;
    private int MaxClicks = 3;
    private readonly float VisibleAlpha = 1.0f;
    private readonly float InvisibleAlpha = 0.0f;
    private readonly float BrewDuration = 2.5f;
    private readonly float BakeDuration = 5.0f;
    private readonly int BaseDamage = 20;
    private readonly int RottenDamage = -10;
    private readonly int SweetsDamage = 10;
    private readonly float BakePerfectDamageBonus = 0.15f;
    private readonly float BakeGoodDamageBonus = 0.05f;
    private readonly int RandomDamageRangeOffset = 8;
    private int CurrentDamage;
    private Phase CookingAbilityPhase = Phase.Inactive;

    private GameObject Ganiel;
    private GameObject ConfectionMixObject;
    private ConfectionVfx ConfectionMixVfx;
    private GameObject Target;
    [SerializeField] private float ConfectionMixVfxVerticalOffset;
    [SerializeField] private DragAndDrop[] IngredientTypes;
    [SerializeField] private Transform[] IngredientSpawnLocations;
    private bool[] IsIngredientReserved;
    private readonly Stack<DragAndDrop> IngredientStack = new Stack<DragAndDrop>();

    public new void Start()
    {
        base.Start();
        Ganiel = GameObject.Find("Ganiel");
        ConfectionMixObject = GameObject.Find("ConfectionPowerMove");
        ConfectionMixVfx = ConfectionMixObject.GetComponent<ConfectionVfx>();
        Timer = GetComponent<Timer>();
        
        StartUI();

        IsIngredientReserved = new bool[IngredientSpawnLocations.Length];

        TargetSchema = new TargetSchema(
            1,
            CombatantType.Enemy,
            SelectorType.Number);
    }

    private void StartUI()
    {
        StartBrewUI();
        StartBakeUI();
    }

    private void StartBrewUI()
    {
        CookingPot = BrewCanvas.GetComponentInChildren<ItemsBeingDropped>();
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

            var newSweetCount = CookingPot.GetSweetsDropped();
            if (newSweetCount > SweetsDropped)
            {
                SweetCollectedSound.Play();
                SweetsDropped = newSweetCount;
            }

            var newRottenCount = CookingPot.GetRotsDropped();
            if (newRottenCount > RotsDropped)
            {
                RottenCollectedSound.Play();
                RotsDropped = newRottenCount;
            }
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
        else if (Timer.IsFinished())
        {
            StartCoroutine(EndBakeUpdate());
        }
    }

    private IEnumerator EndBakeUpdate()
    {
        Timer.ResetTimer();
        SliderScript.IsBaking = false;
        yield return new WaitForSeconds(1.0f);
        EnableCanvas(BakeCanvas, false);
        CookingAbilityPhase = Phase.Inactive;
        CalculateBakeDamage();

        //BakeSound.Play();

        EndAbility();
    }

    private void EndBrewPhase()
    {
        Debug.Log($"Brew Complete with candy: {SweetsDropped}");
        Debug.Log($"Brew Complete with rots: {RotsDropped}");

        BrewSound.Play();

        CalculateBrewDamage();
        ResetBrewComponents();
        EnableCanvas(BrewCanvas, false);

        Debug.Log($"Brewing Build Up Damage: {CurrentDamage}");

        Thread.Sleep(500);
        StartBakePhase();
    }

    public IEnumerator DealConfectionDamage()
    {
        //Only deals damage to one enemy
        Attack attack = new Attack(CalculateTotalDamage(), Element, Style);
        Target.GetComponent<Combatant>().Defend(attack);
        ConfectionMixVfx.ExplodeConfectionMix();

        yield return new WaitForSeconds(1.5f);

        ConfectionMixVfx.ResetVfx();
        CombatSystem.EndTurn();
    }

    protected override void EndAbility()
    {
        EnableCanvas(BrewCanvas, false);
        EnableCanvas(BakeCanvas, false);
        Debug.Log($"Confection Damage total: {CurrentDamage}");
        ConfectionMixVfx.SwitchConfectionMixParticleSystemsState();
        Target = TargetedCombatants[0];
        ConfectionMixVfx.SetTarget(Target);
        StartCoroutine(CastConfection());
    }

    public void ThrowConfectionCastingAtEnemy()
    {
        ConfectionMixVfx.StartMoving();
    }

    private IEnumerator CastConfection()
    {
        float animationTime = 0f;
        float animationDuration = 2.5f;
        Animator.SetBool("IsFinishedCasting", false);
        var offset = TargetedCombatants[0].GetComponent<Combatant>().isBoss
            ? ConfectionMixVfxVerticalOffset * 2
            : ConfectionMixVfxVerticalOffset;

        ConfectionMixObject.transform.position = Ganiel.transform.position + new Vector3(0.0f, ConfectionMixVfxVerticalOffset, 0.0f);
        CameraRigSystem.MoveCameraToSelectedTarget(Target, offset);

        while (animationTime < animationDuration)
        {
            animationTime += Time.deltaTime;
            Animator.Play("Base Layer.Casting");
            yield return null;
        }

        Animator.SetBool("IsFinishedCasting", true);
    }

    protected override void ContinueAbilityAfterTargeting()
    {
        Debug.Log("Calling ContinueAbility()");

        EnableCanvas(BrewCanvas, true);

        CurrentDamage = 0;

        StartBrewPhase();
    }

    private void StartBrewPhase()
    {
        CookingAbilityPhase = Phase.Brew;
        SweetsDropped = 0;
        RotsDropped = 0;

        if (BrewCanvas != null)
        {
            //Move canvas to middle of the screen
            BrewCanvas.transform.position = new Vector3(Screen.width/2f,Screen.height/2f,0f);
            EnableCanvas(BrewCanvas, true);
        }
        else
        { 
            Debug.Log("Canvas is Null in StartBrewPhase()"); 
        }
        
        RandomizeIngredientSpawns();

        Timer.StartTimer(BrewDuration);
    }

    private void RandomizeIngredientSpawns()
    {
        foreach (var ingredient in IngredientTypes)
        {
            IngredientStack.Push(ingredient);
        }

        while (IngredientStack.Count != 0)
        {
            int randomPosition = Random.Range(0, IngredientSpawnLocations.Length);

            if (!IsIngredientReserved[randomPosition])
            {
                var ingredient = IngredientStack.Pop();
                Debug.Log($"Ingredient Popped: {IngredientStack.Count} left");
                ingredient.gameObject.SetActive(true);

                ingredient.GetComponent<RectTransform>().position = 
                    IngredientSpawnLocations[randomPosition].GetComponent<RectTransform>().position;
                ingredient.InitializeStartingPosition();
                IsIngredientReserved[randomPosition] = true;
            }
        }
    }

    private void StartBakePhase()
    {
        CookingAbilityPhase = Phase.Bake;
        SliderScript.enabled = true;
        Clicks = 0;

        if(BakeCanvas != null)
        {
            //Move canvas to middle of the screen
            BakeCanvas.transform.position = new Vector3(Screen.width/2f,Screen.height/2f,0f);
            EnableCanvas(BakeCanvas, true);
            SliderScript.IsBaking = true;
            SliderScript.StartSlider();
        }
        else
        { 
            Debug.Log("Canvas is Null in StartBakePhase()"); 
        }

        Timer.StartTimer(BakeDuration);
    }

    private void CalculateBrewDamage()
    {
        var sweetsBaseDamage = SweetsDropped * SweetsDamage;
        var rotsBaseDamage = RotsDropped * RottenDamage;

        CurrentDamage = sweetsBaseDamage + rotsBaseDamage;
        
        if (CurrentDamage < BaseDamage)
            CurrentDamage = BaseDamage;

        Debug.Log("Damage after Brew " + CurrentDamage);
    }

    private void CalculateBakeDamage()
    {
        var bakeDamageMultiplier = 1f;
        bakeDamageMultiplier += GoodClicks * BakeGoodDamageBonus;
        bakeDamageMultiplier += PerfectClicks * BakePerfectDamageBonus;

        CurrentDamage = (int) (bakeDamageMultiplier * CurrentDamage);

        Debug.Log("Damage after Bake " + CurrentDamage);
    }

    private int CalculateTotalDamage()
    {
        CurrentDamage = Random.Range(CurrentDamage, CurrentDamage + RandomDamageRangeOffset);

        return CurrentDamage;
    }

    private void ResetBrewComponents()
    {
        foreach (var ingredient in IngredientTypes)
        {
            ingredient.ResetPosition();
            ingredient.gameObject.SetActive(false);
        }

        for (var i = 0; i < IngredientSpawnLocations.Length; ++i)
        {
            if (IsIngredientReserved[i])
            {
                IsIngredientReserved[i] = false;
            }
        }

        if (IngredientStack.Count > 0)
        {
            IngredientStack.Clear();
        }
        
        SweetsDropped = 0;
        RotsDropped = 0;
        CookingPot.ResetConfectionValues();
    }
}
