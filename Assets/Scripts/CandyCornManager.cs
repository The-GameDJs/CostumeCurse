using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CandyCornManager : MonoBehaviour
{
    [SerializeField] private int TotalCandyCorn;
    private TMP_Text CandyCornValue;

    void Start()
    {
        CandyCornValue = GameObject.Find("CandyCornValue").GetComponent<TMP_Text>();
    }

    void Update()
    {
        CandyCornValue.text = TotalCandyCorn.ToString("0000");
    }

    public int GetTotalCandyCorn()
    {
        return TotalCandyCorn;
    }

    public void SetCandyCorn(int candyCorn)
    {
        Debug.Log($"Set Candy Corn! {candyCorn}");
        if (candyCorn >= 0)
            TotalCandyCorn = candyCorn;
        else
            Debug.LogWarning("Hey buddy, use RemoveCandyCorn if you want to remove");
    }

    public void AddCandyCorn(int candyCorn)
    {
        Debug.Log($"Adding Candy Corn! {candyCorn}");
        if (candyCorn >= 0)
            TotalCandyCorn += candyCorn;
        else
            Debug.LogWarning("Hey buddy, use RemoveCandyCorn if you want to remove");
    }

    public void RemoveCandyCorn(int candyCorn)
    {
        if (candyCorn >= 0)
            TotalCandyCorn -= candyCorn;
        else
            Debug.LogWarning("Hey buddy, candyCorn passed here should be positive");
    }
}
