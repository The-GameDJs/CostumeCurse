using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static Vector3 InputDirection => _inputDirection;
    public static bool HasPressedActionCommand => _hasPressedActionCommand;
    public static string CurrentControlScheme => _currentControlScheme;
    
    private PlayerInput _inputAction;
    private static Vector3 _inputDirection;
    private static bool _hasPressedActionCommand;
    private static string _currentControlScheme;

    public static Action<bool> ActionCommandPressed;
    public static Action<bool> BackButtonPressed;
    public static Action<Vector2> ControllerMoved;
    public static Action<Vector2> JoystickTapped;
    
    void Start()
    {
        _inputAction = GetComponent<PlayerInput>();
        _inputAction.onControlsChanged += OnControlSchemeChanged;
    }

    private void OnControlSchemeChanged(PlayerInput input)
    {
        _currentControlScheme = input.currentControlScheme;
        Debug.Log($"Current control scheme: {_currentControlScheme}");
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // Used for determining if the user tapped the joystick/dpad
            Vector2 input = context.ReadValue<Vector2>();
        
            JoystickTapped?.Invoke(input);
        }
        else if (context.performed)
        {
            // Used for constant controller updates
            Vector2 input = context.ReadValue<Vector2>();
            _inputDirection = new Vector3(input.x, 0f, input.y).normalized;
            ControllerMoved?.Invoke(input);
        }
        else if(context.canceled)
        {
            // Used if the user let go of the joystick
            _inputDirection = Vector3.zero;
            ControllerMoved?.Invoke(_inputDirection);
            JoystickTapped?.Invoke(Vector3.zero);
        }
    }

    public void OnActionCommand(InputAction.CallbackContext context)
    {
        ActionCommandPressed?.Invoke(context.performed);
        _hasPressedActionCommand = context.performed;
    }

    public void OnBackButton(InputAction.CallbackContext context)
    {
        BackButtonPressed?.Invoke(context.started);
    }

    private void OnDestroy()
    {
        _inputAction.onControlsChanged -= OnControlSchemeChanged;
    }
}
