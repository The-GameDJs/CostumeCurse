﻿using Assets.Scripts.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class ThunderStorm : Ability
{
    private enum Phase { Cloud, Strike, Inactive }

    [SerializeField]
    private GameObject thunder;
    private Timer timer;

    private readonly int TotalThunderStrikes = 3;
    private int currentThunderStrike = 0;

    private readonly float timeWindowForStrikes = 3.0f;
    private readonly float goodStrikeTimeWindow = 1.5f;
    private readonly float perfectStrikeTimeWindow = 0.5f;
    private readonly float strikeTimeInterval = 2.0f;

    private int pressCounter;
    private readonly float maxTimeOfInput = 5.0f;
    private readonly int minDamage = 10;
    private readonly int maxDamage = 30;
    private readonly int perfectDamageBonus = 50;
    private readonly int goodDamageBonus = 20;
    private int currentDamage;

    private Phase phase = Phase.Inactive;
    
    private readonly float thunderCloudGrowthSpeed = 0.05f;
    private readonly float thunderStrikeGrowthSpeed = 1.0f;

    public new void Start()
    {
        base.Start();
        timer = GetComponent<Timer>();
        thunder.SetActive(false);

        TargetSchema = new TargetSchema(
            0,
            CombatantType.Enemy,
            SelectorType.All);
    }

    private void Update()
    {
        if (phase == Phase.Cloud)
            ThundercloudUpdate();

        if (phase == Phase.Strike)
            ThunderstrikeUpdate();
    }

    private void ThunderstrikeUpdate()
    {
        if (timer.IsInProgress())
        {
            float progress = timer.GetProgress();

            AnimateThunderstrike(progress);

            if (Input.GetButtonDown("Action Command"))
            {
                Debug.Log("Action Command pressed during Thunderstrike Phase");

                timer.StopTimer();
            }
        }

        if (timer.IsFinished())
        {
            EvaluateThunderStrikeInput(timer.GetProgress());
            phase = Phase.Inactive;
            thunder.SetActive(false);

            if (currentThunderStrike < TotalThunderStrikes)
                Invoke(nameof(NewThunderStrike), UnityEngine.Random.Range(strikeTimeInterval, 1.5f * strikeTimeInterval));
            else
                EndAbility();
        }
    }

    private void AnimateThunderstrike(float progress)
    {        
        if (progress <= timeWindowForStrikes / 2.0f)
            thunder.transform.localScale += Vector3.one * thunderStrikeGrowthSpeed * Time.deltaTime;
        else
            thunder.transform.localScale -= Vector3.one * thunderStrikeGrowthSpeed * Time.deltaTime;

        if (WithinPerfectStrikeWindow(progress))
            thunder.GetComponent<Renderer>().material.color = Color.red;
        else if (WithinGoodStrikeWindow(progress))
            thunder.GetComponent<Renderer>().material.color = Color.blue;
        else
            thunder.GetComponent<Renderer>().material.color = Color.white;
    }

    protected override void EndAbility()
    {
        Debug.Log($"Thunderstorm Damage total: {currentDamage}");
        thunder.SetActive(false);

        // Deal damage to defender, wait
        // for now, let's just pick 3 random ones
        // this might be done before EndAbility
        Attack attack = new Attack(currentDamage);
        for(int i = 0; i < TotalThunderStrikes; i++)
            TargetedCombatants[Random.Range(0, TargetedCombatants.Length)].GetComponent<Combatant>()
                .Defend(attack);

        CombatSystem.EndTurn(this.GetComponentInParent<Combatant>().gameObject);
    }

    private void ThundercloudUpdate()
    {
        if (timer.IsInProgress())
        {
            ThundercloudMash();
        }
        else
        {
            Debug.Log($"Thundercloud Complete with presses: {pressCounter}");

            currentDamage = CalculateThunderCloudDamage();
            thunder.SetActive(false);
            Debug.Log($"Thunder Cloud Build Up damage: {currentDamage}");

            StartThunderStrikePhase();
        }
    }

    private void ThundercloudMash()
    {
        if (Input.GetButtonDown("Action Command"))
        {
            pressCounter++;
            thunder.transform.localScale += Vector3.one * thunderCloudGrowthSpeed;
        }
    }

    protected override void ContinueAbilityAfterTargeting()
    {
        StartThunderCloudPhase();
    }

    private void StartThunderCloudPhase()
    {
        pressCounter = 0;
        phase = Phase.Cloud;

        thunder.SetActive(true);
        thunder.transform.position = transform.position + 5f * Vector3.up;
        thunder.transform.localScale = Vector3.one;
        thunder.GetComponent<Renderer>().material.color = Color.white;

        timer.StartTimer(maxTimeOfInput);
    }

    private int CalculateThunderCloudDamage()
    {
        int damage = (int)((pressCounter / 100.0f * (maxDamage - minDamage + 1)) + minDamage);

        return damage;
    }


    private void StartThunderStrikePhase()
    {
        phase = Phase.Inactive;

        thunder.SetActive(true);
        thunder.transform.localScale = Vector3.one;
        thunder.transform.position = transform.position + 5f * Vector3.up + 5f * Vector3.right;

        currentThunderStrike = 0;

        Invoke(nameof(NewThunderStrike), UnityEngine.Random.Range(strikeTimeInterval, 1.5f * strikeTimeInterval));

    }

    private void NewThunderStrike()
    {
        currentThunderStrike++;
        Debug.Log($"New thunderstrike, currently on # {currentThunderStrike}");

        phase = Phase.Strike;

        thunder.SetActive(true);
        thunder.transform.localScale = Vector3.one;
        timer.StartTimer(timeWindowForStrikes);
    }

    // TODO: Implement user feedback
    private void EvaluateThunderStrikeInput(float timerValue)
    {
        Debug.Log("Timer Value: " + timerValue);

        if (WithinPerfectStrikeWindow(timerValue))
        {
            Debug.Log("Perfect Strike");
            currentDamage += perfectDamageBonus;
        }
        else if (WithinGoodStrikeWindow(timerValue))
        {
            Debug.Log("Good Strike");
            currentDamage += goodDamageBonus;
        }
        else
        {
            Debug.Log("Missed Strike");
        }
    }

    private bool WithinGoodStrikeWindow(float timerValue)
    {
        return timerValue >= ((timeWindowForStrikes - goodStrikeTimeWindow) / 2.0f) &&
                    timerValue <= ((timeWindowForStrikes + goodStrikeTimeWindow) / 2.0f);
    }

    private bool WithinPerfectStrikeWindow(float timerValue)
    {
        return timerValue >= ((timeWindowForStrikes - perfectStrikeTimeWindow) / 2.0f) &&
                    timerValue <= ((timeWindowForStrikes + perfectStrikeTimeWindow) / 2.0f);
    }
}
