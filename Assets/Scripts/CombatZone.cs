using System;
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
    protected CombatSystem CombatSystem;


    private bool CombatStarted;

    public void Start()
    {
        CombatSystem = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (CombatStarted)
            return;

        if (other.gameObject.CompareTag("Player"))
        {
            InitiateCombat();
        }
    }

    private void InitiateCombat()
    {
        CombatStarted = true;

        SetCombatPositions();

        CombatSystem.StartCombat(this.gameObject, Players, Enemies);
    }

    private void SetCombatPositions()
    {
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
    }

    public void DestroyCombatZone()
    {
        Debug.Log("Combat has Ended");
        foreach(GameObject player in Players)
        {
            player.GetComponent<CharacterController>().enabled = true;
            player.GetComponent<Player>().enabled = true;
            player.GetComponentInChildren<Costume>().DisplayAbilities(false);
        }

        foreach (GameObject enemy in Enemies)
        {
            Destroy(enemy.GetComponent<Combatant>().HealthBar);
            Destroy(enemy);
        }

        Destroy(gameObject);
    }
}
