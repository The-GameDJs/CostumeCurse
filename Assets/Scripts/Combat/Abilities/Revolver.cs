using Assets.Scripts.Combat;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Combat.Abilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Revolver : Ability
{
    private Timer Timer;
    private readonly float ReloadDuration = 5f;
    private readonly float ShootingDuration = 3f;
    private int BulletsInClip = 0;
    private bool[] IsBulletReserved;
    private bool[] IsPimpkinReserved;
    private DragAndDrop[] BulletUIInReload;
    private PimpkinHead[] Pimpkins;

    private enum RevolverPhase { Load, Shoot, Inactive }
    private RevolverPhase CurrentPhase = RevolverPhase.Inactive;
    private readonly Stack<DragAndDrop> Bullets = new Stack<DragAndDrop>();
    private Text ReloadTimerText;
    private Text ShootingTimerText;

    private float TotalDamage;
    private readonly float BaseBulletDamage = 10f;
    private readonly float BaseTotalDamage = 10f;
    private readonly float MaxDamage = 80f;
    private readonly int RandomDamageRangeOffset = 10;
    private readonly float BulletTargetHeightOffset = 3.0f;
    private int TotalPimpkinsHit = 0;
    private int TotalBulletsDropped = 0;

    [SerializeField] AudioSource ReloadSource;
    [SerializeField] AudioSource ShootSource;
    [SerializeField] AudioSource MissSource;

    [Header("Reload Phase")]
    [SerializeField] Canvas ReloadCanvas;
    [SerializeField] GameObject[] BulletPositions;
    [SerializeField] GameObject[] Clips;
   

    [Header("Shooting Pimpkin Phase")]
    [SerializeField] Canvas ShootingCanvas;
    [SerializeField] GameObject[] BulletUIInShoot;
    [SerializeField] GameObject[] PimpkinSpawnLocations;
    private readonly Stack<PimpkinHead> PimpkinStack = new Stack<PimpkinHead>();

    [Header("Shooting Battle Phase")]
    [SerializeField] GameObject Bullet;
    private Transform RevolverNozzle;
    [SerializeField] private ParticleSystem Gunshot;

    private bool isCoroutineWaiting = false;


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
                CheckForPimpkinHeadClick();
            }
        }
        else
        {
            Debug.Log("Ending Shooting Phase");
            if (!isCoroutineWaiting)
            {
                StartCoroutine(EndShootingPhase());
            }
        }

    }

    private IEnumerator EndShootingPhase()
    {
        isCoroutineWaiting = true;
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Ending Ability");

        Timer.ResetTimer();
        ResetShootingValues();
        Debug.Log($"Total Pimpkins Hit: {TotalPimpkinsHit}");
        CalculateRevolverDamage();
        EndAbility();
        isCoroutineWaiting = false;
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
        int revolverdamage = (int) TotalDamage;
        Attack attack = new Attack(revolverdamage, Element, Style);
        TargetedCombatants[0].GetComponent<Combatant>().Defend(attack);

        yield return new WaitForSeconds(1.5f);

        CombatSystem.EndTurn(this.GetComponentInParent<Combatant>().gameObject);
    }

    private IEnumerator FireGun()
    {
        float animationTime = 0f;
        float animationDuration = 2.5f;
        Animator.SetBool("IsFinishedShooting", false);
        ShootingCanvas.gameObject.SetActive(false);
        ReloadSource.Play();

        while (animationTime < animationDuration)
        {
            animationTime += Time.deltaTime;
            Animator.Play("Base Layer.Shoot");
            yield return null;
        }

        Animator.SetBool("IsFinishedShooting", true);
    }

    private void CheckForPimpkinHeadClick()
    {
        var eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        // Create a list to store raycast results.
        var raycastResults = new List<RaycastResult>();

        // Raycast into the UI system.
        EventSystem.current.RaycastAll(eventData, raycastResults);

        PimpkinHead head = null;
        // Check if any UI button elements of type PimpkinHead were hit.
        foreach (var result in raycastResults.Where(result => result.gameObject.GetComponentInParent<Button>() && result.gameObject.GetComponentInParent<PimpkinHead>()))
        {
            head = result.gameObject.GetComponentInParent<PimpkinHead>();
            Debug.Log("Pimpkin Head Hit! " + result.gameObject.name);
        }

        if (!head) StartCoroutine(StartPlayingMissBulletSound());
    }

    private IEnumerator StartPlayingMissBulletSound()
    {
        yield return new WaitForSeconds(0.3f);
        MissSource.Play();
    }
    
    protected override void EndAbility()
    {
        TotalPimpkinsHit = 0;
        Debug.Log($"Revolver Damage total: {TotalDamage}");

        
        StartCoroutine(FireGun());
    }
}
