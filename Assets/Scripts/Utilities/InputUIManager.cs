using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputUIType
{
    Joystick,
    DPad,
    ButtonSouth,
    Arrows, // or WASD
    MouseLeftClick
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
    
    public RectTransform JoystickUI => _joystickUI;
    public RectTransform ButtonSouthUI => _buttonSouthUI;

    public void SetActiveUIButton(InputUIType inputType, PointsBar pointsBar, bool setActive)
    {
        var input = GetInputRectFromType(inputType);
        input.transform.position = setActive ? pointsBar.InputUIAnchor.position : Vector3.zero;
        input.gameObject.SetActive(setActive);
    }

    public void SetJoystickUIButton(PointsBar pointsBar, bool setActive, string animation = "Default")
    {
        _joystickUI.transform.position = setActive ? pointsBar.InputUIAnchor.position : Vector3.zero;
        if (!setActive)
        {
            _joystickUIAnimator.Play("Default");
            _joystickUIAnimator.Rebind();
            _joystickUIAnimator.Update(0.0f);
        }
        _joystickUI.gameObject.SetActive(setActive);
        if (setActive)
        {
            _joystickUIAnimator.Play("Default");
        }
    }
    
    public void SetGamepadSouthUIButton(PointsBar pointsBar, bool setActive, string animation = "Default")
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

    public RectTransform GetInputRectFromType(InputUIType inputType)
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
