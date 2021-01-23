﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class Clip : MonoBehaviour, IDropHandler
{
    private Collider2D Collider2D;
    private bool IsFilled;

    [Header("Bullet Sprites")]

    [SerializeField] public Sprite BulletEmpty;
    [SerializeField] public Sprite BulletFilled;

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
            if (eventData.pointerDrag.name == "Bullet" && !IsFilled)
            {
                Debug.Log("Bullet Dropped");
                eventData.pointerDrag.GetComponent<DragAndDrop>().SetIsInside(true);
                IsFilled = true;
                GetComponent<Image>().sprite = BulletFilled;
                eventData.pointerDrag.SetActive(false);
            }
        }
    }

    public void ResetRevolverValues()
    {
        IsFilled = false;
        GetComponent<Image>().sprite = BulletEmpty;
    }

    public bool IsClipFilled()
    {
        return IsFilled;
    }

}
