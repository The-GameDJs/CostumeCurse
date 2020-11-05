using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float movementSpeed = 1f;
    public float rotationSpeed = 1f;
    private CharacterController characterController;
    
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {      
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalMovement, 0f, verticalMovement).normalized;

        characterController.SimpleMove(direction * movementSpeed);

        if (!Mathf.Approximately(verticalMovement, 0.0f) || !Mathf.Approximately(horizontalMovement, 0.0f))
            transform.localRotation = Quaternion.RotateTowards(
                transform.localRotation,
                Quaternion.LookRotation(direction, Vector3.up),
                rotationSpeed * Time.deltaTime);
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
