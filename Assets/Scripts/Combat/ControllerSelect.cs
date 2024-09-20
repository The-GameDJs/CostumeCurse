using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControllerSelect : MonoBehaviour
{
    [SerializeField] private GameObject TargetIndicator;
    
    private List<GameObject> SelectableObjects;
    public GameObject SelectedObject;
    private TargetSelector TargetSelector;
    public bool IsTargetSelected = false;
    public bool IsRegrettingDecision = false;
    public bool IsSingleTargetting = false;
    private int _currentSelection;
    private Vector2 _controllerInput;
    private float previousStickPosition;
    
    void Start()
    {
        TargetSelector = GetComponent<TargetSelector>();
        TargetIndicator.SetActive(false);
    }

    private void OnControllerMoved(Vector2 input)
    {
        _controllerInput = input;
        
        // This is needed because everytime the stick moves, this function gets called
        // This includes when the stick is being pulled back to the middle or when it is in the process of moving left or right
        // This statement avoids the selection from constantly scrolling through each frame and makes it more smoother
        if (Math.Abs(input.x) - Math.Abs(previousStickPosition) < 0.2f)
        {
            previousStickPosition = input.x;
            return;
        }

        previousStickPosition = input.x;
        
        if (_controllerInput.x > 0.88f)
        {
            _currentSelection++;

            if (_currentSelection > SelectableObjects.Count - 1)
                _currentSelection = SelectableObjects.Count - 1;
        }
        else if (_controllerInput.x < -0.88f)
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
            if (SelectedObject != null)
            {
                if (target == SelectedObject)
                    return;

                ClearSelection();
            }

            SelectedObject = target;

            Renderer[] renderers = SelectedObject.GetComponentsInChildren<Renderer>();
            foreach(Renderer r in renderers) {
                Material m = r.material;
                m.color = Color.red;
                r.material = m;
            }
        }
    }
    
    void ClearSelection()
    {
        if(SelectedObject == null)
        {
            TargetIndicator.SetActive(false);
            return;
        }

        Renderer[] renderers = SelectedObject.GetComponentsInChildren<Renderer>();
        foreach(Renderer r in renderers) {
            Material m = r.material;
            m.color = Color.white;
            r.material = m;
        }

        SelectedObject = null;
    }

    private void OnEnable()
    {
        InputManager.ControllerMoved += OnControllerMoved;
        InputManager.ActionCommandPressed += OnActionCommandPressed;
        InputManager.BackButtonPressed += OnBackButtonPressed;
    }

    private void OnDisable()
    {
        InputManager.ControllerMoved -= OnControllerMoved;
        InputManager.ActionCommandPressed -= OnActionCommandPressed;
        InputManager.BackButtonPressed -= OnBackButtonPressed;
    }
}
