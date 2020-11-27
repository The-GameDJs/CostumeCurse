using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

public class MagicShield : Ability
{
    private enum Button { Up, Down, Left, Right };
    private enum Phase { SequencePhase, InputSequence, Activate, Inactive };
    private Timer SequenceTimer;
    private Queue<Button> Sequence = new Queue<Button>();
    private Queue<Button> SequenceOrder = new Queue<Button>();
    private Queue<GameObject> Directions = new Queue<GameObject>();

    [SerializeField]
    private GameObject Shield;
    [SerializeField]
    private GameObject[] Arrows; // 0: Up, 1: Down, 2: Left, 3: Right 

    private readonly float SequenceDuration = 2f;
    private readonly float InputDuration = 5f;
    private readonly float PositionOffset = 4f;
    private Vector3 Position = Vector3.zero;
    private Phase CurrentPhase = Phase.Inactive;

    private int CorrectInputs = 0;

    public new void Start()
    {
        base.Start();
        SequenceTimer = GetComponent<Timer>();
        Shield.SetActive(false);

        foreach(GameObject arrow in Arrows)
            arrow.SetActive(false);

        TargetSchema = new TargetSchema(
            0,
            CombatantType.Ally,
            SelectorType.All);
    }

    new public void StartAbility()
    {
        CorrectInputs = 0;
        base.StartAbility();
    }

    private void Update()
    {
        if(CurrentPhase == Phase.SequencePhase)
            SequencePhaseUpdate();

        else if(CurrentPhase == Phase.InputSequence)
            InputSequenceUpdate();
    }

    protected override void ContinueAbilityAfterTargeting()
    {
        StartMagicShieldSequence();
    }

    protected override void EndAbility()
    {
        // TODO
    }

    private void StartMagicShieldSequence()
    {
        Debug.Log("Starting Magic Shield Ability");
        Array buttons = Enum.GetValues(typeof(Button));

        for (int i = 0; i < buttons.Length; i++)
        {
            Button random_button = (Button)buttons.GetValue(Random.Range(0, buttons.Length));
            Sequence.Enqueue(random_button);
            SequenceOrder.Enqueue(random_button);
        }

        Position = Arrows[0].transform.position;
        Debug.Log("Sequence Appearing!");
        SequenceTimer.StartTimer(SequenceDuration);
        CurrentPhase = Phase.SequencePhase;
          
    }

    private void SequencePhaseUpdate()
    {

        if (SequenceTimer.IsInProgress())
        {
            if (Sequence.Count != 0)
            {
                Button button = Sequence.Dequeue();
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

                direction.transform.localPosition = Position;
                Position += new Vector3(PositionOffset, 0, 0);
                direction.SetActive(true);
                Directions.Enqueue(direction);
            }
        }

        if (SequenceTimer.IsFinished())
        {
            EndSequencePhase();
        }
    }

    private void EndSequencePhase()
    {
        foreach (GameObject arrow in Directions)
            arrow.SetActive(false);

        StartInputPhase();
    }

    private void StartInputPhase()
    {
        SequenceTimer.StartTimer(InputDuration);
        CurrentPhase = Phase.InputSequence;
    }

    private void InputSequenceUpdate()
    {
        if (SequenceTimer.IsInProgress())
            CheckUserInputs();

        else if (SequenceTimer.IsFinished())
            EndInputPhase();
    }

    private bool IsArrowInputDown()
    {
        return Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.D) ||
            Input.GetKeyDown(KeyCode.S) ||
            Input.GetKeyDown(KeyCode.A);
    }

    private void OnCorrectInput()
    {
        Debug.Log("OnCorrectInput");
        
        CorrectInputs += 1;

        SequenceOrder.Dequeue();
    }

    private void OnIncorrectInput()
    {
        Debug.Log("OnIncorrectInput");

        SequenceOrder.Dequeue();
    }

    private void CheckUserInputs()
    {
        if (!IsArrowInputDown())
            return;

        Button expectedButton = SequenceOrder.Peek();
        Debug.Log(expectedButton);

        switch (expectedButton)
        {
            case Button.Up:
                if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A))
                    OnIncorrectInput();
                else
                    OnCorrectInput();
                break;
            case Button.Down:
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.D))
                    OnIncorrectInput();
                else
                    OnCorrectInput();
                break;
            case Button.Left:
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.S))
                    OnIncorrectInput();
                else
                    OnCorrectInput();
                break;
            case Button.Right:
                if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.W))
                    OnIncorrectInput();
                else
                    OnCorrectInput();
                break;
            default:
                break;
        }

        if (SequenceOrder.Count == 0)
            EndInputPhase();

    }

    private void EndInputPhase()
    {
        SequenceTimer.StopTimer();
        CurrentPhase = Phase.Inactive;

        if (CorrectInputs == 4)
        {
            Debug.Log("Magic Shield is casted!");
        }

        else
        {
            Debug.Log("Time ran out! Spell cannot be casted! ");
            Debug.Log($"Correct inputs: {CorrectInputs} / 4");
        }

        EndAbility();

    }
}
