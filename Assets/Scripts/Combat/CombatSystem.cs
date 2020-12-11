using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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

    void Start()
    {
        #if UNITY_EDITOR
            AssetDatabase.Refresh(); // This will update all animators, fixes a bug with Git! 
        #endif

        MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        CameraRig = MainCamera.GetComponent<CameraRig>();

        Sield = GameObject.Find("Sield");
        Ganiel = GameObject.Find("Sield");
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
        IsInProgress = true;

        CurrentCombatZone = CombatZone;
        AllyCombatants = allies.ToList();
        EnemyCombatants = enemies.ToList();
        
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

    private void OnEnemyWin()
    {
        Debug.Log("....Enemies Win....");

        CurrentCombatZone.GetComponent<CombatZone>().DestroyCombatZone();

        CameraRig.SetTransitionSmoothness(60);
        CameraRig.MoveCameraAbsolute(
            CameraRig.transform.position + 100 * Vector3.up,
            Quaternion.Euler(Vector3.up));
    }

    public void EndTurn(GameObject combatant)
    {
        Debug.Log($"A Combantant has finished their turn!");

        // TODO maybe a state would be useful after all?
        if (AlliesWon() || EnemiesWon())
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

        CurrentCombatantTurn++;

        if (CurrentCombatantTurn > Combatants.Count)
            StartNewRound();

        var currentCombatant = Combatants[CurrentCombatantTurn - 1].GetComponent<Combatant>();

        CameraRig.SetTargetGO(currentCombatant.gameObject);
        CameraRig.SetTransitionSmoothness(2);
        CameraRig.MoveCameraRelative(CurrentCombatZone.GetComponent<CombatZone>().CameraArea.Offset,
           Quaternion.Euler(CurrentCombatZone.GetComponent<CombatZone>().CameraArea.Rotation));

        currentCombatant.StartTurn();
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
}
