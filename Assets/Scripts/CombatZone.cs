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
    [SerializeField]
    private float MovementTime;
    private Vector3[] InitalPositionsEnemies;
    private Vector3[] InitalPositionsPlayers;

    protected CombatSystem CombatSystem;
    public CameraArea CameraArea;
    private Timer Timer;

    private bool CombatStarted;

    public void Start()
    {
        CombatSystem = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();
        CameraArea = GetComponentInChildren<CameraArea>();
        Timer = GetComponent<Timer>();
        
        Players = new GameObject[2];
        Players[0] = GameObject.Find("Sield");
        Players[1] = GameObject.Find("Ganiel");
    }

    public void Update()
    {
        if(Timer.IsFinished())
        {
            Timer.ResetTimer();
            StartCombat();
        }

        if (!Timer.IsInProgress())
            return;

        SetCombatPositionsUpdate();
    }

    void OnTriggerEnter(Collider other)
    {
        if (CombatStarted)
            return;

        if (other.gameObject.CompareTag("Player"))
        {
            Timer.StartTimer(MovementTime);
            DisablePlayerMovement();
            SetInitialCombatPositions();
        }
    }

    private void SetInitialCombatPositions()
    {
        CombatStarted = true;
        InitalPositionsEnemies = new Vector3[Enemies.Length];
        InitalPositionsPlayers = new Vector3[Players.Length];

        for (int i = 0; i < InitalPositionsEnemies.Length; i++)
            InitalPositionsEnemies[i] = Enemies[i].transform.position;

        for (int i = 0; i < InitalPositionsPlayers.Length; i++)
            InitalPositionsPlayers[i] = Players[i].transform.position;

    }

    private void DisablePlayerMovement()
    {
        if (Timer.IsFinished())
            return;


        for (int i = 0; i < PlayerPositions.Length; i++)
        {
            GameObject player = Players[i];
            player.GetComponent<Player>().DisableMovement();
        }
    }

    public void SetCombatPositionsUpdate()
    {
        for (int i = 0; i < EnemyPositions.Length; i++)
        {
            Enemies[i].transform.position = Vector3.Lerp(InitalPositionsEnemies[i],
                                                         EnemyPositions[i].transform.position,
                                                         Timer.GetProgress() / MovementTime);

            Vector3 direction = (EnemyPositions[i].transform.position - Enemies[i].transform.position).normalized;
            Enemies[i].transform.rotation = Quaternion.Lerp(Enemies[i].transform.rotation,
                                                            Quaternion.LookRotation(direction), 
                                                            Timer.GetProgress() / MovementTime);
        }

        for (int i = 0; i < PlayerPositions.Length; i++)
        {
            Players[i].transform.position = Vector3.Lerp(InitalPositionsPlayers[i],
                                                         PlayerPositions[i].transform.position,
                                                         Timer.GetProgress() / MovementTime);

            Vector3 direction = (PlayerPositions[i].transform.position - Players[i].transform.position).normalized;
            Players[i].transform.rotation = Quaternion.Lerp(Players[i].transform.rotation, 
                                                            Quaternion.LookRotation(direction),
                                                            Timer.GetProgress() / MovementTime);
        }
    }

    public void DestroyCombatZone()
    {
        Debug.Log("Destroying this Combat Zone!");
        CombatStarted = false;

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

    private void StartCombat()
    {
        foreach (var enemy in Enemies)
            enemy.GetComponent<Combatant>().EnterCombat();
        foreach (var player in Players)
            player.GetComponent<Combatant>().EnterCombat();

        CombatSystem.StartCombat(this.gameObject, Players, Enemies);
    }

}
