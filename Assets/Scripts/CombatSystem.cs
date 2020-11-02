using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    // Not sure if keeping states
    private enum CombatState { CombatEnd, TurnStart, TurnEnd }
    private enum PlayerTurn { Sield, Ganiel }

    private CombatState combatState;
    private PlayerTurn playerTurn = PlayerTurn.Sield;

    [SerializeField]
    private GameObject sieldGO;

    private PlayableCharacter sield;

    private List<Character> CharacterOrder;
    private int CurrentCharacterTurnIndex;

    //[SerializeField]
    //private GameObject ganiel;


    // Start is called before the first frame update
    void Start()
    {
        sield = sieldGO.GetComponent<PlayableCharacter>();
        CharacterOrder = new List<Character>();
    }

    // Update is called once per frame
    void Update()
    {
        if (combatState == CombatState.TurnStart)
        {
            CharacterOrder[CurrentCharacterTurnIndex].PlayTurn();
        }

        if (combatState == CombatState.TurnEnd)
        {
            
        }

        if (combatState == CombatState.CombatEnd)
        {

        }
    }

    /*
     * Invokable by the CombatZone
    */
    public void InitiateCombat(GameObject[] players, GameObject[] enemies)
    {
        CharacterOrder.Clear();

        // Will add to the list in the same order as in the inspector
        foreach (GameObject character in players)
        {
            CharacterOrder.Add(character.GetComponent<PlayableCharacter>());
        }

        foreach (GameObject character in enemies)
        {
            CharacterOrder.Add(character.GetComponent<EnemyCharacter>());
        }

        CurrentCharacterTurnIndex = 0;
        combatState = CombatState.TurnStart;

        Debug.Log("Start of our test");
        Debug.Log($"Test concluded with result: {sield.ReturnablePlayTurn()}");
        //sield.PlayTurn();
        // ResultOfTurn resultOfTurn = sield.PlayTurn();
    }

}
