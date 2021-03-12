using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    
    private Vector3 OriginalPos;
    private bool IsInside = false;
    private RectTransform RectTransform;
    private CanvasGroup CanvasGroup;
    private Canvas Canvas;

    private void Start()
    {
        Canvas = GetComponentInParent<Canvas>();
        RectTransform = GetComponent<RectTransform>();
        CanvasGroup = GetComponent<CanvasGroup>();
    }

    public void InitializeStartingPosition()
    {
        OriginalPos = this.gameObject.transform.position;
    }

    public void ResetPosition()
    {
        Debug.Log("ResetPosition");
        this.gameObject.transform.position = OriginalPos;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        IsInside = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        CanvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // TODO: possibly snap back to original position ?
        if (!IsInside) {
            Vector2 delta = eventData.delta;
            this.gameObject.transform.position += new Vector3(delta.x, delta.y, 0);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        CanvasGroup.blocksRaycasts = true;
        gameObject.transform.position = OriginalPos;
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
