﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ThunderStorm : Ability
{
    private enum Phase { Cloud, Strike, Inactive }

    [SerializeField]
    private GameObject thunder;
    private Timer timer;

    private readonly int totalThunderStrikes = 3;
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
    private Vector3 scaleChange = new Vector3(0.01f, 0.01f, 0.01f);

    private void Start()
    {
        timer = GetComponent<Timer>();
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

            if (currentThunderStrike < totalThunderStrikes)
                Invoke(nameof(NewThunderStrike), UnityEngine.Random.Range(strikeTimeInterval, 1.5f * strikeTimeInterval));
            else
                ConcludeAbility();
        }
    }

    private void AnimateThunderstrike(float progress)
    {
        if (progress <= timeWindowForStrikes / 2.0f + perfectStrikeTimeWindow)
            thunder.transform.localScale += scaleChange;
        else
            thunder.transform.localScale -= scaleChange;

        if (WithinPerfectStrikeWindow(progress))
            thunder.GetComponent<Renderer>().material.color = Color.red;
        else if (WithinGoodStrikeWindow(progress))
            thunder.GetComponent<Renderer>().material.color = Color.blue;
        else
            thunder.GetComponent<Renderer>().material.color = Color.white;
    }

    private void ConcludeAbility()
    {
        Debug.Log($"Thunderstorm Damage total: {currentDamage}");
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
            Debug.Log($"Thunder Cloud Build Up damage: {currentDamage}");

            StartThunderStrikePhase();
        }
    }

    private void ThundercloudMash()
    {
        if (Input.GetButtonDown("Action Command"))
        {
            pressCounter++;
            thunder.transform.localScale += 20 * scaleChange;
        }

    }

    public override void UseAbility()
    {
        StartThunderCloudPhase();
    }

    private void StartThunderCloudPhase()
    {
        Debug.Log("Thundercloud phase start");

        pressCounter = 0;
        phase = Phase.Cloud;
        timer.StartTimer(maxTimeOfInput);
    }

    private int CalculateThunderCloudDamage()
    {
        int damage = (int)((pressCounter / 100.0f * (maxDamage - minDamage + 1)) + minDamage);

        return damage;
    }


    private void StartThunderStrikePhase()
    {
        Debug.Log("Thunderbolt phase start");

        phase = Phase.Inactive;
        thunder.transform.localScale = Vector3.one;

        currentThunderStrike = 0;

        Invoke(nameof(NewThunderStrike), UnityEngine.Random.Range(strikeTimeInterval, 1.5f * strikeTimeInterval));

    }

    private void NewThunderStrike()
    {
        currentThunderStrike++;
        Debug.Log($"New thunderstrike, currently on # {currentThunderStrike}");

        phase = Phase.Strike;

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
