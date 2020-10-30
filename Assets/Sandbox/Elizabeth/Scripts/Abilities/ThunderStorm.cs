using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderStorm : Ability
{
    enum PHASE { THUNDERCLOUD, THUNDERSTRIKE, INACTIVE}

    // This holds the thunderstrike/thunderstorm prefab
    [SerializeField] GameObject thunder;
    Timer timer;

    int numberOfThunderStrikes = 3;
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

    PHASE phase = PHASE.INACTIVE;
    // Temporary for testing purposes
    Vector3 scaleChange = new Vector3(0.01f, 0.01f, 0.01f);

    // TODO: Implement damange dealing to enemies
    
    void Start()
    {
        timer = GetComponent<Timer>();
    }

    void Update()
    {
        if (phase == PHASE.THUNDERCLOUD)
        {
            if(timer.GetTimerState())
            {
                if(Input.GetKeyDown(KeyCode.Z))
                {
                    pressCounter++;
                }
            }
            else
            {
                Debug.Log(pressCounter);
                Debug.Log(CalculateBaseDamage());
                completedThunderStrikes = 0;
                //Start next phase after this phase is completed
                StartThunderStrike();
            }
        }

        else if (phase == PHASE.THUNDERSTRIKE)
        {
            if(timer.GetTimerState())
            {
                // Temporary for testing purposes
                // TODO: Change to final animation
                if(timer.GetCurrentTime() <= timeWindowForStrikes / 2.0f + perfectStrikeTimeWindow)
                {
                    thunder.transform.localScale += scaleChange;
                }
                else
                {
                    thunder.transform.localScale -= scaleChange;
                }

                if(Input.GetKeyDown(KeyCode.X))
                {
                    Debug.Log("X pressed");
                    timer.StopTimer();
                    // TODO: At this point add calculated damage and bonus damage
                }
            }
            else
            {
                EvaluateThunderStrikeInput(timer.GetCurrentTime());
                phase = PHASE.INACTIVE;
                completedThunderStrikes++;
                // Start next thunderstrike
                if(completedThunderStrikes < numberOfThunderStrikes)
                {
                    thunder.transform.localScale = new Vector3(1,1,1);
                    StartThunderStrike();
                }
            }
        }
        // TODO: Implement "break"
    }

    public override void UseAbility()
    {   
        pressCounter = 0;
        phase = PHASE.THUNDERCLOUD;
        timer.StartTimer(maxTimeOfInput);
        Debug.Log("Thundercloud phase start");
    }

    int CalculateBaseDamage()
    {
        int damage = 0;
        damage = (int) (pressCounter / 100.0f * ((maxDamage-minDamage)+1) + minDamage);
        return damage;
    }

    // TODO: Implement bonus damage calculation

    void StartThunderStrike()
    {
        phase = PHASE.THUNDERSTRIKE;
        Debug.Log("Thunderbolt phase start");
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
