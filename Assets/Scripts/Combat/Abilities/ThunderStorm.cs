using Assets.Scripts.Combat;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ThunderStorm : Ability
{
    private enum ThunderstormPhase { Cloud, Strike, Inactive }

    [SerializeField] private GameObject Lightning;
    private static GameObject Thunder;
    private Timer Timer;

    private readonly int TotalThunderStrikes = 3;
    private int CurrentThunderStrike = 0;

    [Header("Strike Parameters")]
    private readonly float TimeWindowForStrikes = 3.0f;
    private readonly float GoodStrikeTimeWindow = 1.0f;
    private readonly float PerfectStrikeTimeWindow = 0.7f;
    private readonly float StrikeTimeInterval = 2.0f;
    private bool ThunderHasAppeared;
    private float MomentOfActionCommand;
    private bool ActionCommandPressed;

    private float StartOfStrike;

    [Header("Damage Parameters")]
    private int Presses;
    private readonly float ThunderStormScale = 0.04f;
    private readonly float ThunderStormHeight = 8f;
    private readonly float ThunderCloudDuration = 5.0f;
    private readonly int ThunderCloudMinimumDamage = 15;
    private readonly int ThunderCloudMaximumDamage = 30;
    private readonly int ThunderCloudDifficultyCurve = 10;
    private readonly int ThunderStrikePerfectDamageBonus = 40;
    private readonly int ThunderStrikeGoodDamageBonus = 20;

    private ThunderstormPhase CurrentPhase = ThunderstormPhase.Inactive;
    private GameObject CurrentVictim;

    private readonly float ThunderCloudGrowthSpeed = 0.02f;
    private readonly float ThunderStrikeGrowthSpeed = 1.0f;

    [Header("Components")]
    private ParticleSystem ParticleComponent;
    private ParticleSystem.MainModule MainModule;
    private float InitialCloudSize;

    [SerializeField] private AudioSource LightningStrikeSound;
    [SerializeField] private AudioSource ThunderCloudSound;

    public new void Start()
    {
        base.Start();
        Thunder = GameObject.Find("Thunderstorm");
        Timer = GetComponent<Timer>();
        ParticleComponent = Thunder.GetComponent<ParticleSystem>();
        MainModule = ParticleComponent.main;
        InitialCloudSize = ParticleComponent.transform.localScale.magnitude;
        
        Thunder.SetActive(false);

        TargetSchema = new TargetSchema(
            0,
            CombatantType.Enemy,
            SelectorType.All);
    }

    public new void StartAbility(bool userTargeting = false)
    {
        Presses = 0;
        Animator.SetBool("IsFinishedCasting", false);

        base.StartAbility();
    }

    private void Update()
    {
        if (CurrentPhase == ThunderstormPhase.Cloud)
            ThunderCloudUpdate();

        if (CurrentPhase == ThunderstormPhase.Strike)
            ThunderstrikeUpdate();
    }

    private void ThunderstrikeUpdate()
    {
        if (Timer.IsInProgress())
        {
            if (WithinPerfectStrikeWindow(Timer.GetProgress()))
                Debug.Log($"currently WithinPerfectStrikeWindow");
            else if (WithinGoodStrikeWindow(Timer.GetProgress()))
                Debug.Log($"currently WithinGoodStrikeWindow");

            float progress = Timer.GetProgress();

            AnimateThunderstrike(progress); // Keep handy for debugging with colours!

            if (!ActionCommandPressed && Input.GetButtonDown("Action Command"))
            {
                Debug.Log("Action Command pressed during Thunderstrike Phase");
                MomentOfActionCommand = Timer.GetProgress();
                ActionCommandPressed = true;

                if (WithinPerfectStrikeWindow(Timer.GetProgress()))
                    PerfectActionCommandSound.Play();
                else if (WithinGoodStrikeWindow(Timer.GetProgress()))
                    GoodActionCommandSound.Play();
                else
                    MissedActionCommandSound.Play();
            }

            if (!ThunderHasAppeared && Timer.GetProgress() >= TimeWindowForStrikes / 2)
            {
                SpawnThunderBolt();
                StartCoroutine(DamageVictim(GoodStrikeTimeWindow / 2));
            }
        }

        if (Timer.IsFinished())
        {
            CurrentPhase = ThunderstormPhase.Inactive;

            var possibleVictims = Array.FindAll(TargetedCombatants, combatant => combatant.GetComponent<Combatant>().IsAlive).Length;
            if (CurrentThunderStrike < TotalThunderStrikes && possibleVictims > 0)
                Invoke(nameof(NewThunderStrike), Random.Range(StrikeTimeInterval, 1.5f * StrikeTimeInterval));
            else
                EndAbility();
        }
    }

    private void SpawnThunderBolt()
    {
        StartOfStrike = Time.time;
        //Time.timeScale = 0.01f;
        Instantiate(Lightning, Thunder.transform.position, Thunder.transform.rotation * Quaternion.Euler(180.0f, 0.0f, 0.0f));
        ThunderHasAppeared = true;
        LightningStrikeSound.Play();
    }

    private void AnimateThunderstrike(float progress)
    {        
        if (progress <= TimeWindowForStrikes / 2.0f)
            Thunder.transform.localScale += Vector3.one * ThunderStrikeGrowthSpeed * ThunderStormScale * Time.deltaTime;
        else
            Thunder.transform.localScale -= Vector3.one * ThunderStrikeGrowthSpeed * ThunderStormScale * Time.deltaTime;


        if (WithinPerfectStrikeWindow(progress))
            MainModule.startColor = Color.white;
        else if (WithinGoodStrikeWindow(progress))
            MainModule.startColor = Color.gray;
        else
            MainModule.startColor = Color.gray;
    }

    protected override void EndAbility()
    {
        Thunder.SetActive(false);

        Animator.SetBool("IsFinishedCasting", true);

        CombatSystem.EndTurn(this.GetComponentInParent<Combatant>().gameObject);
    }

    private void ThunderCloudUpdate()
    {
        if (Timer.IsInProgress())
        {
            ThunderCloudMash();
        }
        else
        {
            EndThunderCloudPhase();
        }
    }

    private void EndThunderCloudPhase()
    {
        Debug.Log($"Thundercloud Complete with presses: {Presses}");

        Thunder.SetActive(false);
        ThunderCloudSound.Stop();

        StartThunderStrikePhase();
    }

    private void ThunderCloudMash() // BUILD UP CLOUD MASH WITH CLICK
    {
        if (Input.GetButtonDown("Action Command"))
        {
            Presses++;
            Thunder.transform.localScale += Vector3.one * ThunderCloudGrowthSpeed * ThunderStormScale;

            if (!ThunderCloudSound.isPlaying)
                ThunderCloudSound.Play();

            ThunderCloudSound.volume = Mathf.Clamp(Presses / 50f, 0.01f, 1f);
        }
    }

    protected override void ContinueAbilityAfterTargeting()
    {
        StartThunderCloudPhase();
    }

    private void StartThunderCloudPhase()
    {
        Presses = 0;
        CurrentPhase = ThunderstormPhase.Cloud;

        Thunder.SetActive(true);
        Animator.Play("Base Layer.Casting");
        
        Thunder.transform.position = transform.position + ThunderStormHeight * Vector3.up;
        Thunder.transform.localScale = Vector3.one * ThunderStormScale;
        Thunder.GetComponent<Renderer>().material.color = Color.white;

        Timer.StartTimer(ThunderCloudDuration);
    }

    private float CalculateThunderCloudDamage()
    {
        // Using variables from https://www.desmos.com/calculator/km7jlgm5ws
        float M = ThunderCloudMaximumDamage;
        float m = ThunderCloudMinimumDamage;
        float d = ThunderCloudDifficultyCurve;
        float p = Presses;

        // Please refer to https://www.desmos.com/calculator/km7jlgm5ws for curve
        float thunderCloudDamage = (M - m) / (Mathf.PI / 2) * Mathf.Atan(p / d) + m;

        return thunderCloudDamage;
    }


    private void StartThunderStrikePhase()
    {
        CurrentPhase = ThunderstormPhase.Inactive;

        CurrentThunderStrike = 0;

        Invoke(nameof(NewThunderStrike), UnityEngine.Random.Range(StrikeTimeInterval, 1.5f * StrikeTimeInterval));

    }

    private void NewThunderStrike()
    {
        CurrentThunderStrike++;
        ThunderHasAppeared = false;
        ActionCommandPressed = false;
        Debug.Log($"New thunderstrike, currently on # {CurrentThunderStrike}");

        Thunder.SetActive(true);
        Thunder.transform.localScale = Vector3.one * ThunderStormScale;

        var possibleVictims = Array.FindAll(TargetedCombatants, combatant => combatant.GetComponent<Combatant>().IsAlive);

        CurrentVictim = possibleVictims.Length > 0 ?
            possibleVictims[Random.Range(0, possibleVictims.Length)] :
            TargetedCombatants[Random.Range(0, possibleVictims.Length)];

        var heightMultiplier = 1.0f;
        // Set Thunderstorm a little higher for boss, since he's much taller
        if (CurrentVictim 
            && CurrentVictim.TryGetComponent<Combatant>(out var combatantVictim)
            && combatantVictim.isBoss)
        {
            heightMultiplier = 3.0f;
        }
        Thunder.transform.position = CurrentVictim.transform.position + ThunderStormHeight * heightMultiplier * Vector3.up;

        CurrentPhase = ThunderstormPhase.Strike;
        Timer.StartTimer(TimeWindowForStrikes); // 10 seconds , at 5 seconds, that is when the stirke appears and is also the perfect frame window. 
    }

    // TODO: Implement user feedback
    private float EvaluateThunderStrikeInput()
    {
        var damage = CalculateThunderCloudDamage();

        if (WithinPerfectStrikeWindow(MomentOfActionCommand))
        {
            Debug.Log("Perfect Strike");
            damage += ThunderStrikePerfectDamageBonus;
        }
        else if (WithinGoodStrikeWindow(MomentOfActionCommand))
        {
            Debug.Log("Good Strike");
            damage += ThunderStrikeGoodDamageBonus;
        }
        else
        {
            Debug.Log("Missed Strike");
        }

        MomentOfActionCommand = 0.0f;
        return damage;
    }

    private bool WithinGoodStrikeWindow(float timerValue)
    {
        return timerValue >= ((TimeWindowForStrikes - GoodStrikeTimeWindow) / 2.0f) &&
                    timerValue <= ((TimeWindowForStrikes + GoodStrikeTimeWindow) / 2.0f);
    }

    private bool WithinPerfectStrikeWindow(float timerValue)
    {
        return timerValue >= ((TimeWindowForStrikes - PerfectStrikeTimeWindow) / 2.0f) &&
                    timerValue <= ((TimeWindowForStrikes + PerfectStrikeTimeWindow) / 2.0f);
    }

    private IEnumerator DamageVictim(float secondsToWait)
    {
        yield return new WaitForSeconds(secondsToWait);

        Thunder.SetActive(false);
        ParticleComponent.transform.localScale = Vector3.one * InitialCloudSize;
        Attack attack = new Attack((int)EvaluateThunderStrikeInput());
        CurrentVictim.GetComponent<Combatant>().Defend(attack);

        yield return new WaitForSeconds(1.0f);
    }
}
