using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour, IObserver
{
    // Not sure if keeping states
    private enum CombatState { CombatLose, CombatWin, TurnStart, TurnEnd }
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
            // Check if all players or all enemies are dead

            // If all players are dead, game over
            // Change combat state to CombatLose

            // If all enemies are dead, end of combat
            // Change combat state to CombatWin

            // Else, continue combat
            // Increase turn index (Make it circular) CurrentCharacterTurnIndex
            // Change combat state back to TurnStart
        }

        if (combatState == CombatState.CombatLose)
        {
            // Load game over screen
        }

        if (combatState == CombatState.CombatWin)
        {
            // Get rid of combat trigger
            // ??? Do we want to start each fight with full HP or not?
            // Revert back to overworld control
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

    public void OnNotify(CharacterEvent gameEvent)
    {
        if(gameEvent.isTurnCompleted == true)
        {
            combatState = CombatState.TurnEnd;
        }
    }
}
