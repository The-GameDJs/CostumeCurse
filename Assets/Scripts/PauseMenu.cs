using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private GameObject PauseMenuUI { get; set; }
    public static bool IsPaused = false;


    private void Awake()
    {
        PauseMenuUI = GameObject.Find("PauseMenu");
    }

    void Start()
    {
        PauseMenuUI.SetActive(false);
        InputManager.PausedAction += OnPauseButtonClicked;
    }

    private void OnDestroy()
    {
        InputManager.PausedAction -= OnPauseButtonClicked;
    }

    private void OnPauseButtonClicked()
    {
        IsPaused = !IsPaused;

        if(IsPaused)
        {
            ActivateMenu();
        }
        else
        {
            DeactivateMenu();
        }
    }

    void ActivateMenu()
    {
        Time.timeScale = 0;
        PauseMenuUI.SetActive(true);
    }

    public void DeactivateMenu()
    {
        Time.timeScale = 1;
        PauseMenuUI.SetActive(false);
        IsPaused = false;
    }

    public void BackToTitle()
    {
        IsPaused = false;
        DeactivateMenu();
        SceneManager.LoadScene("Title_Screen");
    }

    public void QuitGame()
    {
        InputManager.PausedAction -= OnPauseButtonClicked;
        Application.Quit();
    }
}
