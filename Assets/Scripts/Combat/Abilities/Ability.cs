using System.Collections;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    [SerializeField] private int CandyCornCost;
    protected CandyCornManager CandyCornManager;
    private GameObject NotEnoughCandiesPrompt;

    protected TargetSelector TargetSelector;
    protected CombatSystem CombatSystem;
    protected CameraRig CameraRigSystem;
    public CameraRig GetCameraRigSystem => CameraRigSystem;

    public TargetSchema TargetSchema;

    // Not used right now, but wanted to do a smooth rotation somehow...which is possible but it would be heavy due to how it is structured right now (the GetComponent functions we call every rotation)
   // private enum Phase { Inactive, StartFacingEachOther };
   // private Phase CurrentPhase;

    protected GameObject[] TargetedCombatants;

    [SerializeField] protected Animator Animator;

    protected Combatant Combatant;

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
        CameraRigSystem = GameObject.FindObjectOfType<CameraRig>();

        //CurrentPhase = Phase.Inactive;
        if (GetComponent<Combatant>() != null)
            Combatant = GetComponent<Combatant>();
        else
            Combatant = GetComponentInParent<Combatant>();
    }

    public void StartAbility(bool userTargeting = true)
    {
        // Sometimes the manager is not assigned. Find it again!
        if (!CandyCornManager)
        {
            CandyCornManager = FindObjectOfType<CandyCornManager>();
        }
        if (CandyCornCost <= CandyCornManager.GetTotalCandyCorn())
        {
            // Hide UI for Allies
            Costume costume = GetComponentInChildren<Costume>();
            if (costume)
                costume.DisplayAbilities(false);

            // Find target selector because it keeps being null!
            if (!TargetSelector)
            {
                TargetSelector = FindObjectOfType<TargetSelector>();
            }
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
        
        Debug.Log($"Ability about to continue! Got {targetedCombatants.Length} {targetedCombatants.GetType()}");
        TargetedCombatants = targetedCombatants;
        Debug.Log($"Should be targetting {TargetedCombatants[0].GetType()}");
        if (TargetedCombatants[0].TryGetComponent(out EnemyCombatant _)) // Kinda heavy on performance, but I am not sure how to check the class without getting the component
            FaceEnemyInCombat();

        ContinueAbilityAfterTargeting();
    }

    private void FaceEnemyInCombat()
    {
        Debug.Log("Ally is facing its target!");
        var facedOpponent = TargetedCombatants[0]; // For now, since ally abilities only ever have 1 target, we can just hardcode this
        facedOpponent.GetComponent<Combatant>().TurnToFaceInCombat(this.gameObject.transform);
        Combatant.TurnToFaceInCombat(facedOpponent.gameObject.transform);
    }

    public void FaceAllyInCombat(GameObject allyTarget)
    {
        Debug.Log("Enemy is facing its target!");
        allyTarget.GetComponent<Combatant>().TurnToFaceInCombat(this.gameObject.transform);
        Combatant.TurnToFaceInCombat(allyTarget.gameObject.transform);
    }

    protected abstract void EndAbility();

    private IEnumerator ShowNotEnoughCandiesPrompt()
    {
        NotEnoughCandiesPrompt.GetComponent<CanvasGroup>().alpha = 1f;
        yield return new WaitForSeconds(1);
        NotEnoughCandiesPrompt.GetComponent<CanvasGroup>().alpha = 0f;
    }
}
