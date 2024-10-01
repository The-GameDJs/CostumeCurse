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
    [SerializeField] private Canvas BakeCanvas;
    private Text TimerText;
    private static CanvasGroup BrewCanvasGroup;
    private static CanvasGroup BakeCanvasGroup;
    private Timer Timer;
    private Queue<InputManager.Direction> InputOrder;

    [SerializeField] private AudioSource BrewSound;
    [SerializeField] private AudioSource BakeSound;
    [SerializeField] private AudioSource SweetCollectedSound;
    [SerializeField] private AudioSource RottenCollectedSound;

    private int TotalCorrectInput;
    private readonly float BrewDuration = 4.5f;
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
    private bool[] IsIngredientReserved;

    public static Action<int> CastConfectionAction;

    public new void Start()
    {
        base.Start();
        Ganiel = GameObject.Find("Ganiel");
        ConfectionMixObject = GameObject.Find("ConfectionPowerMove");
        ConfectionMixVfx = ConfectionMixObject.GetComponent<ConfectionVfx>();
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
        TimerText = BrewCanvas.GetComponentInChildren<Text>();
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
        TotalCorrectInput = 0;
        
        BrewCanvas.transform.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        if (BrewCanvas != null)
        {
            BrewCanvas.enabled = true;
        }
        else
        { 
            Debug.Log("Canvas is Null in StartBrewPhase()"); 
        }
        
        Timer.StartTimer(BrewDuration);
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
            float progress = BrewDuration - Timer.GetProgress();
            TimerText.text =  Mathf.RoundToInt(progress).ToString();
        }
        else
        {
            EndBrewPhase();
        }
    }

    private void EndBrewPhase()
    {
        Debug.Log($"Brew Complete with candy: {TotalCorrectInput}");

        CookingAbilityPhase = Phase.Inactive;
        
        BrewSound.Play();

        CastConfectionAction?.Invoke(TotalCorrectInput);
        
        ResetBrewComponents();

        StartCoroutine(EndBrewUpdate());
    }

    private IEnumerator EndBrewUpdate()
    {
        Timer.ResetTimer();
        yield return new WaitForSeconds(1.0f);

        BrewCanvas.enabled = false;
        EndAbility();
    }

    private void OnJoystickTapped(Vector2 input)
    {
        VerifyConfectionInput(input);
    }

    private void VerifyConfectionInput(Vector2 input)
    {
        if (CookingAbilityPhase != Phase.Brew || input == Vector2.zero)
            return;
        
        var direction = InputOrder.Dequeue();

        switch (direction)
        {
            case InputManager.Direction.Left:
                if (input.x <= -0.1f)
                {
                    TotalCorrectInput++;
                }
                else
                {
                    TotalCorrectInput--;
                }
                break;
            case InputManager.Direction.Right:
                if (input.x >= 0.1f)
                {
                    TotalCorrectInput++;
                }
                else
                {
                    TotalCorrectInput--;
                }
                break;
        }

        if (TotalCorrectInput < 0)
            TotalCorrectInput = 0;
        
        Debug.Log($"Total Correct Input {TotalCorrectInput}");
        InputOrder.Enqueue(direction);
    }

    public IEnumerator DealConfectionDamage()
    {
        Attack attack = new Attack(CalculateTotalDamage(), Element, Style);
        Target.GetComponent<Combatant>().Defend(attack);
        ConfectionMixVfx.ExplodeConfectionMix();

        yield return new WaitForSeconds(1.5f);

        ConfectionMixVfx.ResetVfx();
        CombatSystem.EndTurn();
    }

    private int CalculateTotalDamage()
    {
        int total = (int) (TotalCorrectInput * BakePerfectDamageBonus + BaseDamage);

        // Yeah this needs work lmao
        total = InputManager.CurrentControlScheme == "Controller" ? total * total * total : total;
        
        CurrentDamage = Random.Range(total, total + RandomDamageRangeOffset);

        return CurrentDamage;
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
        CinemachineCameraRig.Instance.SetCinemachineCameraTarget(Target.transform);

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
        Debug.Log($"Confection Damage total: {CurrentDamage}");
        ConfectionMixVfx.SwitchConfectionMixParticleSystemsState();
        Target = TargetedCombatants[0];
        ConfectionMixVfx.SetTarget(Target);
        StartCoroutine(CastConfection());
    }

    private void ResetBrewComponents()
    {
        TotalCorrectInput = 0;
    }

    private void OnEnable()
    {
        InputManager.JoystickTapped += OnJoystickTapped;
    }

    private void OnDestroy()
    {
        InputManager.JoystickTapped -= OnJoystickTapped;
    }
}
