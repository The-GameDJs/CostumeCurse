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
            GameObject enemy = Enemies[i];
            enemy.transform.position = EnemyPositions[i].transform.position;
            enemy.GetComponent<Combatant>().ShowHealth();
        }

        for (int j = 0; j < PlayerPositions.Length; j++)
        {
            GameObject player = Players[j];
            player.GetComponent<CharacterController>().enabled = false;
            player.GetComponent<Combatant>().ShowHealth();
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
            EnableMovement(player);
        }

        foreach (GameObject enemy in Enemies)
        {
            enemy.GetComponent<Combatant>().RemoveHealthBars();
            Destroy(enemy);
        }

        Destroy(gameObject);
    }

    private static void EnableMovement(GameObject player)
    {
        player.GetComponent<CharacterController>().enabled = true;
        player.GetComponent<Combatant>().RemoveHealthBars();
        player.GetComponent<Player>().enabled = true;
        player.GetComponentInChildren<Costume>().DisplayAbilities(false);
    }
}
