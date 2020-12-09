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
            HandleCookingPot(eventData);
        }
    }

    private void HandleCookingPot(PointerEventData eventData)
    {
        if (!eventData.pointerDrag.GetComponent<DragAndDrop>().GetIsInside())
        {
            if(eventData.pointerDrag.name == "Sweet")
            {
                SweetsDropped++;
                Debug.Log("Sweets Dropped:" + SweetsDropped);
                eventData.pointerDrag.GetComponent<DragAndDrop>().SetIsInside(true);
            }
            else if(eventData.pointerDrag.name == "Rotten")
            {
                RotsDropped++;
                eventData.pointerDrag.GetComponent<DragAndDrop>().SetIsInside(true);
            }
        }

        eventData.pointerDrag.SetActive(false);
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
