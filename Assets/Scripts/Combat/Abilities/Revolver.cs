using Assets.Scripts.Combat;
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
    private readonly float ReloadDuration = 5f;
    private readonly float ShootingDuration = 5f;
    private int BulletsInClip = 0;
    private bool[] IsBulletReserved;
    private bool[] IsPimpkinReserved;
    private DragAndDrop[] BulletUIInReload;
    private PimpkinHead[] Pimpkins;

    private enum RevolverPhase { Load, Shoot, Inactive }
    private RevolverPhase CurrentPhase = RevolverPhase.Inactive;
    private Stack<DragAndDrop> Bullets = new Stack<DragAndDrop>();
    private Text ReloadTimerText;
    private Text ShootingTimerText;

    private float TotalDamage;
    private readonly float BaseBulletDamage = 8f;
    private readonly float BaseTotalDamage = 20f;
    private readonly float MaxDamage = 80f;
    private readonly int RandomDamageRangeOffset = 5;
    private readonly float BulletTargetHeightOffset = 3.0f;
    private int TotalPimpkinsHit = 0;
    private int TotalBulletsDropped = 0;

    [SerializeField] AudioSource ReloadSource;
    [SerializeField] AudioSource ShootSource;

    [Header("Reload Phase")]
    [SerializeField] Canvas ReloadCanvas;
    [SerializeField] GameObject[] BulletPositions;
    [SerializeField] GameObject[] Clips;
   

    [Header("Shooting Phase")]
    [SerializeField] Canvas ShootingCanvas;
    [SerializeField] GameObject[] BulletUIInShoot;
    [SerializeField] GameObject[] PimpkinSpawnLocations;
    private Stack<PimpkinHead> PimpkinStack = new Stack<PimpkinHead>();
    [SerializeField] GameObject Bullet;
    private Transform RevolverNozzle;


    public new void Start()
    {
        base.Start();
        Timer = GetComponent<Timer>();
        ReloadCanvas.gameObject.SetActive(false);
        ShootingCanvas.gameObject.SetActive(false);
        IsBulletReserved = new bool[BulletPositions.Length];
        IsPimpkinReserved = new bool[PimpkinSpawnLocations.Length];
        BulletUIInReload = ReloadCanvas.GetComponentsInChildren<DragAndDrop>();
        Pimpkins = ShootingCanvas.GetComponentsInChildren<PimpkinHead>();
        RevolverNozzle = GameObject.Find("RevolverNozzle").transform;


        for (int i = 0; i < BulletUIInShoot.Length; i++)
            BulletUIInShoot[i].SetActive(false);

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
        PrepareReloadUI();
        CurrentPhase = RevolverPhase.Load;
        Timer.StartTimer(ReloadDuration);
    }

    private void PrepareReloadUI()
    {
        ReloadCanvas.transform.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        foreach (DragAndDrop bullet in BulletUIInReload)
            bullet.gameObject.SetActive(true);

        RandomizeBulletSpawnPositions(BulletUIInReload);
        TotalBulletsDropped = 0;
        ReloadCanvas.gameObject.SetActive(true);
        ReloadTimerText = GameObject.Find("ReloadTimerText").GetComponent<Text>();
    }

    private void RandomizeBulletSpawnPositions(DragAndDrop[] bullets)
    {
        foreach (DragAndDrop bullet in bullets)
            Bullets.Push(bullet);

        while (Bullets.Count != 0)
        {
            int randomPosition = Random.Range(0, 8);

            if (!IsBulletReserved[randomPosition])
            {
                DragAndDrop bullet = Bullets.Pop();
                bullet.ResetPosition();
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
        Debug.Log("Ending Reload Phase");
        Timer.ResetTimer();
        ReloadCanvas.gameObject.SetActive(false);

        ResetReloadValues();

        Debug.Log($"Total bullets dropped: {TotalBulletsDropped}");
        ReloadSource.Play();
        StartShootingPhase();
    }

    private void ResetReloadValues()
    {
        foreach (GameObject c in Clips)
        {
            Clip singleClip = c.GetComponent<Clip>();

            if (singleClip.IsClipFilled())
                TotalBulletsDropped++;

            singleClip.ResetRevolverValues();
        }

        for (int i = 0; i < IsBulletReserved.Length; i++)
            IsBulletReserved[i] = false;
    }

    private void StartShootingPhase()
    {
        Debug.Log("Starting Shooting Phase");
        PrepareShootingUI();

        CurrentPhase = RevolverPhase.Shoot;
        TotalPimpkinsHit = 0;
        BulletsInClip = TotalBulletsDropped;
        Timer.StartTimer(ShootingDuration);
    }

    private void PrepareShootingUI()
    {
        ShootingCanvas.gameObject.SetActive(true);
        ShootingCanvas.transform.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        ShootingTimerText = GameObject.Find("ShootingTimerText").GetComponent<Text>();

        for (int i = 0; i < TotalBulletsDropped; i++)
            BulletUIInShoot[i].SetActive(true);

        RandomizePimpkinHeadSpawns();
    }

    private void RandomizePimpkinHeadSpawns()
    {
        foreach (PimpkinHead pimpkin in Pimpkins)
            PimpkinStack.Push(pimpkin);

        while (PimpkinStack.Count != 0)
        {
            int randomPosition = Random.Range(0, 6);

            if (!IsPimpkinReserved[randomPosition])
            {
                PimpkinHead pimpkin = PimpkinStack.Pop();
                Debug.Log($"Pimpkin Popped: {PimpkinStack.Count} left");
                pimpkin.gameObject.SetActive(true);

                pimpkin.gameObject.transform.position = PimpkinSpawnLocations[randomPosition].transform.position;
                IsPimpkinReserved[randomPosition] = true;
            }
        }
    }

    private void ShootUpdate()
    {
        if (BulletsInClip > 0 && Timer.IsInProgress())
        {
            float timeRemaining = ReloadDuration - Timer.GetProgress();
            ShootingTimerText.text = Mathf.RoundToInt(timeRemaining) + "";

            if (Input.GetButtonDown("Action Command"))
            {
                BulletUIInShoot[BulletsInClip - 1].SetActive(false);
                BulletsInClip--;
                ShootSource.Play();
            }
        }

        else
        {
            Debug.Log("Ending Shooting Phase");
            EndShootingPhase();
        }

    }

    private void EndShootingPhase()
    {
        Debug.Log("Ending Ability");

        Timer.ResetTimer();
        ResetShootingValues();
        Debug.Log($"Total Pimpkins Hit: {TotalPimpkinsHit}");
        CalculateRevolverDamage();
        EndAbility();
    }

    private void ResetShootingValues()
    {
        CurrentPhase = RevolverPhase.Inactive;
        BulletsInClip = 0;

        for (int i = 0; i < BulletUIInShoot.Length; i++)
        {
            BulletUIInShoot[i].SetActive(false);
            IsPimpkinReserved[i] = false;
        }

        Debug.Log($"Total Pimpkins Heads: {Pimpkins.Length}");

        foreach (PimpkinHead pimpkin in Pimpkins)
        {
            if (pimpkin.GetHit())
                TotalPimpkinsHit++;

            pimpkin.ResetPimpkinValues();
        }

    }

    private float CalculateRevolverDamage()
    {
        float b = BaseBulletDamage;
        float T = TotalBulletsDropped;
        float P = TotalPimpkinsHit;
        float B = BaseTotalDamage;

        TotalDamage = b * T + P * b + B;

        float randomTotalDamage = Random.Range(TotalDamage, TotalDamage + RandomDamageRangeOffset);

        return randomTotalDamage;
    }

    public void ShootBulletFromRevolver()
    {
        GameObject go = Instantiate(Bullet, RevolverNozzle.transform.position, RevolverNozzle.transform.rotation);
        var bullet = go.GetComponent<Bullet>();
        bullet.SetTarget(TargetedCombatants[0]);
        Vector3 direction = (TargetedCombatants[0].transform.position + new Vector3(0f, BulletTargetHeightOffset, 0f) - RevolverNozzle.position).normalized;
        bullet.GetRigidBody().velocity = direction * bullet.GetSpeed();
    }


    public void DealRevolverDamage()
    {
        StartCoroutine(FinishRevolverDamage());
    }

    private IEnumerator FinishRevolverDamage()
    {
        int revolverdamage = (int) CalculateRevolverDamage();
        Attack attack = new Attack(revolverdamage);
        TargetedCombatants[0].GetComponent<Combatant>().Defend(attack);

        yield return new WaitForSeconds(1.0f);

        CombatSystem.EndTurn(this.GetComponentInParent<Combatant>().gameObject);
    }

    private IEnumerator FireGun()
    {
        float animationTime = 0f;
        float animationDuration = 2.5f;
        Animator.SetBool("IsFinishedShooting", false);
        ShootingCanvas.gameObject.SetActive(false);

        while (animationTime < animationDuration)
        {
            animationTime += Time.deltaTime;
            Animator.Play("Base Layer.Shoot");
            yield return null;
        }

        Animator.SetBool("IsFinishedShooting", true);
    }

    protected override void EndAbility()
    {
        TotalPimpkinsHit = 0;
        Debug.Log($"Revolver Damage total: {TotalDamage}");

        
        StartCoroutine(FireGun());
    }
}
