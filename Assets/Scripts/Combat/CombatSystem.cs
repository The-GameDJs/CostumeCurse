using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using System.Collections;
using Combat.Enemy_Abilities;
using TMPro;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public class CombatSystem : MonoBehaviour
{
    public List<GameObject> Combatants;
    public List<GameObject> AllyCombatants;
    public List<GameObject> EnemyCombatants;
    private int CurrentCombatantTurn;

    private GameObject MainCamera;
    private CameraRig CameraRig;

    private GameObject CurrentCombatZone;

    private GameObject Sield;
    private GameObject Ganiel;

    public bool IsInProgress = false;
    public bool IsBossDead;
    
    private CandyCornManager CandyCornManager;
    private int TotalCandyReward;
    private GameObject BattleVictoryBanner;

    void Start()
    {
        #if UNITY_EDITOR
            AssetDatabase.Refresh(); // This will update all animators, fixes a bug with Git! 
        #endif

        BattleVictoryBanner = GameObject.Find("BattleVictoryBanner");
        BattleVictoryBanner.GetComponent<CanvasGroup>().alpha = 0f;
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        CameraRig = MainCamera.GetComponent<CameraRig>();

        Sield = GameObject.Find("Sield");
        Ganiel = GameObject.Find("Sield");

        CandyCornManager = FindObjectOfType<CandyCornManager>();
    }

    private int SortByTurnPriority(GameObject combatant1, GameObject combatant2)
    {
        int turnPriority1 = combatant1.GetComponent<Combatant>().TurnPriority;
        int turnPriority2 = combatant2.GetComponent<Combatant>().TurnPriority;

        if (turnPriority1 < turnPriority2)
            return -1;

        if (turnPriority1 == turnPriority2)
            return (int)(Random.value - 0.5f);

        return 1;
    }

    // Called by CombatZone
    public void StartCombat(GameObject CombatZone, GameObject[] allies, GameObject[] enemies)
    {
        TotalCandyReward = 0;
        IsInProgress = true;

        CurrentCombatZone = CombatZone;
        AllyCombatants = allies.ToList();
        EnemyCombatants = enemies.ToList();

        foreach (var enemy in EnemyCombatants)
            TotalCandyReward += enemy.GetComponent<EnemyCombatant>().GetCandyCornValue();
        
        Combatants = AllyCombatants.Concat(EnemyCombatants).ToList();

        StartNewRound();
    }

    private void EndCombat()
    {
        if (EnemiesWon())
            OnEnemyWin();
        else
            OnAllyWin();

        IsInProgress = false;
    }

    private void OnAllyWin()
    {
        Debug.Log("!!!!Allies Win!!!!");
        CandyCornManager.AddCandyCorn(TotalCandyReward);

        // If it's not the boss that died, return to normal gameplay win
        if (!IsBossDead)
        {
            BattleVictoryBanner.GetComponentInChildren<TMP_Text>().text = "You got " + TotalCandyReward;

            StartCoroutine(ShowVictoryBanner());

            CurrentCombatZone.GetComponent<CombatZone>().DestroyCombatZone();

            if (Sield != null)
            {
                CameraRig.SetTargetGO(Sield);
                CameraRig.MoveCameraRelative(CameraRig.DefaultOffset, CameraRig.DefaultRotation);
            }

            else if (Ganiel != null)
            {
                CameraRig.SetTargetGO(Ganiel);
                CameraRig.MoveCameraRelative(CameraRig.DefaultOffset, CameraRig.DefaultRotation);
            }
        }
        // If boss died, play boss death scenario and return to menu
        else
        {
            Debug.Log("!!!!Boss Died!!!!");
            // Move camera to boss
            CameraRig.SetTargetGO(EnemyCombatants.Find(c => c.TryGetComponent<EnemyCombatant>(out var boss) && boss.isBoss));
            CameraRig.MoveCameraRelative(CameraRig.DefaultBossOffset, Quaternion.Euler(CameraRig.DefaultBossRotation));
            PlayerPrefs.SetInt("CandyCollected", CandyCornManager.GetTotalCandyCorn());
        }

    }

    private void OnEnemyWin()
    {
        SceneManager.LoadScene("Game_Over");
    }

    public void EndTurn(GameObject combatant)
    {
        Debug.Log($"A Combantant has finished their turn!");

        // TODO maybe a state would be useful after all?
        if (AlliesWon() || EnemiesWon() || IsBossDead)
            EndCombat();
        else 
            StartNextTurn();
    }

    private bool AlliesWon()
    {
        foreach (GameObject enemy in EnemyCombatants)
            if (enemy.GetComponent<EnemyCombatant>().IsAlive)
                return false;

        return true;
    }

    private bool EnemiesWon()
    {
        foreach (GameObject ally in AllyCombatants)
            if (ally.GetComponent<AllyCombatant>().IsAlive)
                return false;

        return true;
    }

    private void StartNextTurn()
    {
        Debug.Log($"Starting a new turn");

        foreach (var combatant in Combatants)
        {
            combatant.GetComponent<Combatant>().HideBarsUI();
        }

        CurrentCombatantTurn++;

        if (CurrentCombatantTurn > Combatants.Count)
            StartNewRound();

        var currentCombatant = Combatants[CurrentCombatantTurn - 1].GetComponent<Combatant>();

        currentCombatant.StartTurn();
        
        CameraRig.SetTargetGO(currentCombatant.gameObject);
        CameraRig.SetTransitionSmoothness(2);
        if (currentCombatant is EnemyCombatant enemy && enemy.isBoss && enemy.GetComponentInChildren<CandyStorm>().GetCandyCornPhase == CandyStorm.CandyStormPhase.Cloud)
        {
            CameraRig.MoveCameraRelative(CurrentCombatZone.GetComponent<CombatZone>().CameraArea.Offset,
                Quaternion.Euler(CurrentCombatZone.GetComponent<CombatZone>().CameraArea.Rotation + new Vector3(-15.0f, -5.0f, 0.0f)));
        }
        else
        {
            CameraRig.MoveCameraRelative(CurrentCombatZone.GetComponent<CombatZone>().CameraArea.Offset,
                Quaternion.Euler(CurrentCombatZone.GetComponent<CombatZone>().CameraArea.Rotation));
        }
    }

    public void GoBackToAbilitySelect()
    {
        Combatants[CurrentCombatantTurn - 1].GetComponent<Combatant>().StartTurn();
    }

    private void StartNewRound()
    {
        Debug.Log($"Starting a new round");

        CurrentCombatantTurn = 1;

        Combatants.Sort(SortByTurnPriority);

        foreach (GameObject ally in AllyCombatants)
            ally.transform.LookAt(EnemyCombatants[Random.Range(0, EnemyCombatants.Count)].transform.position);
        foreach (GameObject enemy in EnemyCombatants)
            enemy.transform.LookAt(AllyCombatants[Random.Range(0, AllyCombatants.Count)].transform.position);

        Combatants[CurrentCombatantTurn - 1].GetComponent<Combatant>().StartTurn();
    }

    private IEnumerator ShowVictoryBanner()
    {
        BattleVictoryBanner.GetComponent<CanvasGroup>().alpha = 1f;
        yield return new WaitForSeconds(2);
        BattleVictoryBanner.GetComponent<CanvasGroup>().alpha = 0f;
    }
}
