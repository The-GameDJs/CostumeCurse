using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Costume : MonoBehaviour
{
    [SerializeField] private GameObject AbilitiesUIPanel;
    private bool IsDisplayingAbilities;
    private List<GameObject> Abilities;
    private int AbilityCount = 1;

    // TODO: Stop displaying abilities UI after a choice

    private void Awake()
    {
        AbilitiesUIPanel.SetActive(false);
        Abilities = new List<GameObject>();
        
        foreach(Transform child in AbilitiesUIPanel.transform)
            Abilities.Add(child.gameObject);
    }

    private void Start()
    {
        var save = SaveSystem.LoadSave();

        switch (transform.parent.gameObject.name)
        {
            case "Sield": 
                AbilityCount = save.SieldAbilityIndex; 
                break;
            default: 
                AbilityCount = save.GanielAbilityIndex;
                break;
        }
        
        DialogueManager.GrantAbility += OnFinishedTalkingToMonk;
    }

    private void OnFinishedTalkingToMonk(string character)
    {
        if(transform.parent.gameObject.name == character)
            AbilityCount++;
    }

    private void Update()
    {
        if (IsDisplayingAbilities)
        {
            Vector3 relativeScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
            AbilitiesUIPanel.transform.position = relativeScreenPosition;
        }
    }

    public void DisplayAbilities(bool displayAbilities)
    {
        IsDisplayingAbilities = displayAbilities;
        AbilitiesUIPanel.SetActive(displayAbilities);
        
        for(int i = 0; i <= AbilityCount; i++)
            Abilities[i].SetActive(true);
    }

    public int GetAbilityIndex()
    {
        return AbilityCount;
    }

    private void OnDestroy()
    {
        DialogueManager.GrantAbility -= OnFinishedTalkingToMonk;
    }
}
