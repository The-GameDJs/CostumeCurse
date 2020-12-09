using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetIndicator : MonoBehaviour
{
    private MouseSelect MouseSelector;
    private const float IndicatorScale = 1.25f;

    void Start()
    {
        MouseSelector = GameObject.FindGameObjectWithTag("TargetSelector").GetComponent<MouseSelect>();
    }

    void Update()
    {
        if(MouseSelector.SelectedObject != null) {
			Bounds bounds = MouseSelector.SelectedObject.GetComponent<Collider>().bounds;

			transform.position = new Vector3(bounds.center.x, MouseSelector.SelectedObject.transform.position.y, bounds.center.z);
			transform.localScale = bounds.size * IndicatorScale;
		}
    }
}
