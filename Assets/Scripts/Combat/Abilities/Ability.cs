using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Ability : MonoBehaviour
{
    [SerializeField] private int CandyCornCost;
    private CandyCornManager CandyCornManager;
    private GameObject NotEnoughCandiesPrompt;

    protected TargetSelector TargetSelector;
    protected CombatSystem CombatSystem;

    public TargetSchema TargetSchema;

    protected GameObject[] TargetedCombatants;

    protected Animator Animator;

    private Combatant Combatant;

    [SerializeField] protected AudioSource MissedActionCommandSound;
    [SerializeField] protected AudioSource GoodActionCommandSound;
    [SerializeField] protected AudioSource PerfectActionCommandSound;

    public void Start()
    {
        NotEnoughCandiesPrompt = GameObject.Find("NotEnoughCandyText");
        NotEnoughCandiesPrompt.GetComponent<CanvasGroup>().alpha = 0f;
        CandyCornManager = GameObject.FindObjectOfType<CandyCornManager>();
        TargetSelector = GameObject.FindGameObjectWithTag("TargetSelector").GetComponent<TargetSelector>();
        CombatSystem = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();
        Animator = GetComponentInParent<Animator>();
        if (GetComponent<Combatant>() != null)
            Combatant = GetComponent<Combatant>();
        else
            Combatant = GetComponentInParent<Combatant>();
    }

    public void StartAbility(bool userTargeting = true)
    {
        if (CandyCornCost <= CandyCornManager.GetTotalCandyCorn())
        {
            // Hide UI for Allies
            Costume costume = GetComponentInChildren<Costume>();
            if (costume)
                costume.DisplayAbilities(false);

            TargetSelector.Target(this, userTargeting);
        }
        else
        {
            StartCoroutine(ShowNotEnoughCandiesPrompt());
        }

    }

    protected abstract void ContinueAbilityAfterTargeting();

    // called by the TargetSelector once it has selected targets
    public void SetTargetedCombatants(GameObject[] targetedCombatants)
    {
        CandyCornManager.RemoveCandyCorn(CandyCornCost);
        
        Debug.Log($"Ability about to continue! Got {targetedCombatants.Length} combatnats");
        TargetedCombatants = targetedCombatants;
        
        FaceEachOtherInCombat();

        ContinueAbilityAfterTargeting();
    }

    private void FaceEachOtherInCombat()
    {
        var facedOpponent = TargetedCombatants[Random.Range(0, TargetedCombatants.Length)];
        facedOpponent.GetComponent<Combatant>().TurnToFaceInCombat(this.gameObject.transform);
        Combatant.TurnToFaceInCombat(facedOpponent.gameObject.transform);
    }

    protected abstract void EndAbility();

    private IEnumerator ShowNotEnoughCandiesPrompt()
    {
        NotEnoughCandiesPrompt.GetComponent<CanvasGroup>().alpha = 1f;
        yield return new WaitForSeconds(1);
        NotEnoughCandiesPrompt.GetComponent<CanvasGroup>().alpha = 0f;
    }
}
