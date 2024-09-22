using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControllerSelect : MonoBehaviour
{
    [SerializeField] private GameObject TargetIndicator;
    
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
        TargetIndicator.SetActive(false);
    }

    private void OnControllerMoved(Vector2 input)
    {
        _controllerInput = input;
        
        if (_controllerInput.x > 0.1f)
        {
            _currentSelection++;

            if (_currentSelection > SelectableObjects.Count - 1)
                _currentSelection = SelectableObjects.Count - 1;
        }
        else if (_controllerInput.x < -0.1f)
        {
            _currentSelection--;
            if (_currentSelection < 0)
                _currentSelection = 0;
        }
        
        SelectTarget(SelectableObjects[_currentSelection]);
    }

    private void OnActionCommandPressed(bool hasButtonPressed)
    {
        if (!hasButtonPressed)
            return;
        
        Debug.Log("Selected target");
        TargetIndicator.SetActive(false);
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
        TargetIndicator.SetActive(false);
    }

    public void SetSelectableObjects(List<GameObject> enemies, List<GameObject> objects)
    {
        SelectableObjects = new List<GameObject>(enemies);
        SelectableObjects.AddRange(objects);
        SelectTarget(enemies.First(x => x.GetComponent<EnemyCombatant>().IsAlive));
    }

    void SelectTarget(GameObject target)
    {
        if(target.GetComponent<Combatant>().IsAlive)
        {
            if (_selectedObject != null)
            {
                if (target == _selectedObject)
                    return;

                ClearSelection();
            }

            _selectedObject = target;

            Renderer[] renderers = _selectedObject.GetComponentsInChildren<Renderer>();
            foreach(Renderer r in renderers) {
                Material m = r.material;
                m.color = Color.red;
                r.material = m;
            }
        }
    }
    
    void ClearSelection()
    {
        if(_selectedObject == null)
        {
            TargetIndicator.SetActive(false);
            return;
        }

        Renderer[] renderers = _selectedObject.GetComponentsInChildren<Renderer>();
        foreach(Renderer r in renderers) {
            Material m = r.material;
            m.color = Color.white;
            r.material = m;
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
