using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    protected TargetSelector TargetSelector;
    protected CombatSystem CombatSystem;

    public TargetSchema TargetSchema;

    protected GameObject[] TargetedCombatants;

    protected Animator Animator;

    public void Start()
    {
        TargetSelector = GameObject.FindGameObjectWithTag("TargetSelector").GetComponent<TargetSelector>();
        CombatSystem = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();
        Animator = GetComponentInParent<Animator>();
    }

    public void StartAbility(bool userTargeting = true)
    {
        // Hide UI for Allies
        Costume costume = GetComponentInChildren<Costume>();
        if (costume)
            costume.DisplayAbilities(false);

        TargetSelector.Target(this, userTargeting);
    }

    protected abstract void ContinueAbilityAfterTargeting();

    // called by the TargetSelector once it has selected targets
    public void SetTargetedCombatants(GameObject[] targetedCombatants)
    {
        Debug.Log($"Ability about to continue! Got {targetedCombatants.Length} combatnats");
        TargetedCombatants = targetedCombatants;

        ContinueAbilityAfterTargeting();
    }

    protected abstract void EndAbility();
}
