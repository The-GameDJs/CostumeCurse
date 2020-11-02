using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    private enum CombatState { Inactive, Start, End, Player, Enemy }
    private enum PlayerTurn { Sield, Ganiel }

    private CombatState combatState = CombatState.Inactive;
    private PlayerTurn playerTurn = PlayerTurn.Sield;

    [SerializeField]
    private GameObject sieldGO;
    private PlayableCharacter sield;
    //[SerializeField]
    //private GameObject ganiel;


    // Start is called before the first frame update
    void Start()
    {
        sield = sieldGO.GetComponent<PlayableCharacter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            InitiateCombat();

        if (combatState == CombatState.Start)
        {

        }

        if (combatState == CombatState.Player)
        {
            

            // show UI of player
        }

        if (combatState == CombatState.Enemy)
        {

        }
    }

    /*
     * Invokable by the CombatZone
    */
    public void InitiateCombat()
    {
        combatState = CombatState.Start;

        Debug.Log("Start of our test");
        Debug.Log($"Test concluded with result: {sield.ReturnablePlayTurn()}");
        //sield.PlayTurn();
        // ResultOfTurn resultOfTurn = sield.PlayTurn();
    }
}
