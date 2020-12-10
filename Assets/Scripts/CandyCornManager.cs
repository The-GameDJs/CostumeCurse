using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyCornManager : MonoBehaviour
{
    private int TotalCandyCorn;

    void Start()
    {
        TotalCandyCorn = 0;
    }

    void Update()
    {
        
    }

    public int GetTotalCandyCorn()
    {
        return TotalCandyCorn;
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
