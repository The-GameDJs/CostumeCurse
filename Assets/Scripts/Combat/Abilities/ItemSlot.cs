using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// This handles the GameObject that holds DragAndDrop items
// When a GameObject is dropped on it, it will snap to the center
public class ItemSlot : MonoBehaviour, IDropHandler
{
    private int SweetsDropped = 0;
    private int RotsDropped = 0;

    public void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag != null)
        {
            if(eventData.pointerDrag.GetComponent<BoxCollider2D>().IsTouching(GetComponent<BoxCollider2D>()))
            {
                eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
                HandleCookingPot(eventData);
            }
        }
    }

    private void HandleCookingPot(PointerEventData eventData)
    {
        if(!eventData.pointerDrag.GetComponent<DragAndDrop>().GetIsInside())
        {
            if(eventData.pointerDrag.name == "Sweet")
            {
                SweetsDropped++;
                eventData.pointerDrag.GetComponent<DragAndDrop>().SetIsInside(true);
            }
            else if(eventData.pointerDrag.name == "Rotten")
            {
                RotsDropped++;
                eventData.pointerDrag.GetComponent<DragAndDrop>().SetIsInside(true);
            }
        }
    }

    public void ResetValues()
    {
        SweetsDropped = 0;
        RotsDropped = 0;
    }

    public int GetSweetsDropped()
    {
        return SweetsDropped;
    }

    public int GetRotsDropped()
    {
        return RotsDropped;
    }
}
