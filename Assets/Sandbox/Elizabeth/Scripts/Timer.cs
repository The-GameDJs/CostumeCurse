using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    float timer;
    float maxTime;
    bool isStarted;

    void Start()
    {
        isStarted = false;
    }

    void Update()
    {
        if(isStarted && timer < maxTime)
        {
            timer += Time.deltaTime;
        }
        else
        {
            StopTimer();
        }
    }

    public void StartTimer(float max)
    {
        maxTime = max;
        timer = 0;
        isStarted = true;
    }

    public void StopTimer()
    {
        isStarted = false;
    }

    public float GetCurrentTime()
    {
        return timer;
    }

    public bool GetTimerState()
    {
        return isStarted;
    }
}
