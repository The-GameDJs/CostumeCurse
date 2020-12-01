using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    
    private Vector3 OriginalPos;
    private bool IsInside = false;
    private RectTransform RectTransform;
    private CanvasGroup CanvasGroup;

    private void Start()
    {
        RectTransform = GetComponent<RectTransform>();
        CanvasGroup = GetComponent<CanvasGroup>();
        OriginalPos = RectTransform.localPosition;
    }

    public void ResetPosition()
    {
        RectTransform.localPosition = OriginalPos;
        IsInside = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        CanvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // TODO: possibly snap back to original position ?
        if(!IsInside) // for sweets/rots
            RectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        CanvasGroup.blocksRaycasts = true;
    }

    public void SetIsInside(bool isInsideSlot)
    {
        IsInside = isInsideSlot;
    }

    public bool GetIsInside()
    {
        return IsInside;
    }
}
