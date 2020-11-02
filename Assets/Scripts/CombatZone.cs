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

    void Start()
    {
     
    }

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


            // other.GetComponent<CharacterController>().SimpleMove(playerPositions[0].transform.position - other.transform.position);
            other.gameObject.SetActive(false);
            other.transform.position = PlayerPositions[0].transform.position;
            other.gameObject.SetActive(true);

        }
    }

    void OnTriggerExit(Collider other)
    {
        CombatStarted = false;
    }
}
