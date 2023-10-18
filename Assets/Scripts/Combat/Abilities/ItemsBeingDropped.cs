using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

// This handles the GameObject that holds DragAndDrop items
// When a GameObject is dropped on it, it will snap to the center
public class ItemsBeingDropped : MonoBehaviour, IDropHandler
{
    private int SweetsDropped = 0;
    private int RotsDropped = 0;

    private Collider2D Collider2D;

    public void Start()
    {
        Collider2D = GetComponent<Collider2D>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("ON DROP!!");
        if (eventData.pointerDrag != null)
        {
            Debug.Log("Dropped object was: " + eventData.pointerDrag.name);
            Debug.Log("Dropped object pos: " + eventData.pointerDrag.gameObject.transform.position);
            HandleDroppedItems(eventData);
        }
    }

    private void HandleDroppedItems(PointerEventData eventData)
    {
        if (eventData.pointerDrag.TryGetComponent<DragAndDrop>(out var dropItem) && !dropItem.GetIsInside())
        {
            if(dropItem.DropTypeEnumPublic == DragAndDrop.DropType.Sweet)
            {
                SweetsDropped++;
                Debug.Log("Sweets Dropped:" + SweetsDropped);
            }

            else if(dropItem.DropTypeEnumPublic == DragAndDrop.DropType.Veggie)
            {
                RotsDropped++;
            }
            eventData.pointerDrag.GetComponent<DragAndDrop>().SetIsInside(true);
            eventData.pointerDrag.SetActive(false);
        }
    }

    public void ResetConfectionValues()
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
