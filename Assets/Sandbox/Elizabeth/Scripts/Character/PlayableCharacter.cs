using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableCharacter : Character
{
    [SerializeField] Costume costume;

    void Start()
    {
        
        costume.DisplayAbilities(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space pressed");
            costume.DisplayAbilities(true);
        }
    }

    public override void PlayTurn()
    {
        // TODO: Implement turn system
    }

}
