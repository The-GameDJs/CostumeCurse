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
    private int TotalBulletsDropped = 0;
    private int BulletsInClip = 0;
    private int TotalPimpkinsHit = 0;
    private bool[] IsBulletReserved;
    private bool[] IsPimpkinReserved;
    private DragAndDrop[] BulletUIInReload;
    private PimpkinHead[] Pimpkins;

    private enum RevolverPhase { Load, Shoot, Inactive }
    private RevolverPhase CurrentPhase = RevolverPhase.Inactive;
    private Stack<DragAndDrop> Bullets = new Stack<DragAndDrop>();
    private Text ReloadTimerText;
    private Text ShootingTimerText;

    private int CurrentDamage;
    private readonly float BaseDamage = 0f;



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


    public new void Start()
    {
        base.Start();
        Timer = GetComponent<Timer>();
        ReloadCanvas.gameObject.SetActive(false);
        ShootingCanvas.gameObject.SetActive(false);
        IsBulletReserved = new bool[BulletPositions.Length];
        IsPimpkinReserved = new bool[PimpkinSpawnLocations.Length];
        BulletUIInReload = ReloadCanvas.GetComponentsInChildren<DragAndDrop>();
        Pimpkins = ShootingCanvas.GetComponentsInChildren<PimpkinHead>(); // only gets the ones that you didnt hit


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
        ReloadCanvas.transform.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        foreach (DragAndDrop bullet in BulletUIInReload)
            bullet.gameObject.SetActive(true);

        SetBulletLocations(BulletUIInReload);

        ReloadCanvas.gameObject.SetActive(true);
        ReloadTimerText = GameObject.Find("ReloadTimerText").GetComponent<Text>();
        CurrentPhase = RevolverPhase.Load;
        Timer.StartTimer(ReloadDuration);
    }

    private void SetBulletLocations(DragAndDrop[] bullets)
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
        Timer.ResetTimer();
        ReloadCanvas.gameObject.SetActive(false);

        ResetReloadValues();

        Debug.Log($"Total bullets dropped: {TotalBulletsDropped}");
        ReloadSource.Play();
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
        ShootingTimerText = GameObject.Find("ShootingTimerText").GetComponent<Text>();

        for (int i = 0; i < TotalBulletsDropped; i++)
            BulletUIInShoot[i].SetActive(true);

        PimpkinHead[] pimpkins = ShootingCanvas.GetComponentsInChildren<PimpkinHead>();

        foreach (PimpkinHead pimpkin in pimpkins)
            PimpkinStack.Push(pimpkin);

        while (PimpkinStack.Count != 0)
        {
            int randomPosition = Random.Range(0, 6);

            if (!IsPimpkinReserved[randomPosition])
            {
                PimpkinHead pimpkin = PimpkinStack.Pop();
                pimpkin.gameObject.SetActive(true);
                
                pimpkin.gameObject.transform.position = PimpkinSpawnLocations[randomPosition].transform.position;
                IsPimpkinReserved[randomPosition] = true;
            }
        }

        CurrentPhase = RevolverPhase.Shoot;
        BulletsInClip = TotalBulletsDropped;
        Timer.StartTimer(ShootingDuration);
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
        // Reset Values
        Timer.ResetTimer();
        CurrentPhase = RevolverPhase.Inactive;
        TotalBulletsDropped = 0;
        for (int i = 0; i < BulletUIInShoot.Length; i++)
            BulletUIInShoot[i].SetActive(false);

        Debug.Log($"Total Pimpkins Heads: {Pimpkins.Length}");

        foreach (PimpkinHead pimpkin in Pimpkins)
        {
            if (pimpkin.GetHit())
                TotalPimpkinsHit++;

            pimpkin.ResetPimpkinValues();
        }

        ShootingCanvas.gameObject.SetActive(false);
        Debug.Log($"Total Pimpkins Hit: {TotalPimpkinsHit}");
        CalculateRevolverDamage();
        EndAbility();

    }

    private void CalculateRevolverDamage()
    {
            
    }

    private IEnumerator FireGun()
    {
        // TODO
        float animationTime = 0f;
        float animationDuration = 2f;
        Animator.Play("Base Layer.Shoot");

        yield return null;


        /*while (animationTime < animationDuration)
        {
            animationTime += Time.deltaTime;
        }*/


        // CombatSystem.EndTurn(this.GetComponentInParent<Combatant>().gameObject);
    }

    protected override void EndAbility()
    {
        StartCoroutine(FireGun());
        TotalPimpkinsHit = 0;
        Debug.Log($"Revolver Damage total: {CurrentDamage}");

        Attack attack = new Attack(CurrentDamage);
        TargetedCombatants[Random.Range(0, TargetedCombatants.Length)].GetComponent<Combatant>().Defend(attack);

        CombatSystem.EndTurn(this.GetComponentInParent<Combatant>().gameObject);
    }
}
