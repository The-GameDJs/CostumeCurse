using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum TimerState { InProgress, Finished, NotStarted }

public class Timer : MonoBehaviour
{
    float progress;
    float duration;
    private TimerState timerState;

    void Start()
    {
        timerState = TimerState.NotStarted;
    }

    void Update()
    {
        if (timerState != TimerState.InProgress)
            return;
        
        if (progress < duration)
            IncrementTimer();
        else
            StopTimer();
    }

    private void IncrementTimer()
    {
        progress += Time.deltaTime;
    }

    public void StartTimer(float duration)
    {
        this.duration = duration;
        progress = 0;
        timerState = TimerState.InProgress;
    }

    public void StopTimer()
    {
        timerState = TimerState.Finished;
    }
    
    public void ResetTimer()
    {
        timerState = TimerState.NotStarted;
    }

    public float GetProgress()
    {
        return progress;
    }

    public bool IsInProgress()
    {
        return timerState == TimerState.InProgress;
    }
    
    public bool IsFinished()
    {
        return timerState == TimerState.Finished;
    }
}
