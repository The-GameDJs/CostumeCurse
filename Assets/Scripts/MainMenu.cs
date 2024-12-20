﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject Crossfade;
    [SerializeField] private Animator CrossfadeAnimator;
    [SerializeField] private GameObject CreditsPanel;
    [SerializeField] private RectTransform CreditsPanelRectTransform;
    [SerializeField] private GameObject PlayButton;
    [SerializeField] private GameObject NewGameButton;
    [SerializeField] private GameObject CreditsButton;
    [SerializeField] private GameObject QuitButton;
    [SerializeField] private GameObject CloseCreditsButton;
    [SerializeField] private GameObject TitleText;
    [SerializeField] private float ScrollSpeed;
    private bool AreCreditsRolling;
    private Vector3 InitialCreditsPanelRectPosition;
    

    void Start()
    {
        InitialCreditsPanelRectPosition = CreditsPanelRectTransform.position;
        NewGameButton.SetActive(SaveSystem.Load().RestPosition.x != 0);
    }

    private void Update()
    {
        if (AreCreditsRolling && CreditsPanelRectTransform.localPosition.y <= 1600.0f)
        {
            CreditsPanelRectTransform.position += new Vector3(0.0f, Time.deltaTime * ScrollSpeed, 0.0f);
        }
    }

    public void PlayGame()
    {
        if (SaveSystem.Load().RestPosition.x != 0)
        {
            SceneManager.LoadScene("Main_Scene");
            return;
        }

        StartCoroutine(LoadUpPrologueScene());
    }
    
    public void NewGame()
    {
        SaveSystem.DeleteSave();
        StartCoroutine(LoadUpPrologueScene());
    }

    public void OpenDemoSurvey()
    {
        Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSc1df417j_xaY7g2bp4JJU2YszVhLbZQIQqkmd5BGcMcc7Dpw/viewform?usp=sharing");
    }

    private IEnumerator LoadUpPrologueScene()
    {
        Crossfade.SetActive(true);
        CrossfadeAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene("Prologue_Scene");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ToggleCredits()
    {
        CreditsPanel.SetActive(true);
        AreCreditsRolling = true;
        PlayButton.SetActive(false);
        CreditsButton.SetActive(false);
        QuitButton.SetActive(false);
        TitleText.SetActive(false);
        CloseCreditsButton.SetActive(true);
    }

    public void CloseCredits()
    {
        PlayButton.SetActive(true);
        CreditsButton.SetActive(true);
        QuitButton.SetActive(true);
        AreCreditsRolling = false;
        CreditsPanelRectTransform.position = InitialCreditsPanelRectPosition;
        CreditsPanel.SetActive(false);
        TitleText.SetActive(true);
        
        CloseCreditsButton.SetActive(false);
    }
}
