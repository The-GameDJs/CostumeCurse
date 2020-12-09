﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSelect : MonoBehaviour
{
    public GameObject SelectedObject;
    private TargetSelector TargetSelector;
    public LayerMask LayerMask;
    public bool IsTargetSelected = false;
    public bool IsRegrettingDecision = false;
    public bool IsSingleTargetting = false;

    void Start()
    {
        TargetSelector = GetComponent<TargetSelector>();
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, LayerMask)) {
            GameObject hitObject = hit.transform.gameObject;
            SelectTarget(hitObject);
        }
        else 
        {
            ClearSelection();
        }

        if (Input.GetButtonDown("Fast Forward"))
        {
            IsRegrettingDecision = true;
        }

        if (SelectedObject != null && Input.GetButtonDown("Action Command"))
        {
            Debug.Log("Selected target");
            
            if (IsSingleTargetting)
                TargetSelector.ChooseTarget(SelectedObject);

            IsTargetSelected = true;
        }
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
        }
    }

    void ClearSelection()
    {
        SelectedObject = null;
    }
}
