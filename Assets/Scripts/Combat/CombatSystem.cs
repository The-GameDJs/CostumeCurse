using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CombatSystem : MonoBehaviour
{
    public List<GameObject> Combatants;
    private int CurrentCombatantTurn;

    [SerializeField]
    private GameObject Sield;
    [SerializeField]
    private GameObject Ganiel;

    void Start()
    {

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
    
    public void StartCombat(GameObject[] allies, GameObject[] enemies)
    {
        CurrentCombatantTurn = 0;
        
        Combatants = new List<GameObject>();
        foreach (GameObject ally in allies)
            Combatants.Add(ally);
        foreach (GameObject enemy in enemies)
            Combatants.Add(enemy);
        Combatants.Sort(SortByTurnPriority);

        Combatants[CurrentCombatantTurn].GetComponent<Combatant>().StartTurn();
    }

    public void EndTurn(GameObject combatant)
    {
        Debug.Log($"A Combantant has finished their turn!");

        CurrentCombatantTurn++;

        Combatants[CurrentCombatantTurn].GetComponent<Combatant>().StartTurn();
    }

    // In favor of direct communication between Combantants, and the CombantantIsDoneTurn method
    // Leaving this for now in case!
    //public void OnNotify(CharacterEvent gameEvent)
    //{
    //    if(gameEvent.isTurnCompleted == true)
    //    {
    //        combatState = CombatState.TurnEnd;
    //    }
    //}
}
