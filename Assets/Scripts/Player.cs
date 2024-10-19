using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField]
    private bool IsMainPlayer = false;
    public bool GetIsMainPlayer => IsMainPlayer;
    [SerializeField]
    private float MovementSpeed = 1f;
    [SerializeField]
    private float RotationSpeed = 1f;
    [SerializeField]
    private float FollowDistance = 5f;
    [SerializeField]
    private float CatchUpSpeed = 2f; // ? TODO

    private float CurrentCatchUpSpeed;
    [SerializeField] private Collider PlayerCollider;

    private Vector3 TargetPosition;
    private Quaternion TargetRotation;
    private Vector3 Direction;

    private CandyCornManager CandyCornManager;

    private bool _isMovementDisabled;
    public bool IsMovementDisabled;

    private CharacterController CharacterController;
    private Player MainPlayer;
    private Timer Timer;
    private InputManager _inputSystem;

    void Start()
    {
        CharacterController = GetComponent<CharacterController>();
        CandyCornManager = GameObject.FindObjectOfType<CandyCornManager>();
        Timer = GetComponent<Timer>();
        _inputSystem = FindObjectOfType<InputManager>();

        var foundPlayers = GameObject.FindObjectsOfType<Player>();
        MainPlayer = Array.Find(foundPlayers, player => player.IsMainPlayer);
        CurrentCatchUpSpeed = CatchUpSpeed;

        var currentCheckpoint = Vector3.zero;
        
        // Ensure the Loading system occurs only in the Main Scene.
        // In the future, when the prologue scene becomes a little bit more extensive, add saving/loading system there too.
        if (SceneManager.GetActiveScene().name == "Main_Scene")
        {
            currentCheckpoint = new Vector3(SaveSystem.Load("Rest.x"), SaveSystem.Load("Rest.y"), SaveSystem.Load("Rest.z"));
        }
        
        transform.position = currentCheckpoint == Vector3.zero ? transform.position : currentCheckpoint;

    }

    void Update()
    {
        if (IsMovementDisabled)
            return;

        if (IsMainPlayer)
            UpdateMainCharacterMovement();
        else
            UpdateSecondaryCharacterMovement();
    }

    private void UpdateMainCharacterMovement()
    {
        Vector3 direction = InputManager.InputDirection;
        var horizontalMovement = direction.x;
        var verticalMovement = direction.z;
        direction = Camera.main.transform.TransformDirection(direction);
        direction.y = 0;
        
        direction = Vector3.ProjectOnPlane(direction, Vector3.up).normalized;

        CharacterController.SimpleMove(direction * MovementSpeed);

        if (!Mathf.Approximately(verticalMovement, 0.0f) || !Mathf.Approximately(horizontalMovement, 0.0f))
            transform.localRotation = Quaternion.RotateTowards(
                transform.localRotation,
                Quaternion.LookRotation(direction, Vector3.up),
                RotationSpeed * Time.deltaTime);
    }

    private void UpdateSecondaryCharacterMovement()
    {
        if (!Timer.IsInProgress())
        {
            Timer.StartTimer(.001f);

            if (MainPlayer == null)
            {
                var foundPlayers = GameObject.FindObjectsOfType<Player>();
                Debug.Log(foundPlayers.Length);
                MainPlayer = Array.Find(foundPlayers, player => player.IsMainPlayer);
            }

            TargetPosition = MainPlayer.gameObject.transform.position -
                MainPlayer.gameObject.transform.forward * FollowDistance;

            Direction = (TargetPosition - transform.position).normalized;

            TargetRotation = transform.localRotation;
            TargetRotation = Quaternion.LookRotation(Direction, Vector3.up);
            TargetRotation.x = TargetRotation.z = 0;
        }

        // Decelerate Second Player when it approaches the target position
        if (Vector3.Distance(transform.position, TargetPosition) <= 4f)
        {
            var distance = Vector3.Distance(transform.position, TargetPosition);
            CurrentCatchUpSpeed = (CatchUpSpeed / 2) * distance - CatchUpSpeed;
        }

        if (Vector3.Distance(transform.position, TargetPosition) >= 40f)
        {
            transform.position = TargetPosition;
            transform.localRotation = TargetRotation;
            return;
        }

        CharacterController.SimpleMove(Direction * CurrentCatchUpSpeed);
        transform.localRotation = Quaternion.RotateTowards(
            transform.localRotation,
            TargetRotation,
            RotationSpeed * Time.deltaTime);
    }

    public Vector3 GetTargetPosition()
    {
        return TargetPosition;
    }

    public Quaternion GetTargetRotation()
    {
        return TargetRotation;
    }

    public void EnableMovement()
    {
        GetComponent<CharacterController>().enabled = true;
        GetComponent<Player>().enabled = true;
        IsMovementDisabled = false;
    }
    public void DisableMovement()
    {
        GetComponent<CharacterController>().enabled = false;
        GetComponent<Player>().enabled = false;
        IsMovementDisabled = true;
    }

    public void SetColliderVisibility(bool isActive)
    {
        PlayerCollider.enabled = isActive;
    }
}
