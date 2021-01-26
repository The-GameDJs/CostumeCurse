using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PimpkinHead : MonoBehaviour
{

    private bool IsHit;
    private RectTransform PimpkinBody;


    void Start()
    {
        IsHit = false;
        PimpkinBody = GetComponent<RectTransform>();
    }

    void Update()
    {
        /*    
        RectTransform mrect = GetComponent<RectTransform>();
        Vector2 apos = mrect.anchoredPosition;
        float xpos = apos.x;
        xpos = Mathf.Clamp(xpos, 0, Screen.width - mrect.sizeDelta.x);
        apos.x = xpos;
        mrect.anchoredPosition = apos;
        */
    }

    public void DestroyPimpkin()
    {
        gameObject.SetActive(false);
        IsHit = true;
    }

    public void ResetPimpkinValues()
    {
        IsHit = false;
    }
}
