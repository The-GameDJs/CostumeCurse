using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private GameObject PauseMenuUI;
    public static bool IsPaused = false;

    void Start()
    {
        PauseMenuUI = GameObject.Find("PauseMenu");
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            IsPaused = !IsPaused;
        }

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
        DeactivateMenu();
        SceneManager.LoadScene("Title_Screen");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
