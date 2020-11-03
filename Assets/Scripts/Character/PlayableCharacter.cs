using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableCharacter : Character
{
    [SerializeField] 
    Costume costume;
    [SerializeField] 
    CombatSystem combatSystem;

    void Start()
    {
        costume.DisplayAbilities(false);
        //combatSystem = 
    }

    void Update()
    {

    }

    public override void PlayTurn()
    {
        Debug.Log("Playing Turn");

        costume.DisplayAbilities(true);

        // TODO: Implement turn system

        // notify gamesystem
    }

    public bool ReturnablePlayTurn()
    {
        Debug.Log("Playing Turn");
        costume.DisplayAbilities(true);

        return true;
        // TODO: Implement turn system

        // notify gamesystem
    }

    
}