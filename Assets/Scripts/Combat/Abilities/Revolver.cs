using Assets.Scripts.Combat;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Combat.Abilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Revolver : Ability
{
    private Timer Timer;
    private readonly float ReloadDuration = 3f;
    private readonly float ShootingDuration = 0.8f;
    private int BulletsInClip = 0;
    private bool[] IsBulletReserved;
    private bool[] IsPimpkinReserved;
    private DragAndDrop[] BulletUIInReload;
    private PimpkinHead[] Pimpkins;

    private enum RevolverPhase { Load, Shoot, Inactive }
    private RevolverPhase CurrentPhase = RevolverPhase.Inactive;
    [SerializeField] private GameObject RevolverCanvas;
    [SerializeField] private Text ReloadTimerText;
    [SerializeField] private Text ShootingTimerText;
    
    private float TotalDamage;
    private readonly float BaseBulletDamage = 10f;
    private readonly float BaseTotalDamage = 10f;
    private readonly float MaxDamage = 80f;
    private readonly int RandomDamageRangeOffset = 10;
    private readonly float BulletTargetHeightOffset = 3.0f;

    [SerializeField] AudioSource ReloadSource;
    [SerializeField] AudioSource ShootSource;
    [SerializeField] AudioSource MissSource;

    [Header("Reload Phase")] 
    private float HoldDownDuration;

    [Header("Shooting Phase")] 
    private bool HasLetGoOfJoystick;
    private bool HasMissedShot;
    private float ShootingFactor = 2f;

    [Header("Shooting Battle Phase")]
    [SerializeField] GameObject Bullet;
    private Transform RevolverNozzle;
    [SerializeField] private ParticleSystem Gunshot;

    private bool isCoroutineWaiting = false;
    
    
    public new void Start()
    {
        base.Start();
        Timer = GetComponent<Timer>();
        RevolverNozzle = GameObject.Find("RevolverNozzle").transform;
        RevolverCanvas.SetActive(false);

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
        Debug.Log("Starting Reload Phase");
        RevolverCanvas.SetActive(true);
        RevolverCanvas.transform.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        StartCoroutine(FireGun());
        CurrentPhase = RevolverPhase.Load;
        InputUIManager.Instance.SetRotatingInputUIButton(Combatant.HealthBarUI.GetComponentInChildren<PointsBar>(), true, "Pull");
        Timer.StartTimer(Random.Range(2f, ReloadDuration));
    }

    private void ReloadUpdate()
    {
        ShootingTimerText.text = "Get Ready...";
        float timeRemaining = ReloadDuration - Timer.GetProgress();
        ReloadTimerText.text = Mathf.RoundToInt(timeRemaining) + "";
        
        if(Timer.IsInProgress())
        {
            if (InputManager.InputDirection.z <= -0.98)
            {
                HoldDownDuration += Mathf.Sqrt(Time.deltaTime);
            }
        }

        if(Timer.IsFinished())
            EndReloadPhase();
    }

    private void EndReloadPhase()
    {
        Debug.Log("Ending Reload Phase");
        Timer.ResetTimer();

        Debug.Log($"Total time held down: {HoldDownDuration}");
        ReloadSource.Play();
        InputUIManager.Instance.SetRotatingInputUIButton(Combatant.HealthBarUI.GetComponentInChildren<PointsBar>(), false);
        StartShootingPhase();
    }

    private void StartShootingPhase()
    {
        Debug.Log("Starting Shooting Phase");

        CurrentPhase = RevolverPhase.Shoot;
        ShootingTimerText.color = Color.red;
        ShootingTimerText.fontStyle = FontStyle.Bold;
        Timer.StartTimer(ShootingDuration);
    }

    private void ShootUpdate()
    {
        if (Timer.IsInProgress())
        {
            ShootingTimerText.text = "Shoot!";
            // Maybe depending on how long you hold down, it will load a certain amount of bullets
            // Then you time your hold down joystick motion based on the number of bullets loaded?
            // Could be a future TODO
            if (!HasLetGoOfJoystick && InputManager.InputDirection.z == 0f)
            {
                HasLetGoOfJoystick = true;
                Timer.StopTimer();
                Debug.Log("Ending Shooting Phase Early");
                if (!isCoroutineWaiting)
                {
                    StartCoroutine(EndShootingPhase());
                }
            }
        }
        else
        {
            ShootingTimerText.color = Color.white;
            ShootingTimerText.text = "Miss...";
            Debug.Log("Ending Shooting Phase");
            HasLetGoOfJoystick = true;
            HasMissedShot = true;
            Timer.ResetTimer();
            if (!isCoroutineWaiting)
            {
                StartCoroutine(EndShootingPhase());
            }
        }

    }

    private IEnumerator EndShootingPhase()
    {
        isCoroutineWaiting = true;
        CurrentPhase = RevolverPhase.Inactive;
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Ending Ability");
        
        Timer.ResetTimer();
        ResetShootingValues();
        EndAbility();
        isCoroutineWaiting = false;
    }

    private void ResetShootingValues()
    {
        BulletsInClip = 0;
        HoldDownDuration = 0f;
        HasLetGoOfJoystick = false;
        HasMissedShot = false;
        ShootingTimerText.fontStyle = FontStyle.Normal;
        ShootingTimerText.color = Color.white;
    }

    private float CalculateRevolverDamage()
    {
        float b = BaseBulletDamage;
        float T = HoldDownDuration;
        float P = !HasMissedShot ? ShootingFactor : 0f;
        float B = BaseTotalDamage;

        TotalDamage = b + P * T + B;

        float randomTotalDamage = Random.Range(TotalDamage, TotalDamage + RandomDamageRangeOffset);
        
        return randomTotalDamage;
    }

    public void ShootBulletFromRevolver()
    {
        GameObject go = Instantiate(Bullet, RevolverNozzle.transform.position, RevolverNozzle.transform.rotation);
        var bullet = go.GetComponent<Bullet>();
        bullet.SetTarget(TargetedCombatants[0]);
        var offset = TargetedCombatants[0].GetComponent<Combatant>().isBoss
            ? BulletTargetHeightOffset * 4
            : BulletTargetHeightOffset;
        Vector3 direction = (TargetedCombatants[0].gameObject.transform.position + new Vector3(0f, offset, 0f) - RevolverNozzle.position).normalized;
        bullet.GetRigidBody().velocity = direction * bullet.GetSpeed();
        Gunshot.Play();
        ShootSource.Play();
    }
    
    public void DealRevolverDamage()
    {
        StartCoroutine(FinishRevolverDamage());
    }

    private IEnumerator FinishRevolverDamage()
    {
        CalculateRevolverDamage();
        int revolverdamage = (int) TotalDamage;
        Attack attack = new Attack(revolverdamage, Element, Style);
        TargetedCombatants[0].GetComponent<Combatant>().Defend(attack);

        yield return new WaitForSeconds(1.5f);

        CombatSystem.EndTurn();
    }

    private IEnumerator FireGun()
    {
        float animationTime = 0f;
        float animationDuration = ReloadDuration + ShootingDuration;
        Animator.SetBool("IsFinishedShooting", false);
        ReloadSource.Play();
        Animator.Play("Base Layer.Shoot");
        while (animationTime < animationDuration)
        {
            animationTime += Time.deltaTime;
            if (CurrentPhase != RevolverPhase.Shoot
                && animationTime >= 1.1f 
                && animationTime <= 1.25f)
            {
                Animator.speed = 0f;
            }
            else if(HasLetGoOfJoystick || HasMissedShot)
            {
                Animator.speed = 1f;
            }
            yield return null;
        }

        Animator.SetBool("IsFinishedShooting", true);
    }
    
    protected override void EndAbility()
    {
        RevolverCanvas.SetActive(false);
        Debug.Log($"Revolver Damage total: {TotalDamage}");
    }
}
