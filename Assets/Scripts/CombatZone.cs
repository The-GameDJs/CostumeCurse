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
    private GameObject[] Players;
    [SerializeField]
    private GameObject[] PlayerPositions;
    protected CombatSystem CombatSystem;

    public CameraArea CameraArea;


    private bool CombatStarted;

    public void Start()
    {
        CombatSystem = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();
        CameraArea = GetComponent<CameraArea>();
        
        if (Players == null)
        {
            Players = new GameObject[2];
            Players[0] = GameObject.Find("Sield");
            Players[1] = GameObject.Find("Ganiel");
        }

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
            enemy.GetComponent<Combatant>().EnterCombat();
        }

        for (int j = 0; j < PlayerPositions.Length; j++)
        {
            GameObject player = Players[j];
            player.GetComponent<Combatant>().EnterCombat();
            player.GetComponent<Player>().DisableMovement();

            player.SetActive(false);
            player.transform.position = PlayerPositions[j].transform.position;
            player.SetActive(true);
        }
    }

    public void DestroyCombatZone()
    {
        Debug.Log("Destroying this Combat Zone!");

        foreach(GameObject player in Players)
        {
            player.GetComponent<Combatant>().ExitCombat();
            player.GetComponent<Player>().EnableMovement();
        }

        foreach (GameObject enemy in Enemies)
        {
            enemy.GetComponent<Combatant>().ExitCombat();
            Destroy(enemy);
        }

        Destroy(gameObject);
    }

}
