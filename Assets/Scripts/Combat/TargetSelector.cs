using System.Collections;
using System.Linq;
using Assets.Scripts.Combat;
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
    [SerializeField] private GameObject SelectTextPrompt;
    [SerializeField] private GameObject WrongSelectionPrompt;
    private GameObject CurrentSelectionPrompt;

    public void Start()
    {
        MouseSelector = GetComponent<MouseSelect>();
        MouseSelector.enabled = false;
        CurrentSelectionPrompt = SelectTextPrompt;
        CurrentSelectionPrompt.SetActive(false);
        CombatSystem = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();
    }

    public void Target(Ability callingAbility, bool userTargeting = true)
    {
        CallingAbility = callingAbility;
        CurrentTargetSchema = callingAbility.TargetSchema;

        // Show Bars UI
        foreach (var combatant in CombatSystem.Combatants)
        {
            if (combatant.TryGetComponent(out ObjectCombatant objCombatant))
            {
                objCombatant.HideBarsUI();
                continue;
            }

            combatant.GetComponent<Combatant>().ShowBarsUI();
        }

        if (CurrentTargetSchema.SelectorType == SelectorType.All)
        {
            if (CurrentTargetSchema.CombatantType == CombatantType.Enemy)
                TargetAllEnemies();
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
        
        if (CurrentTargetSchema.SelectorType == SelectorType.Self)
        {
            // TODO do something else, add rest of options
            TargetSelf();
        }
    }

    private IEnumerator TargetSingleEnemy()
    {
        MouseSelector.enabled = true;
        MouseSelector.IsSingleTargetting = true;
        CurrentSelectionPrompt.SetActive(true);
        
        while (!MouseSelector.IsTargetSelected)
        {
            if (MouseSelector.IsRegrettingDecision)
            {
                MouseSelector.enabled = false;
                Debug.Log("Regretting decision");
                MouseSelector.IsRegrettingDecision = false;
                CombatSystem.GoBackToAbilitySelect();
                CurrentSelectionPrompt.SetActive(false);
                CurrentSelectionPrompt = SelectTextPrompt;
                yield break;
            }
            yield return null;
        }
        
        MouseSelector.IsSingleTargetting = false;
        MouseSelector.IsTargetSelected = false;
        MouseSelector.enabled = false;
        CurrentSelectionPrompt.SetActive(false);
        CurrentSelectionPrompt = SelectTextPrompt;

        ReplyToCallingAbility();
    }

    public void ChooseTarget(GameObject target)
    {
        // TODO: Not select enemies depending on ability selected
        GameObject[] enemy = {target};
        CurrentTargetedCombatants = enemy;
    }

    private void TargetAllEnemies()
    {
        GameObject[] enemies = CombatSystem.Combatants.
            Where(combatant => combatant.CompareTag("Enemy") && combatant.GetComponent<Combatant>().IsAlive).ToArray();

        CurrentTargetedCombatants = enemies;
        
        ReplyToCallingAbility();
    }
    
    private void TargetAllAllies(bool userTargeting = true)
    {
        GameObject[] allies = CombatSystem.Combatants.
          Where(combatant => combatant.CompareTag("Player") && combatant.GetComponent<Combatant>().IsAlive).ToArray();

        CurrentTargetedCombatants = allies;

        ReplyToCallingAbility();
    }

    private void TargetSelf()
    {
        var callingCombatant = CallingAbility.transform.parent.gameObject;
        GameObject[] allies = { callingCombatant };
        CurrentTargetedCombatants = allies;

        ReplyToCallingAbility();
    }

    private void ReplyToCallingAbility()
    {
        if (CallingAbility.AttackStyle == AttackStyle.Melee && CurrentTargetedCombatants[0].GetComponent<Combatant>().CombatType == Combatant.CombatantType.Flying)
        {
            CurrentSelectionPrompt.SetActive(false);
            CurrentSelectionPrompt = WrongSelectionPrompt;
            StartCoroutine(TargetSingleEnemy());
        }
        else
        {
            CallingAbility.SetTargetedCombatants(CurrentTargetedCombatants);
        }
        
    }
}
