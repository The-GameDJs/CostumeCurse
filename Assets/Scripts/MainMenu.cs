using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private GameObject CreditsPanel;

    void Start()
    {
        CreditsPanel = GameObject.Find("Credits");
        CreditsPanel.SetActive(false);
    }


    public void PlayGame()
    {
        SceneManager.LoadScene("Main_Scene");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ToggleCredits()
    {
        CreditsPanel.SetActive(!CreditsPanel.active);
    }
}
