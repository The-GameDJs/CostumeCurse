using System.Collections;
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
    private GameObject TargetIndicator;

    void Start()
    {
        TargetSelector = GetComponent<TargetSelector>();
        TargetIndicator = GameObject.Find("TargetIndicator");
        TargetIndicator.SetActive(false);
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, LayerMask)) {
            GameObject hitObject = hit.transform.gameObject;
            SelectTarget(hitObject);
            TargetIndicator.SetActive(true);
        }
        else 
        {
            ClearSelection();
        }

        if (Input.GetButtonDown("Fast Forward"))
        {
            IsRegrettingDecision = true;
            TargetIndicator.SetActive(false);
        }

        if (SelectedObject != null && Input.GetButtonDown("Action Command"))
        {
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
}
