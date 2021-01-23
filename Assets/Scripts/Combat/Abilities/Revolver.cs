using Microsoft.Unity.VisualStudio.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Revolver : Ability
{
    private Timer Timer;
    private float ReloadDuration = 4f;
    private bool[] IsReserved;
    private enum RevolverPhase { Load, Shoot, Inactive }
    private RevolverPhase CurrentPhase = RevolverPhase.Inactive;
    private Stack<DragAndDrop> BulletStuff = new Stack<DragAndDrop>();
    private Text ReloadTimerText;

    [SerializeField] Canvas ReloadCanvas;
    [SerializeField] GameObject[] BulletPositions;

    public new void Start()
    {
        base.Start();
        Timer = GetComponent<Timer>();
        ReloadCanvas.gameObject.SetActive(false);
        IsReserved = new bool[BulletPositions.Length];

        TargetSchema = new TargetSchema(
            1,
            CombatantType.Enemy,
            SelectorType.Number);
    }

    public new void StartAbility(bool userTargeting = false)
    {
        base.StartAbility();
        Debug.Log("Started Revolver Ability");
    }

    private void Update()
    {
        if (CurrentPhase == RevolverPhase.Load)
            ReloadUpdate();

        if (CurrentPhase == RevolverPhase.Shoot)
            ShootUpdate();
    }

    protected override void ContinueAbilityAfterTargeting()
    {
        StartReloadPhase();
    }

    private void StartReloadPhase()
    {
        DragAndDrop[] bullets = ReloadCanvas.GetComponentsInChildren<DragAndDrop>();

        foreach (DragAndDrop bullet in bullets)
            BulletStuff.Push(bullet);

        while(BulletStuff.Count != 0)
        {
            int randomPosition = Random.Range(0, 8);

            if (!IsReserved[randomPosition])
            {
                DragAndDrop bullet = BulletStuff.Pop();
                bullet.gameObject.transform.position = BulletPositions[randomPosition].transform.position;
                bullet.InitializeStartingPosition();
                IsReserved[randomPosition] = true;
            }
        }

        ReloadCanvas.gameObject.SetActive(true);
        ReloadTimerText = GameObject.Find("ReloadTimerText").GetComponent<Text>();
        CurrentPhase = RevolverPhase.Load;
        ReloadCanvas.transform.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Timer.StartTimer(ReloadDuration);
    }

    private void ReloadUpdate()
    {
        if(Timer.IsInProgress())
        {
            float timeRemaining = ReloadDuration - Timer.GetProgress();
            ReloadTimerText.text = Mathf.RoundToInt(timeRemaining) + "";
        }

        if(Timer.IsFinished())
        {
            EndReloadPhase();
        }
    }

    private void EndReloadPhase()
    {
        // TODO: Calculate number of bullets dropped
    }

    private void StartShootingPhase()
    {
        throw new NotImplementedException();
    }

    private void ShootUpdate()
    {
        throw new NotImplementedException();
    }

    protected override void EndAbility()
    {
        throw new System.NotImplementedException();
    }
}
