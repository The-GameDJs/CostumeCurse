﻿using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class CombatSystem : MonoBehaviour
{
    public List<GameObject> Combatants;
    public List<GameObject> AllyCombatants;
    public List<GameObject> EnemyCombatants;
    private int CurrentCombatantTurn;

    private GameObject MainCamera;

    private GameObject CurrentCombatZone;

    [SerializeField]
    private GameObject Sield;
    [SerializeField]
    private GameObject Ganiel;

    void Start()
    {
        AssetDatabase.Refresh(); // This will update all animators, fixes a bug with Git! 

        MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    void Update()
    {

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
        CurrentCombatZone = CombatZone;
        AllyCombatants = allies.ToList();
        EnemyCombatants = enemies.ToList();
        
        Combatants = AllyCombatants.Concat(EnemyCombatants).ToList();

        StartNewRound();
    }

    private void EndCombat()
    {
        CurrentCombatZone.GetComponent<CombatZone>().DestroyCombatZone();
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

        Debug.Log("!!!!Allies Win!!!!");

        MainCamera.GetComponent<CameraRig>().SetTargetGO(Sield);

        return true;
    }

    private bool EnemiesWon()
    {
        foreach (GameObject ally in AllyCombatants)
            if (ally.GetComponent<AllyCombatant>().IsAlive)
                return false;

        Debug.Log("....Enemies Win....");
        return true;
    }

    private void StartNextTurn()
    {
        Debug.Log($"Starting a new turn");

        CurrentCombatantTurn++;

        if (CurrentCombatantTurn > Combatants.Count)
            StartNewRound();

        var currentCombatant = Combatants[CurrentCombatantTurn - 1].GetComponent<Combatant>();

        MainCamera.GetComponent<CameraRig>().SetTargetGO(currentCombatant.gameObject);

        currentCombatant.StartTurn();

    }

    private void StartNewRound()
    {
        Debug.Log($"Starting a new round");

        CurrentCombatantTurn = 1;

        Combatants.Sort(SortByTurnPriority); // TODO use case for updating priority?

        foreach (GameObject ally in AllyCombatants)
            ally.transform.LookAt(EnemyCombatants[Random.Range(0, EnemyCombatants.Count)].transform.position);
        foreach (GameObject enemy in EnemyCombatants)
            enemy.transform.LookAt(AllyCombatants[Random.Range(0, EnemyCombatants.Count)].transform.position);

        Combatants[CurrentCombatantTurn - 1].GetComponent<Combatant>().StartTurn();
    }
}
