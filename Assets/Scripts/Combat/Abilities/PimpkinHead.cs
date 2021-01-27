using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PimpkinHead : MonoBehaviour
{

    private bool IsHit;
    private RectTransform PimpkinBody;
    private Vector3 StartingPosition;
    [SerializeField] AudioSource PipmkinDeathSource;

    public Vector3 StartPosition
    {
        get
        {
            return StartingPosition;
        }
        set
        {
            StartingPosition = new Vector3(value.x, value.y, value.z);
        }
    }

    void Start()
    {
        IsHit = false;
        PimpkinBody = GetComponent<RectTransform>();
        StartPosition = PimpkinBody.transform.position;
    }

    void Update()
    {
        PimpkinBody.transform.position = StartPosition + new Vector3(Mathf.Sin(5f * Time.deltaTime), Mathf.Cos(2f * Time.deltaTime), 0);
        /*
        Vector2 apos = mrect.anchoredPosition;
        float xpos = apos.x;
        xpos = Mathf.Clamp(xpos, 0, Screen.width - mrect.sizeDelta.x);
        apos.x = xpos;
        mrect.anchoredPosition = apos;
        */
    }

    public void DestroyPimpkin()
    {
        PipmkinDeathSource.Play();
        gameObject.SetActive(false);
        IsHit = true;
    }

    public void ResetPimpkinValues()
    {
        IsHit = false;
    }
}
