using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSelect : MonoBehaviour
{
    public GameObject SelectedObject;
    private TargetSelector TargetSelector;
    public LayerMask LayerMask;
    public bool IsTargetSelected;

    void Start()
    {
        IsTargetSelected = false;
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

        if (SelectedObject != null && Input.GetButtonDown("Action Command"))
        {
            Debug.Log("Selected target");
            TargetSelector.ChooseTarget(SelectedObject);
            IsTargetSelected = true;
        }
    }

    void SelectTarget(GameObject obj)
    {
        if (SelectedObject != null)
        {
            if (obj == SelectedObject)
                return;

            ClearSelection();
        }

        SelectedObject = obj;
    }

    void ClearSelection()
    {
        SelectedObject = null;
    }
}
