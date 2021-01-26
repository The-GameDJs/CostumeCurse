using Microsoft.Unity.VisualStudio.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.Threading;

public class Revolver : Ability
{
    private Timer Timer;
    private float ReloadDuration = 5f;
    private float ShootingDuration = 5f;
    private int TotalBulletsDropped = 0;
    private bool[] IsBulletReserved;
    private bool[] IsPimpkinReserved;

    private enum RevolverPhase { Load, Shoot, Inactive }
    private RevolverPhase CurrentPhase = RevolverPhase.Inactive;
    private Stack<DragAndDrop> BulletStuff = new Stack<DragAndDrop>();
    private Text ReloadTimerText;

    [Header("Reload Phase")]
    [SerializeField] Canvas ReloadCanvas;
    [SerializeField] GameObject[] BulletPositions;
    [SerializeField] GameObject[] Clips;

    [Header("Shooting Phase")]
    [SerializeField] Canvas ShootingCanvas;
    [SerializeField] GameObject[] BulletUI;
    [SerializeField] GameObject[] PimpkinSpawnLocations;
    private Stack<PimpkinHead> Pimpkins = new Stack<PimpkinHead>();


    public new void Start()
    {
        base.Start();
        Timer = GetComponent<Timer>();
        ReloadCanvas.gameObject.SetActive(false);
        ShootingCanvas.gameObject.SetActive(false);
        IsBulletReserved = new bool[BulletPositions.Length];
        IsPimpkinReserved = new bool[PimpkinSpawnLocations.Length];

        for (int i = 0; i < BulletUI.Length; i++)
            BulletUI[i].SetActive(false);

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
        ReloadCanvas.transform.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        SetBulletLocations(bullets);

        ReloadCanvas.gameObject.SetActive(true);
        ReloadTimerText = GameObject.Find("ReloadTimerText").GetComponent<Text>();
        CurrentPhase = RevolverPhase.Load;
        Timer.StartTimer(ReloadDuration);
    }

    private void SetBulletLocations(DragAndDrop[] bullets)
    {
        foreach (DragAndDrop bullet in bullets)
            BulletStuff.Push(bullet);

        while (BulletStuff.Count != 0)
        {
            int randomPosition = Random.Range(0, 8);

            if (!IsBulletReserved[randomPosition])
            {
                DragAndDrop bullet = BulletStuff.Pop();
                bullet.gameObject.transform.position = BulletPositions[randomPosition].transform.position;
                bullet.InitializeStartingPosition();
                IsBulletReserved[randomPosition] = true;
            }
        }
    }

    private void ReloadUpdate()
    {
        if(Timer.IsInProgress())
        {
            float timeRemaining = ReloadDuration - Timer.GetProgress();
            ReloadTimerText.text = Mathf.RoundToInt(timeRemaining) + "";
        }

        if(Timer.IsFinished())
            EndReloadPhase();
    }

    private void EndReloadPhase()
    {
        Timer.ResetTimer();
        ReloadCanvas.gameObject.SetActive(false);

        ResetReloadValues();

        Debug.Log($"Total bullets dropped: {TotalBulletsDropped}");
        Thread.Sleep(500);
        StartShootingPhase();
    }

    private void ResetReloadValues()
    {
        foreach (GameObject c in Clips)
        {
            Clip singleClip = c.GetComponent<Clip>();
            if (singleClip.IsClipFilled())
            {
                TotalBulletsDropped++;
            }

            singleClip.ResetRevolverValues();
        }

        for (int i = 0; i < IsBulletReserved.Length; i++)
            IsBulletReserved[i] = false;
    }

    private void StartShootingPhase()
    {
        ShootingCanvas.gameObject.SetActive(true);
        ShootingCanvas.transform.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        for(int i = 0; i < TotalBulletsDropped; i++)
            BulletUI[i].SetActive(true);

        PimpkinHead[] pimpkins = ShootingCanvas.GetComponentsInChildren<PimpkinHead>();

        foreach (PimpkinHead pimpkin in pimpkins)
            Pimpkins.Push(pimpkin);

        while (Pimpkins.Count != 0)
        {
            int randomPosition = Random.Range(0, 6);

            if (!IsPimpkinReserved[randomPosition])
            {
                PimpkinHead pimpkin = Pimpkins.Pop();
                pimpkin.gameObject.transform.position = PimpkinSpawnLocations[randomPosition].transform.position;
                IsPimpkinReserved[randomPosition] = true;
            }
        }

        CurrentPhase = RevolverPhase.Shoot;
        Timer.StartTimer(ShootingDuration);
    }

    private void ShootUpdate()
    {
        
    }

    protected override void EndAbility()
    {
        throw new System.NotImplementedException();
    }
}
