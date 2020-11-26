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

    public new void Start()
    {
        base.Start();
        SequenceTimer = GetComponent<Timer>();
        Shield.SetActive(false);

        foreach(GameObject arrow in Arrows)
        {
            arrow.SetActive(false);
        }

        TargetSchema = new TargetSchema(
            0,
            CombatantType.Ally,
            SelectorType.All);
    }

    new public void StartAbility()
    {
        base.StartAbility();
    }

    private void Update()
    {
        if(CurrentPhase == Phase.SequencePhase)
        {
            SequencePhaseUpdate();
        }

        else if(CurrentPhase == Phase.InputSequence)
        {
            InputSequenceUpdate();
        }
    }

    protected override void ContinueAbilityAfterTargeting()
    {
        StartMagicShieldSequence();
    }

    protected override void EndAbility()
    {
        throw new System.NotImplementedException();
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
        if (Directions.Count != 0)
            Directions.Dequeue().SetActive(false);
        else
            StartInputPhase();
    }

    private void StartInputPhase()
    {
        CurrentPhase = Phase.Inactive;
        SequenceTimer.StartTimer(InputDuration);
        CurrentPhase = Phase.InputSequence;
    }

    private void InputSequenceUpdate()
    {
        if(SequenceTimer.IsInProgress())
        {
            CheckUserInputs();
        }

        if (SequenceTimer.IsFinished())
        {
            if (SequenceTimer.GetProgress() < SequenceDuration && SequenceOrder.Count > 0)
            {
                Debug.Log("Incorrect button pressed, Magic Shield could not be casted!");
            }

            else if (SequenceOrder.Count > 0)
            {
                Debug.Log("Time ran out! Spell cannot be casted!");
            }
            
            else
            {
                Debug.Log("Magic Shield is casted!");
            }

            CurrentPhase = Phase.Inactive;
        }
    }

    private void CheckUserInputs()
    {
        Button expected_button = SequenceOrder.Peek();

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (SequenceOrder.Peek() == Button.Up)
            {
                Debug.Log("Got one sequence! Button Up!");
                SequenceOrder.Dequeue();
            }

            else
            {
                SequenceTimer.StopTimer();
            }

            IsSequenceGuessed();

        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (SequenceOrder.Peek() == Button.Down)
            {
                Debug.Log("Got one sequence! Button Down!");
                SequenceOrder.Dequeue();
            }

            else
            {
                SequenceTimer.StopTimer();
            }

            IsSequenceGuessed();

        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            if (SequenceOrder.Peek() == Button.Left)
            {
                Debug.Log("Got one sequence! Button Left!");
                SequenceOrder.Dequeue();
            }

            else
            {
                SequenceTimer.StopTimer();
            }

            IsSequenceGuessed();

        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            if (SequenceOrder.Peek() == Button.Right)
            {
                Debug.Log("Got one sequence! Button Right!");
                SequenceOrder.Dequeue();
            }

            else
            {
                SequenceTimer.StopTimer();
            }

            IsSequenceGuessed();

        }
    }

    private void IsSequenceGuessed()
    {
        if (SequenceOrder.Count == 0)
        {
            Debug.Log("Congrats! You got the sequence correctly!");
            SequenceTimer.StopTimer();
        }
    }
}
