﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// aight so what we want? 
    // if MC, move according to controls
    // if !MC: 
        // if outside MC following position, get there quicker than movementspeed
        // if very near position, normal follow

public class Player : MonoBehaviour
{
    [SerializeField]
    private bool IsMainPlayer = false;
    [SerializeField]
    private float MovementSpeed = 1f;
    [SerializeField]
    private float RotationSpeed = 1f;
    [SerializeField]
    private float FollowDistance = 5f;
    [SerializeField]
    private float CatchUpSpeed = 2f; // ? TODO

    private Vector3 TargetPosition;
    private Quaternion TargetRotation;
    private Vector3 Direction;



    private CharacterController CharacterController;
    private Player MainPlayer;
    private Timer Timer;

    // Start is called before the first frame update
    void Start()
    {
        CharacterController = GetComponent<CharacterController>();
        var foundPlayers = GameObject.FindObjectsOfType<Player>();
        Debug.Log(foundPlayers.Length);
        MainPlayer = Array.Find(foundPlayers, player => player.IsMainPlayer);
        Timer = GetComponent<Timer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsMainPlayer)
            UpdateMainCharacterMovement();
        else
            UpdateSecondaryCharacterMovement();
    }

    private void UpdateSecondaryCharacterMovement()
    {
        if (!Timer.IsInProgress())
        {
            Timer.StartTimer(.01f);

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

        if (Vector3.Distance(transform.position, TargetPosition) <= 2f)
            return;

        CharacterController.SimpleMove(Direction * CatchUpSpeed);
        transform.localRotation = Quaternion.RotateTowards(
            transform.localRotation,
            TargetRotation,
            RotationSpeed * Time.deltaTime);
    }

    private void UpdateMainCharacterMovement()
    {      
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalMovement, 0f, verticalMovement).normalized;
        direction = Camera.main.transform.TransformDirection(direction);
        direction.y = 0;

        CharacterController.SimpleMove(direction * MovementSpeed);

        if (!Mathf.Approximately(verticalMovement, 0.0f) || !Mathf.Approximately(horizontalMovement, 0.0f))
            transform.localRotation = Quaternion.RotateTowards(
                transform.localRotation,
                Quaternion.LookRotation(direction, Vector3.up),
                RotationSpeed * Time.deltaTime);
    }

    public void EnableMovement()
    {
        GetComponent<CharacterController>().enabled = true;
        GetComponent<Player>().enabled = true;
    }
    public void DisableMovement()
    {
        GetComponent<CharacterController>().enabled = false;
        GetComponent<Player>().enabled = false;
    }
}
