using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PulsatingFont : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float MinFontSize;
    [SerializeField] private float MaxFontSize;
    [SerializeField] private float GrowthSpeed;

    private bool isGrowing;
    private float currentFontSize;

    private void Start()
    {
        currentFontSize = MinFontSize;
        text.fontSize = MinFontSize;
        isGrowing = true;
    }

    void Update()
    {
        if (isGrowing)
        {
            currentFontSize += GrowthSpeed * Time.deltaTime;
        }
        else
        {
            currentFontSize -= GrowthSpeed * Time.deltaTime;
        }

        if (currentFontSize >= MaxFontSize)
        {
            currentFontSize = MaxFontSize;
            isGrowing = false;
        }
        else if (currentFontSize <= MinFontSize)
        {
            isGrowing = true;
            currentFontSize = MinFontSize;
        }

        text.fontSize = currentFontSize;
    }
}
