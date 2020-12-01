﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderHandle : MonoBehaviour
{
    [SerializeField] private GameObject Slider;
    [SerializeField] private GameObject HitArea;
    [SerializeField] private GameObject StartPosition;
    [SerializeField] private GameObject EndPosition;

    private RectTransform HitTransform;

    private BoxCollider2D SliderCollider;
    private BoxCollider2D GoodCollider;
    private BoxCollider2D PerfectCollider;

    private float SliderSpeed = 300.0f;
    private int Clicks;
    private int MaxClicks = 3;
    private int GoodClicks;
    private int PerfectClicks;
    private bool Arrived = false;

    private enum Click { None, Miss, Good, Perfect } 
    private Click ClickPhase = Click.None;

    private void Start()
    {
        HitTransform = HitArea.GetComponent<RectTransform>();
        SliderCollider = Slider.GetComponent<BoxCollider2D>();
        GoodCollider = HitArea.GetComponent<BoxCollider2D>();
        PerfectCollider = HitArea.transform.GetChild(0).GetComponent<BoxCollider2D>();
    }

    public void StartSlider()
    {
        RandomizeHitAreaPosition();
    }

    private void Update()
    {
        if(Clicks != MaxClicks)
        {
            if(!Arrived)
                Slider.transform.localPosition = Vector3.MoveTowards(Slider.transform.localPosition, EndPosition.transform.localPosition, Time.deltaTime * SliderSpeed); // L to R
            else
                Slider.transform.localPosition = Vector3.MoveTowards(Slider.transform.localPosition, StartPosition.transform.localPosition, Time.deltaTime * SliderSpeed); // R to L

            if(Input.GetMouseButtonDown(0))
            {
                if(SliderCollider.IsTouching(PerfectCollider))
                {
                    ClickPhase = Click.Perfect;
                    PerfectClicks++;
                    Debug.Log("Perfect Hit");
                }
                else if(SliderCollider.IsTouching(GoodCollider))
                {
                    ClickPhase = Click.Good;
                    GoodClicks++;
                    Debug.Log("Good Hit");
                }
                else
                {
                    ClickPhase = Click.Miss;
                    Debug.Log("Missed Hit");
                }

                Clicks++;
                RandomizeHitAreaPosition();
            }

            CheckArrived();
        }
        else
        {
            Clicks = 0; // Will not call update again because it is stopped in Confection.cs
            GoodClicks = 0;
            PerfectClicks = 0;
        }
    }

    private void CheckArrived()
    {
        if(Slider.transform.localPosition == EndPosition.transform.localPosition)
            Arrived = true;

        if(Slider.transform.localPosition == StartPosition.transform.localPosition)
           Arrived = false;
    }

    private void RandomizeHitAreaPosition()
    {
        Vector2 v = new Vector2(HitTransform.rect.xMin, HitTransform.rect.xMax); // v[0] = min | v[1] = max
        float randomX = Random.Range(StartPosition.transform.localPosition.x + v[1], EndPosition.transform.localPosition.x + v[0]);
        HitArea.transform.localPosition = new Vector3(randomX, Slider.transform.localPosition.y, 0.0f);
    }

    public int GetGoodClicks()
    {
        return GoodClicks;
    }

    public int GetPerfectClicks()
    {
        return PerfectClicks;
    }

    public int GetTotalClicks()
    {
        return Clicks;
    }

    public int GetMaxClicks()
    {
        return MaxClicks;
    }
}
