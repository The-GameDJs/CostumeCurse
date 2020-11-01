using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character
{
    [SerializeField] 
    private GameObject enemyPanel;
    [SerializeField]
    private GameObject enemyUITemplate;
    private GameObject enemyUI;

    private bool displayUI;

    private void Update()
    {
        if (displayUI)
        {
            Vector3 relativeScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
            enemyUI.transform.position = relativeScreenPosition;
        }
    }

    public void DisplayUI(bool displayUI)
    {
        this.displayUI = displayUI;
        enemyUITemplate.SetActive(displayUI);
    }


    // Start is called before the first frame update
    void Start()
    {
        enemyUI = Instantiate(enemyUITemplate);
        enemyUI.transform.parent = enemyPanel.transform;

        DisplayUI(true); // for now
    }

    public override void PlayTurn()
    {
        
    }
}
