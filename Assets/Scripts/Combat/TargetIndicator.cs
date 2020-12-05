using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetIndicator : MonoBehaviour
{
    private MouseSelect MouseSelector;
    private const float IndicatorScaler = 1.25f;

    void Start()
    {
        MouseSelector = GameObject.FindGameObjectWithTag("TargetSelector").GetComponent<MouseSelect>();
    }

    void Update()
    {
        if(MouseSelector.SelectedObject != null) {
			Bounds objBounds = MouseSelector.SelectedObject.GetComponent<Collider>().bounds;

			transform.position = new Vector3(objBounds.center.x, MouseSelector.SelectedObject.transform.position.y, objBounds.center.z);
			transform.localScale = objBounds.size * IndicatorScaler;
		}
    }
}
