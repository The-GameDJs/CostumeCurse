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
    private Queue<GameObject> Directions = new Queue<GameObject>();

    [SerializeField]
    private GameObject Shield;
    [SerializeField]
    private GameObject[] Arrows; // 0: Up, 1: Down, 2: Left, 3: Right 

    private readonly float SequenceDuration = 2f;
    private readonly float initialPosition = 5f;
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

        for(int i = 0; i < buttons.Length; i++)
            Sequence.Enqueue((Button)buttons.GetValue(Random.Range(0, buttons.Length)));

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
                GameObject direction = new GameObject();
                Vector3 initialposition = direction.transform.position;

                switch (button)
                {
                    case Button.Up:
                        direction = Arrows[0];
                        initialposition += Arrows[0].transform.position;
                        direction.SetActive(true);
                        Directions.Enqueue(direction);
                        break;
                    case Button.Down:
                        direction = Arrows[1];
                        direction.SetActive(true);
                        Directions.Enqueue(direction);
                        break;
                    case Button.Left:
                        direction = Arrows[2];
                        direction.SetActive(true);
                        Directions.Enqueue(direction);
                        break;
                    case Button.Right:
                        direction = Arrows[3];
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
