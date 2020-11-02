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

    private bool CombatStarted;


    void OnTriggerEnter(Collider other)
    {
        if (CombatStarted)
            return;

        if (other.gameObject.tag == "Player")
        {
            CombatStarted = true;
            Debug.Log("Entered combat zone");

            for (int i = 0; i < EnemyPositions.Length; i++)
            {
                Enemies[i].transform.position = EnemyPositions[i].transform.position;
            }

            for (int j = 0; j < PlayerPositions.Length; j++)
            {
                Players[j].GetComponent<CharacterController>().enabled = !Players[j].GetComponent<CharacterController>().enabled;
                Players[j].GetComponent<Player>().enabled = !Players[j].GetComponent<Player>().enabled;
                Players[j].SetActive(false);
                Players[j].transform.position = PlayerPositions[j].transform.position;
                Players[j].SetActive(true);
            }

        }
    }

    void OnTriggerExit(Collider other)
    {
        CombatStarted = false;

        for (int j = 0; j < PlayerPositions.Length; j++)
        {
            Players[j].GetComponent<CharacterController>().enabled = !Players[j].GetComponent<CharacterController>().enabled;
            Players[j].GetComponent<Player>().enabled = !Players[j].GetComponent<Player>().enabled;
        }
    }
}
