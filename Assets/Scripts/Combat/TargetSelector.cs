using System;
using System.Collections;
using System.Linq;
using System.Threading;
using UnityEngine;


public enum CombatantType { Enemy, Ally };
public enum SelectorType { Self, All, Number };

public struct TargetSchema
{    
    public int NumberOfTargets;
    public CombatantType CombatantType;
    public SelectorType SelectorType;

    public TargetSchema(int numberOfTargets, CombatantType combatantType, SelectorType selectorType)
    {
        NumberOfTargets = numberOfTargets;
        CombatantType = combatantType;
        SelectorType = selectorType;
    }
}

// TODO let's make this virtual: one for allies; one for enemies, cuz they're pretty different
public class TargetSelector : MonoBehaviour
{
    public CombatSystem CombatSystem;
    private MouseSelect MouseSelector;

    private GameObject[] CurrentTargetedCombatants;
    private Ability CallingAbility;
    private TargetSchema CurrentTargetSchema;
    private GameObject SelectTextPrompt;

    public void Start()
    {
        MouseSelector = GetComponent<MouseSelect>();
        MouseSelector.enabled = false;
        SelectTextPrompt = GameObject.Find("CombatTextBox");
        SelectTextPrompt.SetActive(false);
        CombatSystem = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();
    }

    public void Target(Ability callingAbility, bool userTargeting = true)
    {
        CallingAbility = callingAbility;
        CurrentTargetSchema = callingAbility.TargetSchema;

        // Show Bars UI
        foreach (var combatant in CombatSystem.Combatants)
        {
            combatant.GetComponent<Combatant>().ShowBarsUI();
        }

        if (CurrentTargetSchema.SelectorType == SelectorType.All)
        {
            if (CurrentTargetSchema.CombatantType == CombatantType.Enemy)
                StartCoroutine(TargetAllEnemies());
            else if (CurrentTargetSchema.CombatantType == CombatantType.Ally)
                TargetAllAllies(userTargeting);
        }

        if (CurrentTargetSchema.SelectorType == SelectorType.Number)
        {
            // TODO do something else, add rest of options
            if (CurrentTargetSchema.CombatantType == CombatantType.Enemy)
                StartCoroutine(TargetSingleEnemy());
            else if (CurrentTargetSchema.CombatantType == CombatantType.Ally)
                TargetAllAllies(userTargeting);
        }
    }

    private IEnumerator TargetSingleEnemy()
    {
        MouseSelector.enabled = true;
        MouseSelector.IsSingleTargetting = true;
        SelectTextPrompt.SetActive(true);
        
        while (!MouseSelector.IsTargetSelected)
        {
            if (MouseSelector.IsRegrettingDecision)
            {
                MouseSelector.enabled = false;
                Debug.Log("Regretting decision");
                MouseSelector.IsRegrettingDecision = false;
                CombatSystem.GoBackToAbilitySelect();
                SelectTextPrompt.SetActive(false);
                yield break;
            }
            yield return null;
        }
        
        MouseSelector.IsSingleTargetting = false;
        MouseSelector.IsTargetSelected = false;
        MouseSelector.enabled = false;
        SelectTextPrompt.SetActive(false);

        ReplyToCallingAbility();
    }

    public void ChooseTarget(GameObject target)
    {
        GameObject[] enemy = {target};
        CurrentTargetedCombatants = enemy;
    }

    private IEnumerator TargetAllEnemies()
    {
        GameObject[] enemies = CombatSystem.Combatants.
            Where(combatant => combatant.CompareTag("Enemy") && combatant.GetComponent<Combatant>().IsAlive).ToArray();

        MouseSelector.enabled = true;
        SelectTextPrompt.SetActive(true);
        
        while (!MouseSelector.IsTargetSelected)
        {
            if (MouseSelector.IsRegrettingDecision)
            {
                MouseSelector.enabled = false;
                Debug.Log("Regretting decision");
                MouseSelector.IsRegrettingDecision = false;
                CombatSystem.GoBackToAbilitySelect();
                SelectTextPrompt.SetActive(false);
                yield break;
            }
            yield return null;
        }
        SelectTextPrompt.SetActive(false);

        MouseSelector.IsTargetSelected = false;
        MouseSelector.enabled = false;

        CurrentTargetedCombatants = enemies;
        
        ReplyToCallingAbility();
    }
    
    private void TargetAllAllies(bool userTargeting = true)
    {
        GameObject[] allies = CombatSystem.Combatants.
          Where(combatant => combatant.CompareTag("Player") && combatant.GetComponent<Combatant>().IsAlive).ToArray();

        if (userTargeting)
        {
            // TODO highlight each enemy, wait for user selection
            Thread.Sleep(500); // Fake the UI selection
        }

        CurrentTargetedCombatants = allies;

        ReplyToCallingAbility();
    }

    private void ReplyToCallingAbility()
    {
        CallingAbility.SetTargetedCombatants(CurrentTargetedCombatants);
    }
}
