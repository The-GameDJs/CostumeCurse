using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class CombatZone : MonoBehaviour
{
    [SerializeField]
    private GameObject[] Enemies;
    public GameObject[] GetEnemies => Enemies;
    [SerializeField]
    private GameObject[] EnemyPositions;
    private GameObject[] Players;

    [SerializeField] private GameObject[] TargettableObjects;

    public GameObject[] GetTargettableObjects => TargettableObjects;
    
    [SerializeField]
    private GameObject[] PlayerPositions;
    [SerializeField]
    private float MovementTime;

    public Vector3[] EnemiesInitialPosition { get; private set; }
    public Quaternion[] EnemiesInitialRotation { get; private set; }

    private Vector3[] InitalPositionsPlayers;

    protected CombatSystem CombatSystem;
    [SerializeField] private CinemachineVirtualCamera _combatCinemachineCamera;
    public CinemachineVirtualCamera CombatCinemachineCamera => _combatCinemachineCamera;
    
    [SerializeField] private Transform CheckpointResetPosition;
    [SerializeField] private Collider CombatCollider;
    public Transform CheckpointPosition => CheckpointResetPosition;
    private Timer Timer;

    private bool CombatStarted;

    [SerializeField] private bool IsBossBattle;

    [SerializeField] private bool ShouldTriggerBattleEvent;
    
    public bool IsBoss => IsBossBattle;

    public static Action<int> BattleEnded;

    public void Start()
    {
        CombatSystem = GameObject.FindGameObjectWithTag("CombatSystem").GetComponent<CombatSystem>();
        Timer = GetComponent<Timer>();
        //CheckpointResetPosition.parent = null;
        
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
        if (CombatStarted) return;
        
        if (other.gameObject.TryGetComponent<Player>(out var player) && player.GetIsMainPlayer)
        {
            StartCoroutine(EnterBattleSequence());
        }
    }

    private IEnumerator EnterBattleSequence()
    {
        CinemachineCameraRig.Instance.SetCinemachineWorldCamera(CinemachineCameraRig.Instance.CurrentCinemachineCamera);
        CinemachineCameraRig.Instance.SetCinemachineCamera(_combatCinemachineCamera);
        CinemachineCameraRig.Instance.ChangeCinemachineBrainBlendTime(1.0f);
        DisablePlayerMovement();
        
        MusicManager.Instance.StopAllMusic();

        yield return new WaitForSeconds(0.5f);
        
        MusicManager.Instance.EnterCombatMode();
        
        Timer.StartTimer(MovementTime);
        SetInitialCombatPositions();
    }

    private void SetInitialCombatPositions()
    {
        CombatStarted = true;
        EnemiesInitialPosition = new Vector3[Enemies.Length];
        EnemiesInitialRotation = new Quaternion[Enemies.Length];
        InitalPositionsPlayers = new Vector3[Players.Length];

        for (int i = 0; i < EnemiesInitialPosition.Length; i++)
            EnemiesInitialPosition[i] = Enemies[i].transform.position;
        
        for (int i = 0; i < EnemiesInitialRotation.Length; i++)
            EnemiesInitialRotation[i] = Enemies[i].transform.rotation;

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
        float progress = Timer.GetProgress() / MovementTime;
        
        for (int i = 0; i < EnemyPositions.Length; i++)
        {
            // Interpolating the X and Z positions linearly
            var startPos = EnemiesInitialPosition[i];
            var targetPos = EnemyPositions[i].transform.position;
            var currentPos = Vector3.Lerp(startPos, targetPos, progress);

            // Apply the jump using the AnimationCurve
            // Assuming the initial y is at ground level, and the curve adds a jump effect
            var jumpHeight = CombatSystem.MovementAnimCurve.Evaluate(progress); // Jump height based on curve
            currentPos.y = Mathf.Lerp(startPos.y, targetPos.y, progress) + jumpHeight;

            // Update the enemy's position
            Enemies[i].transform.position = currentPos;

            var direction = (EnemyPositions[i].transform.position - Enemies[i].transform.position).normalized;
            Enemies[i].transform.rotation = Quaternion.Lerp(Enemies[i].transform.rotation,
                                                            Quaternion.LookRotation(direction), 
                                                            progress);
        }

        for (int i = 0; i < PlayerPositions.Length; i++)
        {
            // Interpolating the X and Z positions linearly
            var startPos = InitalPositionsPlayers[i];
            var targetPos = PlayerPositions[i].transform.position;
            var currentPos = Vector3.Lerp(startPos, targetPos, progress);
            
            var jumpHeight = CombatSystem.MovementAnimCurve.Evaluate(progress);
            currentPos.y = Mathf.Lerp(startPos.y, targetPos.y, progress) + jumpHeight;
            
            // Update the player's position
            Players[i].transform.position = currentPos;

            var direction = (PlayerPositions[i].transform.position - Players[i].transform.position).normalized;
            Players[i].transform.rotation = Quaternion.Lerp(Players[i].transform.rotation, 
                                                            Quaternion.LookRotation(direction),
                                                            progress);
        }
    }

    public void DestroyCombatZone(bool isAllyWin = true)
    {
        var message = isAllyWin ? "Destroying this Combat Zone!" : "Resetting this Combat Zone!";
        Debug.Log(message);
        CombatStarted = false;

        foreach (var player in Players)
        {
            player.GetComponent<AllyCombatant>().ExitCombat();
            player.GetComponent<Player>().EnableMovement();
        }
        
        if (ShouldTriggerBattleEvent && isAllyWin)
            BattleEnded?.Invoke(gameObject.GetInstanceID());

        foreach (var enemy in Enemies)
        {
            enemy.GetComponent<EnemyCombatant>().ExitCombat();
            if (isAllyWin)
            {
                Destroy(enemy);
            }
        }

        if (isAllyWin)
        {
            Destroy(gameObject);
        }
    }

    private void StartCombat()
    {
        foreach (var enemy in Enemies)
            enemy.GetComponent<Combatant>().EnterCombat();
        foreach (var player in Players)
            player.GetComponent<Combatant>().EnterCombat();
        foreach(var interactableObj in TargettableObjects)
            interactableObj.GetComponent<ObjectCombatant>().EnterCombat();
        
        CombatSystem.StartCombat(this.gameObject, Players, Enemies, TargettableObjects);
    }

    public void SetCombatColliderVisibility(bool isActive)
    {
        CombatCollider.enabled = isActive;
    }
}
