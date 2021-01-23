using Microsoft.Unity.VisualStudio.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revolver : Ability
{
    private Timer Timer;
    private enum RevolverPhase { Load, Shoot, Inactive }
    private RevolverPhase CurrentPhase = RevolverPhase.Inactive;

    [SerializeField] GameObject Bullet;
    [SerializeField] Canvas ReloadCanvas;



    [Header("Bullet Sprites")]

    [SerializeField] Sprite BulletEmpty;
    [SerializeField] Sprite BulletFilled;

    public new void Start()
    {
        base.Start();
        Timer = GetComponent<Timer>();
        ReloadCanvas.gameObject.SetActive(false);

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
            RevolverLoadUpdate();

        if (CurrentPhase == RevolverPhase.Shoot)
            RevolverShootUpdate();
    }

    protected override void ContinueAbilityAfterTargeting()
    {
        StartReloadPhase();
    }

    private void StartReloadPhase()
    {
        throw new NotImplementedException();
    }

    private void RevolverShootUpdate()
    {
        throw new NotImplementedException();
    }

    private void StartShootingPhase()
    {
        throw new NotImplementedException();
    }

    private void RevolverLoadUpdate()
    {
        throw new NotImplementedException();
    }

    protected override void EndAbility()
    {
        throw new System.NotImplementedException();
    }
}
