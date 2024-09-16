using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public Vector3 InputDirection => _inputDirection;
    public bool HasPressedActionCommand => _hasPressedActionCommand; 
    
    private PlayerInput _inputAction;
    private Vector3 _inputDirection;
    private bool _hasPressedActionCommand;

    public static Action<bool> ActionCommandPressed;
    
    void Start()
    {
        _inputAction = GetComponent<PlayerInput>();
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        _inputDirection = new Vector3(input.x, 0f, input.y).normalized;
    }

    public void OnActionCommand(InputAction.CallbackContext context)
    {
        ActionCommandPressed?.Invoke(context.started);
        _hasPressedActionCommand = context.started;
    }
}
