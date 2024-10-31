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
    private int AbilityCount = 2;

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
        var save = SaveSystem.Load();

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
        DialogueManager.DemonstrateAbilityVFX += OnDemonstratedAbilityVFX;
    }

    private void OnFinishedTalkingToMonk(string character)
    {
        if(transform.parent.gameObject.name == character)
            AbilityCount++;
    }

    private void OnDemonstratedAbilityVFX(string character, Monk monk)
    {
        if (transform.parent.gameObject.name == character)
        {
            monk.GrantingPlayerAbility.audioFX.Play();
            monk.GrantingPlayerAbility.audioFX.gameObject.transform.position = transform.parent.position + new Vector3(0.0f, 5.0f, 0.0f);
            foreach (var vfx in monk.GrantingPlayerAbility.visualFX)
            {
                if (vfx.TryGetComponent<ParticleSystem>(out var ps))
                {
                    vfx.transform.position = transform.parent.position + new Vector3(0.0f, 5.0f, 0.0f);
                    ps.Play();
                }
                else
                {
                    StartCoroutine(PeakAbilityGameObject(vfx));
                }
            }
        }
    }

    private IEnumerator PeakAbilityGameObject(GameObject go)
    {
        go.transform.position = transform.parent.position + new Vector3(0.0f, 3.0f, 0.0f);
        yield return new WaitForSeconds(2.0f);
        go.transform.position = Vector3.zero;
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
        DialogueManager.DemonstrateAbilityVFX -= OnDemonstratedAbilityVFX;
    }
}
