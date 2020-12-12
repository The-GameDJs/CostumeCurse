using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameVictoryScreen : MonoBehaviour
{
    private GameObject EpiloguePanel;

    void Start()
    {
        EpiloguePanel = GameObject.Find("PostStory");
        EpiloguePanel.SetActive(false);
    }

    public void GoBackToTitle()
    {
        SceneManager.LoadScene("Title_Screen");
    }

    public void ToggleEpilogue()
    {
        EpiloguePanel.SetActive(!EpiloguePanel.active);
    }
}
