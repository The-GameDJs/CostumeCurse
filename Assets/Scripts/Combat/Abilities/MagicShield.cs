using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

public class MagicShield : Ability
{
    private enum Button { Up, Down, Left, Right };
    private enum Phase { SequencePhase, InputSequence, Inactive };
    private Timer Timer;
    private List<Button> Sequence = new List<Button>();
    private Queue<GameObject> Directions = new Queue<GameObject>();

    [SerializeField]
    private GameObject[] Arrows; // 0: Up, 1: Down, 2: Left, 3: Right 

    private readonly float SequenceDuration = 2f;
    private readonly float InputDuration = 5f;
    private readonly float ArrowPositionHeight = 5f;
    private readonly float NextArrowPositionOffsetX = 4f;
    private readonly int MaxButtonsInSequence = 4;
    private Vector3 ArrowStartPosition = Vector3.zero;
    private Phase CurrentPhase = Phase.Inactive;

    private readonly float MinMagicShieldHealth = 20f;
    private readonly float ShieldSlope = 100f;
    private int CorrectInputs = 0;
    private int ArrowsMoved = 0;
    private int MagicShieldHealth;

    public new void Start()
    {
        base.Start();
        Timer = GetComponent<Timer>();

        foreach(GameObject arrow in Arrows)
            arrow.SetActive(false);

        TargetSchema = new TargetSchema(
            0,
            CombatantType.Ally,
            SelectorType.All);
    }

    new public void StartAbility(bool userTargeting = false)
    {
        CorrectInputs = 0;
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

        foreach (GameObject target in TargetedCombatants)
            target.GetComponent<Combatant>().ApplyShield(MagicShieldHealth);

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

        ArrowStartPosition = new Vector3(-NextArrowPositionOffsetX, ArrowPositionHeight, 0)
            + transform.parent.transform.position;
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
            GameObject direction;

            switch (button)
            {
                case Button.Up:
                    direction = Instantiate(Arrows[0]);
                    break;
                case Button.Down:
                    direction = Instantiate(Arrows[1]);
                    break;
                case Button.Left:
                    direction = Instantiate(Arrows[2]);
                    break;
                case Button.Right:
                    direction = Instantiate(Arrows[3]);
                    break;
                default:
                    direction = Instantiate(Arrows[0]);
                    break;
            }

            direction.transform.localPosition = ArrowStartPosition;
            Sequence.Add(button);
            ArrowStartPosition += new Vector3(NextArrowPositionOffsetX, 0, 0);
            direction.SetActive(true);
            ArrowsMoved++;
            Directions.Enqueue(direction);

        }

        if (Timer.IsFinished())
        {
            ArrowsMoved = 0;
            EndSequencePhase();
        }
    }

    private void EndSequencePhase()
    {
        foreach (GameObject arrow in Directions) {
            Destroy(arrow);
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
        
        CorrectInputs += 1;

        Sequence.RemoveAt(0);
    }

    private void OnIncorrectInput()
    {
        Debug.Log("OnIncorrectInput");

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
