using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

public class MagicShield : Ability
{
    private enum Button { Up, Down, Left, Right };
    private enum Phase { SequencePhase, Activate, Inactive };
    private Timer SequenceTimer;
    private Queue<Button> Sequence = new Queue<Button>();
    private Queue<Button> SequenceOrder = new Queue<Button>();
    private Queue<GameObject> Directions = new Queue<GameObject>();

    [SerializeField]
    private GameObject Shield;
    [SerializeField]
    private GameObject[] Arrows; // 0: Up, 1: Down, 2: Left, 3: Right 

    private readonly float SequenceDuration = 2f;
    private readonly float InitialPosition = 5f;
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
                        direction.transform.localPosition = Position;
                        Position += new Vector3(PositionOffset, 0, 0);
                        direction.SetActive(true);
                        Directions.Enqueue(direction);
                        break;
                    case Button.Down:
                        direction = Instantiate(Arrows[1]);
                        direction.transform.localPosition = Position;
                        Position += new Vector3(PositionOffset, 0, 0);
                        direction.SetActive(true);
                        Directions.Enqueue(direction);
                        break;
                    case Button.Left:
                        direction = Instantiate(Arrows[2]);
                        direction.transform.localPosition = Position;
                        Position += new Vector3(PositionOffset, 0, 0);
                        direction.SetActive(true);
                        Directions.Enqueue(direction);
                        break;
                    case Button.Right:
                        direction = Instantiate(Arrows[3]);
                        direction.transform.localPosition = Position;
                        Position += new Vector3(PositionOffset, 0, 0);
                        direction.SetActive(true);
                        Directions.Enqueue(direction);
                        break;
                }
            }
        }

        if (SequenceTimer.IsFinished())
        {
            if (Directions.Count != 0)
                Directions.Dequeue().SetActive(false);
        }
    }
}
