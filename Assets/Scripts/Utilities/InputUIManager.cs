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
    [SerializeField] private RectTransform _arrowsUI;
    [SerializeField] private RectTransform _buttonSouthUI;
    
    public RectTransform JoystickUI => _joystickUI;

    public void SetActiveUIButton(InputUIType inputType, PointsBar pointsBar, bool setActive)
    {
        var input = GetInputRectFromType(inputType);
        input.transform.position = setActive ? pointsBar.InputUIAnchor.position : Vector3.zero;
        input.gameObject.SetActive(setActive);
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