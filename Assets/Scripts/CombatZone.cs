using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatZone : MonoBehaviour
{
    [SerializeField]
    private GameObject[] Enemies;
    [SerializeField]
    private GameObject[] EnemyPositions;
    [SerializeField]
    private GameObject[] Players;
    [SerializeField]
    private GameObject[] PlayerPositions;
    [SerializeField]
    private CombatSystem CombatSystem;


    private bool CombatStarted;

    void OnTriggerEnter(Collider other)
    {
        if (CombatStarted)
            return;

        if (other.gameObject.CompareTag("Player"))
        {
            CombatStarted = true;
            Debug.Log("Entered combat zone");
            
            // Initializing positions for combat
            for (int i = 0; i < EnemyPositions.Length; i++)
            {
                Enemies[i].transform.position = EnemyPositions[i].transform.position;
            }

            for (int j = 0; j < PlayerPositions.Length; j++)
            {
                GameObject player = Players[j];
                player.GetComponent<CharacterController>().enabled = false;
                player.GetComponent<Player>().enabled = false;
                player.SetActive(false);
                player.transform.position = PlayerPositions[j].transform.position;
                player.SetActive(true);
            }
            
            CombatSystem.InitiateCombat(Players, Enemies);
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        for (int j = 0; j < PlayerPositions.Length; j++)
        {
            Players[j].GetComponent<CharacterController>().enabled = true;
            Players[j].GetComponent<Player>().enabled = true;
        }
    }
}
