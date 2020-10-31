using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderStorm : Ability
{
    enum Phase { Cloud, Strike, Inactive}

    // This holds the thunderstrike/thunderstorm prefab
    [SerializeField] GameObject thunder;
    Timer timer;

    int totalThunderStrikes = 3;
    int completedThunderStrikes = 0;

    float timeWindowForStrikes = 3.0f;
    float goodStrikeTimeWindow = 0.5f;
    float perfectStrikeTimeWindow = 0.2f;
    // TODO: If we want to implement a "break" between thunderstrikes
    float strikeTimeInterval = 2.0f;

    int pressCounter;
    float maxTimeOfInput = 5.0f;
    int minDamage = 10;
    int maxDamage = 30;

    Phase phase = Phase.Inactive;
    // Temporary for testing purposes
    Vector3 scaleChange = new Vector3(0.01f, 0.01f, 0.01f);

    // TODO: Implement damange dealing to enemies
    
    void Start()
    {
        timer = GetComponent<Timer>();
    }

    void Update()
    {
        if (phase == Phase.Cloud)
            ThundercloudUpdate();

        if (phase == Phase.Strike)
            ThunderstrikeUpdate();
        
        // TODO: Implement "break"
    }

    private void ThunderstrikeUpdate()
    {
        if (timer.IsInProgress())
        {
            // Temporary for testing purposes
            // TODO: Change to final animation           
            if (timer.GetProgress() <= timeWindowForStrikes / 2.0f + perfectStrikeTimeWindow)
                thunder.transform.localScale += scaleChange;
            else
                thunder.transform.localScale -= scaleChange;

            if (Input.GetKeyDown(KeyCode.X))
            {
                Debug.Log("X pressed during Thunderstrike Phase");
                
                timer.StopTimer();
                
                // TODO: At this point add calculated damage and bonus damage
            }
        }
        
        if (timer.IsFinished())
        {
            EvaluateThunderStrikeInput(timer.GetProgress());
            phase = Phase.Inactive;
            completedThunderStrikes++;
            
            if (completedThunderStrikes < totalThunderStrikes)
                NewThunderStrike();
        }
    }

    private void ThundercloudUpdate()
    {
        if (timer.IsInProgress())
            ThundercloudMash();
        else
        {
            Debug.Log($"Thundercloud Complete with presses: {pressCounter}");
            
            int baseDamage = CalculateBaseDamage();
            Debug.Log($"Base damage: {baseDamage}");

            StartThunderStrikePhase();
        }
    }

    private void ThundercloudMash()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            pressCounter++;
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

    int CalculateBaseDamage()
    {
        int damage = (int) (pressCounter / 100.0f * ((maxDamage-minDamage)+1) + minDamage);
        // TODO: Implement bonus damage calculation

        return damage;
    }


    void StartThunderStrikePhase()
    {
        Debug.Log("Thunderbolt phase start");

        phase = Phase.Strike;
        completedThunderStrikes = 0;
        
        NewThunderStrike();
    }

    private void NewThunderStrike()
    {
        completedThunderStrikes++;
        Debug.Log($"New thunderstrike, number {completedThunderStrikes}");

        thunder.transform.localScale = Vector3.one;
        timer.StartTimer(timeWindowForStrikes);
    }

    // TODO: Implement user feedback
    void EvaluateThunderStrikeInput(float timerValue)
    {
        Debug.Log("Timer Value: " + timerValue);
    
        if (timerValue >= (timeWindowForStrikes / 2.0f - perfectStrikeTimeWindow) &&
            timerValue <= (timeWindowForStrikes / 2.0f + perfectStrikeTimeWindow))
        {
            Debug.Log("Perfect Strike");
        }

        else if (timerValue >= (timeWindowForStrikes + goodStrikeTimeWindow) &&
                timerValue <= (timeWindowForStrikes - goodStrikeTimeWindow))
        {
            Debug.Log("Good Strike");
        }

        else
        {
            Debug.Log("Missed Strike");
        }
    }
}
