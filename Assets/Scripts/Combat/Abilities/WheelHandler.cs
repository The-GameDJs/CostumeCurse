using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WheelHandler : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform RectTransform;
    private Quaternion LastRotation;
    float LastAngle;
    bool FullRotation;
    
    public void Start() 
    {
        RectTransform = GetComponent<RectTransform>();
        FullRotation = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 dir = Input.mousePosition - RectTransform.position;
        LastRotation = RectTransform.rotation;
        LastAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        //TODO: disable going counter-clockwise
        Vector2 dir = Input.mousePosition - RectTransform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        RectTransform.rotation = LastRotation * Quaternion.AngleAxis(angle - LastAngle, Vector3.forward);
        Debug.Log(RectTransform.rotation);
        if(RectTransform.rotation.z <= 0)
        {
            FullRotation = false;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }

    private void IncrementRotation()
    {
        if(!FullRotation)
        {
            FullRotation = true;
        }
    }
}
