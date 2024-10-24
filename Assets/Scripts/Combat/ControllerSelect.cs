using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControllerSelect : MonoBehaviour
{
    private List<GameObject> SelectableObjects;

    private GameObject _selectedObject;

    public GameObject SelectedObject
    {
        get => _selectedObject;
        set => _selectedObject = value;
    }

    private TargetSelector TargetSelector;
    public bool IsTargetSelected = false;
    public bool IsRegrettingDecision = false;
    public bool IsSingleTargetting = false;
    private int _currentSelection;
    private Vector2 _controllerInput;
    
    void Start()
    {
        TargetSelector = GetComponent<TargetSelector>();
    }

    private void OnControllerMoved(Vector2 input)
    {
        _controllerInput = input;
        
        if (_controllerInput.x > 0.1f)
        {
            _currentSelection++;

            if (_currentSelection > SelectableObjects.Count - 1)
                _currentSelection = 0;
        }
        else if (_controllerInput.x < -0.1f)
        {
            _currentSelection--;
            if (_currentSelection < 0)
                _currentSelection = SelectableObjects.Count - 1;
        }
        
        SelectTarget(SelectableObjects[_currentSelection]);
    }

    private void OnActionCommandPressed(bool hasButtonPressed)
    {
        if (!hasButtonPressed)
            return;
        
        Debug.Log("Selected target");
        Renderer[] renderers = SelectedObject.GetComponentsInChildren<Renderer>();
        foreach(Renderer r in renderers) {
            Material m = r.material;
            m.color = Color.white;
            r.material = m;
        }
            
        if (IsSingleTargetting)
            TargetSelector.ChooseTarget(SelectedObject);

        IsTargetSelected = true;
    }

    private void OnBackButtonPressed(bool hasButtonPressed)
    {
        if (!hasButtonPressed)
            return;
        
        IsRegrettingDecision = true;
        ClearSelection();
    }

    public void SetSelectableObjects(List<GameObject> enemies, List<GameObject> objects)
    {
        _selectedObject = null;
        SelectableObjects = new List<GameObject>(enemies);
        SelectableObjects.AddRange(objects);
        SelectTarget(enemies.First(x => x.GetComponent<EnemyCombatant>().IsAlive));
    }

    void SelectTarget(GameObject target)
    {
        if(target.TryGetComponent<Combatant>(out var enemy) && enemy.IsAlive)
        {
            if (_selectedObject != null)
            {
                if (target == _selectedObject)
                    return;

                ClearSelection();
            }

            _selectedObject = target;

            if (!enemy.SelectorLight.gameObject.activeSelf)
            {
                enemy.SelectorLight.gameObject.SetActive(true);
            }
        }
    }
    
    void ClearSelection()
    {
        if(_selectedObject == null)
        {
            return;
        }

        var light = _selectedObject.GetComponent<Combatant>().SelectorLight.gameObject;
        if (light.activeSelf)
        {
            light.SetActive(false);
        }

        _selectedObject = null;
    }

    private void OnEnable()
    {
        InputManager.JoystickTapped += OnControllerMoved;
        InputManager.ActionCommandPressed += OnActionCommandPressed;
        InputManager.BackButtonPressed += OnBackButtonPressed;
    }

    private void OnDisable()
    {
        InputManager.JoystickTapped -= OnControllerMoved;
        InputManager.ActionCommandPressed -= OnActionCommandPressed;
        InputManager.BackButtonPressed -= OnBackButtonPressed;
    }
}
