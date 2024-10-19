using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputUIType
{
    Joystick,
    ButtonSouth,
    Arrows, // or WASD
}

public class InputUIManager : MonoBehaviour
{
    private static InputUIManager _instance;
    public static InputUIManager Instance 
    {
        get 
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<InputUIManager>();
            }

            return _instance;
        }
    }
    
    [SerializeField] private RectTransform _joystickUI;
    [SerializeField] private Animator _joystickUIAnimator;
    
    [SerializeField] private RectTransform _arrowsUI;
    [SerializeField] private Animator _arrowsUIAnimator;
    
    [SerializeField] private RectTransform _buttonSouthUI;
    [SerializeField] private Animator _buttonSouthUIAnimator;
    
    [SerializeField] private RectTransform _leftMouseUI;
    [SerializeField] private Animator _leftMouseUIAnimator;
    
    public RectTransform JoystickUI => _joystickUI;
    public RectTransform ArrowsUI => _arrowsUI;

    public void SetActiveUIButton(InputUIType inputType, PointsBar pointsBar, bool setActive)
    {
        var input = GetInputRectFromType(inputType);
        input.transform.position = setActive ? pointsBar.InputUIAnchor.position : Vector3.zero;
        input.gameObject.SetActive(setActive);
    }

    public void SetRotatingInputUIButton(PointsBar pointsBar, bool setActive, string animation = "Default")
    {
        var currentDevice = InputManager.CurrentControlScheme;

        if (currentDevice == "Gamepad")
        {
            _joystickUI.transform.position = setActive ? pointsBar.InputUIAnchor.position : Vector3.zero;
            if (!setActive)
            {
                _joystickUIAnimator.Play(animation);
                _joystickUIAnimator.Rebind();
                _joystickUIAnimator.Update(0.0f);
            }
            _joystickUI.gameObject.SetActive(setActive);
            if (setActive)
            {
                _joystickUIAnimator.Play(animation);
            }
        }
        else
        {
            _arrowsUI.transform.position = setActive ? pointsBar.InputUIAnchor.position : Vector3.zero;
            if (!setActive)
            {
                _arrowsUIAnimator.Play(animation);
                _arrowsUIAnimator.Rebind();
                _arrowsUIAnimator.Update(0.0f);
            }
            _arrowsUI.gameObject.SetActive(setActive);
            if (setActive)
            {
                _arrowsUIAnimator.Play(animation);
            }
        }
    }
    
    public void SetActionCommandUIButton(PointsBar pointsBar, bool setActive, string animation = "Default")
    {
        var currentDevice = InputManager.CurrentControlScheme;

        if (currentDevice == "Gamepad")
        {
            _buttonSouthUI.transform.position = setActive ? pointsBar.InputUIAnchor.position : Vector3.zero;
            if (!setActive)
            {
                _buttonSouthUIAnimator.Play(animation);
                _buttonSouthUIAnimator.Rebind();
                _buttonSouthUIAnimator.Update(0.0f);
            }
            _buttonSouthUI.gameObject.SetActive(setActive);
            if (setActive)
            {
                _buttonSouthUIAnimator.Play(animation);
            }
        }
        else
        {
            _leftMouseUI.transform.position = setActive ? pointsBar.InputUIAnchor.position : Vector3.zero;
            if (!setActive)
            {
                _leftMouseUIAnimator.Play(animation);
                _leftMouseUIAnimator.Rebind();
                _leftMouseUIAnimator.Update(0.0f);
            }
            _leftMouseUI.gameObject.SetActive(setActive);
            if (setActive)
            {
                _leftMouseUIAnimator.Play(animation);
            }
        }
        
    }

    private RectTransform GetInputRectFromType(InputUIType inputType)
    {
        return inputType switch
        {
            InputUIType.Joystick => _joystickUI,
            InputUIType.Arrows => _arrowsUI,
            InputUIType.ButtonSouth => _buttonSouthUI,
            _ => _buttonSouthUI
        };
    }
}
