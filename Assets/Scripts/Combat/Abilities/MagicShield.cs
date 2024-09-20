using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MagicShield : Ability
{
    private enum Button { Up, Down, Left, Right };
    private enum Phase { SequencePhase, InputSequence, Inactive };
    private Timer Timer;
    private List<Button> Sequence = new List<Button>();
    private Queue<GameObject> Directions = new Queue<GameObject>();
    private Vector2 currentInput;

    [SerializeField] private Canvas MagicShieldCanvas;
    [SerializeField] private GameObject SequenceArrowsGroup;
    [SerializeField] private GameObject[] Arrows; // 0: Up, 1: Down, 2: Left, 3: Right
    [SerializeField] private AudioSource MagicShieldSound;

    private Vector3 InitialCanvasPosition;
    private readonly float SequenceDuration = 2.5f;
    private float SequenceCountdown = 0.0f;
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

        TargetSchema = new TargetSchema(
            0,
            CombatantType.Ally,
            SelectorType.All);
    }

    private void OnEnable()
    {
        InputManager.JoystickTapped += OnJoystickTapped;
    }

    private void OnJoystickTapped(Vector2 input)
    {
        currentInput = input;
        
        if (Timer.IsInProgress() && CurrentPhase == Phase.InputSequence)
            CheckUserInputs();
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
        
        foreach (var arrow in Arrows)
        {
            arrow.GetComponent<Image>().color = Color.cyan;
            arrow.transform.rotation = Quaternion.identity;
            arrow.SetActive(false);
        }

        SetCanvas(false);
        MagicShieldSound.Play();

        foreach (GameObject target in TargetedCombatants)
            target.GetComponent<Combatant>().ApplyShield(MagicShieldHealth, Element);

        Animator.SetBool("IsFinishedCasting", true);

        CombatSystem.EndTurn();
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

        foreach (var arrow in Arrows)
        {
            if (arrow.activeSelf)
            {
                arrow.SetActive(false);
            }
        }
        
        SetCanvas(true);

        Debug.Log("Sequence Appearing!");
        Timer.StartTimer(SequenceDuration);
        CurrentPhase = Phase.SequencePhase;
    }

    private void SequencePhaseUpdate()
    {
        if (Timer.IsInProgress() && ArrowsMoved < MaxButtonsInSequence && SequenceCountdown >= 0.5f)
        {
            Button button = Sequence[0];
            Sequence.RemoveAt(0);
            Arrows[ArrowsMoved].SetActive(true);
            Arrows[ArrowsMoved].GetComponent<Image>().color = Color.cyan;
            var directionRectTransform = Arrows[ArrowsMoved].GetComponent<RectTransform>();

            directionRectTransform.rotation = button switch
            {
                Button.Up => Quaternion.Euler(0.0f, 0.0f, 0.0f),
                Button.Down => Quaternion.Euler(0.0f, 0.0f, 180.0f),
                Button.Left => Quaternion.Euler(0.0f, 0.0f, 90.0f),
                Button.Right => Quaternion.Euler(0.0f, 0.0f, 270.0f),
                _ => Quaternion.Euler(0.0f, 0.0f, 0.0f)
            };

            Sequence.Add(button);
            ArrowsMoved++;
            Directions.Enqueue(directionRectTransform.gameObject);
            SequenceCountdown = 0.0f;
        }

        if (Timer.IsFinished())
        {
            ArrowsMoved = 0;
            EndSequencePhase();
        }
        
        SequenceCountdown += Time.deltaTime;
    }

    private void EndSequencePhase()
    {
        SequenceCountdown = 0.0f;
        foreach (GameObject arrow in Directions)
        {
            arrow.transform.rotation = Quaternion.identity;
            arrow.SetActive(false);
        }
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
        if (Timer.IsFinished())
            EndInputPhase();
    }

    private bool IsArrowInputDown()
    {
        return currentInput.x >= 0.2f 
               || currentInput.x <= -0.2f
               || currentInput.y >= 0.2f
               || currentInput.y <= -0.2f;
    }

    private void OnCorrectInput()
    {
        Debug.Log("OnCorrectInput");

        PerfectActionCommandSound.Play();
        
        CorrectInputs += 1;

        Sequence.RemoveAt(0);
        
        Arrows[ArrowsMoved].GetComponent<Image>().color = Color.cyan;
    }

    private void OnIncorrectInput()
    {
        Debug.Log("OnIncorrectInput");

        MissedActionCommandSound.Play();

        Sequence.RemoveAt(0);
        
        Arrows[ArrowsMoved].GetComponent<Image>().color = Color.red;
    }

    private void CheckUserInputs()
    {
        if (!IsArrowInputDown())
            return;

        Button expectedButton = Sequence[0];
        Debug.Log(expectedButton);
        // var currentInput = new Vector2(InputManager.InputDirection.x, InputManager.InputDirection.z);
        
        
        switch (expectedButton)
        {
            case Button.Up:
                if (currentInput.y >= 0.1f)
                    OnCorrectInput();
                else
                    OnIncorrectInput();
                Arrows[ArrowsMoved].transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                break;
            case Button.Down:
                if (currentInput.y <= -0.1f)
                    OnCorrectInput();
                else
                    OnIncorrectInput();
                Arrows[ArrowsMoved].transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);
                break;
            case Button.Left:
                if (currentInput.x <= -0.1f)
                    OnCorrectInput();
                else
                    OnIncorrectInput();
                Arrows[ArrowsMoved].transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
                break;
            case Button.Right:
                if (currentInput.x >= 0.1f)
                    OnCorrectInput();
                else
                    OnIncorrectInput();
                Arrows[ArrowsMoved].transform.rotation = Quaternion.Euler(0.0f, 0.0f, 270.0f);
                break;
            default:
                break;
        }
        Arrows[ArrowsMoved].SetActive(true);
        ArrowsMoved++;

        if (Sequence.Count == 0)
            EndInputPhase();
    }

    private IEnumerator InitializeEndAbilityPhase()
    {
        yield return new WaitForSeconds(1.0f);
        EndAbility();
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

        ArrowsMoved = 0;
        
        MagicShieldHealth = CalculateMagicShieldHealth();
        StartCoroutine(InitializeEndAbilityPhase());
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
