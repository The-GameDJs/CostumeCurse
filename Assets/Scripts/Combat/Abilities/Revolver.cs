using Microsoft.Unity.VisualStudio.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Revolver : Ability
{
    private Timer Timer;
    private float ReloadDuration = 4f;
    private bool[] IsReserved;
    private enum RevolverPhase { Load, Shoot, Inactive }
    private RevolverPhase CurrentPhase = RevolverPhase.Inactive;
    private Stack<DragAndDrop> BulletStuff = new Stack<DragAndDrop>();

    [SerializeField] Canvas ReloadCanvas;
    [SerializeField] GameObject[] BulletPositions;

    [Header("Bullet Sprites")]

    [SerializeField] public Sprite BulletEmpty;
    [SerializeField] public Sprite BulletFilled;

    public new void Start()
    {
        base.Start();
        Timer = GetComponent<Timer>();
        ReloadCanvas.gameObject.SetActive(false);
        IsReserved = new bool[BulletPositions.Length];


        TargetSchema = new TargetSchema(
            0,
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
                BulletStuff.Pop().gameObject.transform.position = BulletPositions[randomPosition].transform.position;
                IsReserved[randomPosition] = true;
            }
        }

        ReloadCanvas.gameObject.SetActive(true);
        CurrentPhase = RevolverPhase.Load;
        Timer.StartTimer(ReloadDuration);
    }

    private void ReloadUpdate()
    {
        if(Timer.IsInProgress())
        {
            CheckDroppedBullets();
        }
    }

    private void CheckDroppedBullets()
    {
        
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
