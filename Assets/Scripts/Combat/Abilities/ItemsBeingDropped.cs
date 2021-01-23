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
    private bool IsFilled;



    public void Start()
    {
        Collider2D = GetComponent<Collider2D>();
        IsFilled = false;
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

            else if(eventData.pointerDrag.name == "Bullet" && !IsFilled)
            {
                Debug.Log("Bullet Dropped");
                eventData.pointerDrag.GetComponent<DragAndDrop>().SetIsInside(true);
                IsFilled = true;
                GetComponent<Image>().sprite = GameObject.Find("CowboyChefCostume").GetComponent<Revolver>().BulletFilled;
            }
        }

        eventData.pointerDrag.SetActive(false);
    }

    public void ResetConfectionValues()
    {
        SweetsDropped = 0;
        RotsDropped = 0;
    }

    public void ResetRevolverValues()
    {
        IsFilled = false;
        GetComponent<Image>().sprite = GetComponentInParent<Revolver>().BulletEmpty;
    }

    public int GetSweetsDropped()
    {
        return SweetsDropped;
    }

    public int GetRotsDropped()
    {
        return RotsDropped;
    }

    public bool IsClipFilled()
    {
        return IsFilled;
    }
}
