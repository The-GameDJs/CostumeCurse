using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CandyCornManager : MonoBehaviour
{
    [SerializeField] private int TotalCandyCorn;
    [SerializeField] private int MaxCandyCorn;
    [SerializeField] private Animator CandyCornCollectedAnimator;
    [SerializeField] private TextMeshProUGUI CandyCornCollectedTextField;
    private TMP_Text CandyCornValue;

    void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Main_Scene")
        {
            CandyCornValue = GameObject.Find("CandyCornValue").GetComponent<TMP_Text>();
        }

        TotalCandyCorn = (int) SaveSystem.Load("Candy Corn");
    }

    void Update()
    {
        if (CandyCornValue)
        {
            CandyCornValue.text = TotalCandyCorn.ToString("0000");
        }
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
        {
            PlayCandyAmountAnimation(candyCorn, true);
            TotalCandyCorn += candyCorn;
            TotalCandyCorn = Mathf.Clamp(TotalCandyCorn, 0, MaxCandyCorn);
        }
        else
            Debug.LogWarning("Hey buddy, use RemoveCandyCorn if you want to remove");
    }

    public void RemoveCandyCorn(int candyCorn)
    {
        if (candyCorn >= 0)
        {
            PlayCandyAmountAnimation(candyCorn, false);
            TotalCandyCorn -= candyCorn;
            TotalCandyCorn = Mathf.Clamp(TotalCandyCorn, 0, MaxCandyCorn);
        }
        else
            Debug.LogWarning("Hey buddy, candyCorn passed here should be positive");
    }

    private void PlayCandyAmountAnimation(int candyAmount, bool isCollecting)
    {
        if (candyAmount <= 0) return;
        
        CandyCornCollectedTextField.gameObject.SetActive(true);
        if (isCollecting)
        {
            CandyCornCollectedTextField.text = $"+{candyAmount.ToString()}";
            CandyCornCollectedTextField.color = Color.green;
        }
        else
        {
            CandyCornCollectedTextField.text = $"-{candyAmount.ToString()}";
            CandyCornCollectedTextField.color = Color.red;
        }

        CandyCornCollectedAnimator.enabled = true;
        CandyCornCollectedAnimator.Play("Collect");
    }
}
