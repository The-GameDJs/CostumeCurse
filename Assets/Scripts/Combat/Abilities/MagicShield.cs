using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MagicShield : Ability
{
    private enum Button { Up, Down, Left, Right };
    private enum Phase { SequencePhase, InputSequence, Inactive };
    private Timer Timer;
    private List<Button> Sequence = new List<Button>();
    private Queue<GameObject> Directions = new Queue<GameObject>();

    [SerializeField] private Canvas MagicShieldCanvas;
    [SerializeField] private GameObject SequenceArrowsGroup;
    [SerializeField] private GameObject InputSequenceGroup;
    [SerializeField] private GameObject[] Arrows; // 0: Up, 1: Down, 2: Left, 3: Right 
    [SerializeField] private AudioSource MagicShieldSound;

    private Vector3 InitialCanvasPosition;
    private readonly float SequenceDuration = 2f;
    private readonly float InputDuration = 2.5f;
    private readonly float ArrowPositionHeight = 8f;
    private readonly float NextArrowPositionOffsetX = 4f;
    private readonly int MaxButtonsInSequence = 4;
    private Phase CurrentPhase = Phase.Inactive;

    private readonly float MinMagicShieldHealth = 20f;
    private readonly float ShieldSlope = 20f;
    private int CorrectInputs = 0;
    private int ArrowsMoved = 0;
    private int MagicShieldHealth;

    public new void Start()
    {
        base.Start();
        Timer = GetComponent<Timer>();
        MagicShieldCanvas.gameObject.SetActive(false);
        InitialCanvasPosition = MagicShieldCanvas.transform.position;
        SetCanvas(false);
        SequenceArrowsGroup.SetActive(false);

        TargetSchema = new TargetSchema(
            0,
            CombatantType.Ally,
            SelectorType.All);
    }

    private void SetCanvas(bool isActive)
    {
        MagicShieldCanvas.gameObject.SetActive(isActive);
        MagicShieldCanvas.GetComponent<CanvasGroup>().alpha = isActive ? 1.0f : 0.0f;
        MagicShieldCanvas.transform.position = isActive ? new Vector3(Screen.width/2f,Screen.height/2f,0f) : InitialCanvasPosition;
    }

    public new void StartAbility(bool userTargeting = false)
    {
        CorrectInputs = 0;
        Animator.SetBool("IsFinishedCasting", false);
        Animator.Play("Base Layer.Casting");

        base.StartAbility();
    }

    private void Update()
    {
        if(CurrentPhase == Phase.SequencePhase)
            SequencePhaseUpdate();

        if(CurrentPhase == Phase.InputSequence)
            InputSequenceUpdate();
    }

    protected override void ContinueAbilityAfterTargeting()
    {
        StartMagicShieldSequence();
    }

    protected override void EndAbility()
    {
        Debug.Log($"Magic Shield Total Health: {MagicShieldHealth}");

        SetCanvas(false);
        MagicShieldSound.Play();

        foreach (GameObject target in TargetedCombatants)
            target.GetComponent<Combatant>().ApplyShield(MagicShieldHealth);

        Animator.SetBool("IsFinishedCasting", true);

        CombatSystem.EndTurn(this.GetComponentInParent<Combatant>().gameObject);

    }

    private void StartMagicShieldSequence()
    {
        Debug.Log("Starting Magic Shield Ability");

        Array buttons = Enum.GetValues(typeof(Button));

        foreach (Button button in buttons)
        {
            Button randomButton = (Button)buttons.GetValue(Random.Range(0, buttons.Length));
            Sequence.Add(randomButton);
        }
        
        SetCanvas(true);
        SequenceArrowsGroup.SetActive(true);
        
        Debug.Log("Sequence Appearing!");
        Timer.StartTimer(SequenceDuration);
        CurrentPhase = Phase.SequencePhase;
    }

    private void SequencePhaseUpdate()
    {

        if (Timer.IsInProgress() && ArrowsMoved < MaxButtonsInSequence)
        {
            Button button = Sequence[0];
            Sequence.RemoveAt(0);
            var directionRectTransform = Arrows[ArrowsMoved].GetComponent<RectTransform>();

            switch (button)
            {
                case Button.Up:
                    directionRectTransform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                    break;
                case Button.Down:
                    directionRectTransform.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);
                    break;
                case Button.Left:
                    directionRectTransform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
                    break;
                case Button.Right:
                    directionRectTransform.rotation = Quaternion.Euler(0.0f, 0.0f, 270.0f);
                    break;
                default:
                    directionRectTransform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                    break;
            }
            
            Sequence.Add(button);
            //directionRectTransform.gameObject.SetActive(true);
            ArrowsMoved++;
            Directions.Enqueue(directionRectTransform.gameObject);
        }

        if (Timer.IsFinished())
        {
            ArrowsMoved = 0;
            EndSequencePhase();
        }
    }

    private void EndSequencePhase()
    {
        foreach (GameObject arrow in Directions)
        {
            arrow.transform.rotation = Quaternion.identity;
            //arrow.SetActive(false);
        }
        SequenceArrowsGroup.SetActive(false);
        Directions.Clear();
        StartInputPhase();
    }

    private void StartInputPhase()
    {
        Timer.StartTimer(InputDuration);
        CurrentPhase = Phase.InputSequence;
    }

    private void InputSequenceUpdate()
    {
        if (Timer.IsInProgress())
            CheckUserInputs();

        else if (Timer.IsFinished())
            EndInputPhase();
    }

    private bool IsArrowInputDown()
    {
        return Input.GetButtonDown("Up") ||
            Input.GetButtonDown("Right") ||
            Input.GetButtonDown("Down") ||
            Input.GetButtonDown("Left");
    }

    private void OnCorrectInput()
    {
        Debug.Log("OnCorrectInput");

        PerfectActionCommandSound.Play();
        
        CorrectInputs += 1;

        Sequence.RemoveAt(0);
    }

    private void OnIncorrectInput()
    {
        Debug.Log("OnIncorrectInput");

        MissedActionCommandSound.Play();

        Sequence.RemoveAt(0);
    }

    private void CheckUserInputs()
    {
        if (!IsArrowInputDown())
            return;

        Button expectedButton = Sequence[0];
        Debug.Log(expectedButton);

        switch (expectedButton)
        {
            case Button.Up:
                if (Input.GetButtonDown("Up"))
                    OnCorrectInput();
                else
                    OnIncorrectInput();
                break;
            case Button.Down:
                if (Input.GetButtonDown("Down"))
                    OnCorrectInput();
                else
                    OnIncorrectInput();
                break;
            case Button.Left:
                if (Input.GetButtonDown("Left"))
                    OnCorrectInput();
                else
                    OnIncorrectInput();
                break;
            case Button.Right:
                if (Input.GetButtonDown("Right"))
                    OnCorrectInput();
                else
                    OnIncorrectInput();
                break;
            default:
                break;
        }

        if (Sequence.Count == 0)
            EndInputPhase();

    }

    private void EndInputPhase()
    {
        Timer.StopTimer();
        CurrentPhase = Phase.Inactive;

        if (CorrectInputs == 4)
        {
            Debug.Log("Magic Shield is casted perfectly!");
        }

        else
        {
            Debug.Log("Time ran out! Spell cannot be casted! ");
            Debug.Log($"Correct inputs: {CorrectInputs} / 4");
        }

        MagicShieldHealth = CalculateMagicShieldHealth();
        EndAbility();

    }

    private int CalculateMagicShieldHealth()
    {
        float m = MinMagicShieldHealth;
        float F = ShieldSlope;
        float p = CorrectInputs;

        int shieldMaxHealth = (int) (F * p + m);

        return shieldMaxHealth;
    }
}
