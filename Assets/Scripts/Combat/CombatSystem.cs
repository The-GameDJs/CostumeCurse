using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using System.Collections;
using Cinemachine;
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
    public List<GameObject> TargettableObjects;
    private int CurrentCombatantTurn;

    [SerializeField] private CinemachineVirtualCamera _currentCameraZoneCM;

    private GameObject MainCamera;

    private GameObject CurrentCombatZone;

    private GameObject Sield;
    private GameObject Ganiel;

    public bool IsInProgress = false;
    public bool IsBossDead;
    
    private CandyCornManager CandyCornManager;
    private int TotalCandyReward;
    private GameObject BattleVictoryBanner;
    private TextMeshProUGUI BattleBannerTitleText;
    private TextMeshProUGUI BattleBannerText;
    private int InitialCandyCornNumber;

    [SerializeField] private AnimationCurve _movementAnimCurve;
    public AnimationCurve MovementAnimCurve => _movementAnimCurve;

    void Start()
    {
        #if UNITY_EDITOR
            AssetDatabase.Refresh(); // This will update all animators, fixes a bug with Git! 
        #endif

        BattleVictoryBanner = GameObject.Find("BattleVictoryBanner");
        // Hacky way. CHANGE PREFAB NAMES BEFORE CHANGING THIS!
        BattleBannerTitleText = BattleVictoryBanner.transform.Find("Image").GetComponentInChildren<TextMeshProUGUI>();
        BattleBannerText = BattleVictoryBanner.transform.Find("Panel").GetComponentInChildren<TextMeshProUGUI>();
        BattleVictoryBanner.GetComponent<CanvasGroup>().alpha = 0f;
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera");

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
    public void StartCombat(GameObject CombatZone, GameObject[] allies, GameObject[] enemies, GameObject[] targettableObjects)
    {
        _currentCameraZoneCM = CinemachineCameraRig.Instance.CurrentCinemachineCamera;
        
        InitialCandyCornNumber = CandyCornManager.GetTotalCandyCorn();
        TotalCandyReward = 0;
        IsInProgress = true;

        CurrentCombatZone = CombatZone;
        AllyCombatants = allies.ToList();
        EnemyCombatants = enemies.ToList();
        
        TargettableObjects = targettableObjects != null ? targettableObjects.ToList() : new List<GameObject>();

        foreach (var enemy in EnemyCombatants)
            TotalCandyReward += enemy.GetComponent<EnemyCombatant>().GetCandyCornValue();
        
        Combatants = AllyCombatants.Concat(EnemyCombatants).Concat(TargettableObjects).ToList();

        StartNewRound();
    }

    private void EndCombat()
    {
        if (EnemiesWon())
            OnEnemyWin();
        else
            OnAllyWin();
    }

    private void OnAllyWin()
    {
        Debug.Log("!!!!Allies Win!!!!");
        CandyCornManager.AddCandyCorn(TotalCandyReward);

        // If it's not the boss that died, return to normal gameplay win
        if (!IsBossDead)
        {
            StartCoroutine(ShowVictoryBanner(true, TotalCandyReward));

            CurrentCombatZone.GetComponent<CombatZone>().DestroyCombatZone();

            CinemachineCameraRig.Instance.SetCinemachineCamera(_currentCameraZoneCM);
            CinemachineCameraRig.Instance.ChangeCinemachineBrainBlendTime(2.0f);
            _currentCameraZoneCM = null;
        }
        // If boss died, play boss death scenario and return to menu
        else
        {
            Debug.Log("!!!!Boss Died!!!!");
            // Move camera to boss
            CinemachineCameraRig.Instance.ChangeCinemachineBrainBlendTime(5.0f);
            CinemachineCameraRig.Instance.SetCinemachineCamera(GameObject.Find("CursedPimpkinBoss").GetComponentInChildren<CinemachineVirtualCamera>());
            PlayerPrefs.SetInt("CandyCollected", CandyCornManager.GetTotalCandyCorn());
        }
        IsInProgress = false;
    }

    private void OnEnemyWin()
    {
        Debug.Log("!!!!Enemies Win!!!!");
        var combatZone = CurrentCombatZone.GetComponent<CombatZone>();
        combatZone.SetCombatColliderVisibility(false);

        if (Sield != null)
        {
            Sield.transform.position = combatZone.CheckpointPosition.position;
            Sield.GetComponent<Player>().SetColliderVisibility(false);
            CinemachineCameraRig.Instance.SetCinemachineCamera(_currentCameraZoneCM);
            CinemachineCameraRig.Instance.SetCinemachineCameraTarget(Sield.transform);
            CinemachineCameraRig.Instance.ChangeCinemachineBrainBlendTime(2.0f);
        }
        if (Ganiel != null)
        {
            //Ganiel.transform.position = Sield.GetComponent<Player>().GetTargetPosition();
        }

        for (int i = 0; i < combatZone.GetEnemies.Length; ++i)
        {
            var enemies = CurrentCombatZone.GetComponent<CombatZone>().GetEnemies;
            enemies[i].transform.position = combatZone.EnemiesInitialPosition[i];
            enemies[i].transform.rotation = combatZone.EnemiesInitialRotation[i];
            if (enemies[i].TryGetComponent<Combatant>(out var enemyCombatant) && !enemyCombatant.IsAlive)
            {
                enemies[i].GetComponent<Animator>().Play("Idle");
            }
        }
        
        combatZone.DestroyCombatZone(false);
        
        var candyDifference = InitialCandyCornNumber - CandyCornManager.GetTotalCandyCorn();
        if (candyDifference > 0)
        {
            CandyCornManager.AddCandyCorn(candyDifference);
        }
        else if (candyDifference < 0)
        {
            CandyCornManager.RemoveCandyCorn(-candyDifference); // double negate value to properly subtract in function
        }
        CandyCornManager.AddCandyCorn(InitialCandyCornNumber - CandyCornManager.GetTotalCandyCorn());
        StartCoroutine(ShowVictoryBanner(false, Mathf.Abs(candyDifference)));
        InitialCandyCornNumber = 0;
        CurrentCombatZone = null;

        IsInProgress = false;
        Sield.GetComponent<Player>().SetColliderVisibility(true);
        combatZone.SetCombatColliderVisibility(true);
    }

    public void EndTurn()
    {
        Debug.Log($"{Combatants[CurrentCombatantTurn-1]} has finished their turn!");

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

        var combatZone = CurrentCombatZone.GetComponent<CombatZone>();
        CinemachineCameraRig.Instance.SetCinemachineCamera(combatZone.CombatCinemachineCamera);
        CinemachineCameraRig.Instance.SetCinemachineCameraTarget(currentCombatant.transform);
        
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
        {
            var enemyPosition = EnemyCombatants[Random.Range(0, EnemyCombatants.Count)].transform.position;
            enemyPosition.y = ally.transform.position.y;
            ally.transform.LookAt(enemyPosition);
        }

        foreach (GameObject enemy in EnemyCombatants)
        {
            var allyPosition = AllyCombatants[Random.Range(0, AllyCombatants.Count)].transform.position;
            allyPosition.y = enemy.transform.position.y;
            enemy.transform.LookAt(allyPosition);
        }

        Combatants[CurrentCombatantTurn - 1].GetComponent<Combatant>().StartTurn();
    }

    private IEnumerator ShowVictoryBanner(bool isVictory, int candyDisplay)
    {
        if (isVictory)
        {
            BattleBannerTitleText.text = $"Victory!";
            BattleBannerText.text = $"You won {candyDisplay}";
        }
        else
        {
            BattleBannerTitleText.text = $"Defeat!";
            BattleBannerText.text = $"Returned {candyDisplay}";
        }
        
        BattleVictoryBanner.GetComponent<CanvasGroup>().alpha = 1f;
        yield return new WaitForSeconds(2);
        BattleVictoryBanner.GetComponent<CanvasGroup>().alpha = 0f;
    }
}
