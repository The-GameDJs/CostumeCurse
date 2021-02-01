using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PimpkinHead : MonoBehaviour
{

    private bool IsHit;
    private RectTransform PimpkinBody;
    private Vector3 StartingPosition;
    private int Switch = 0;
    private float randomIntX;
    private float randomIntY;
    [SerializeField] AudioSource PipmkinDeathSource;

    void Start()
    {
        IsHit = false;
        PimpkinBody = GetComponent<RectTransform>();
        randomIntX = Random.Range(-1, 1) + 0.5f; 
        randomIntY = Random.Range(0, 1) + 0.5f;
    }

    void Update()
    {
        Switch++;
        if (Switch % 120 == 0)
        {
            randomIntX = Random.Range(-2, 2);
            randomIntY = Random.Range(0, 2);
        }
        PimpkinBody.transform.position += new Vector3(randomIntX, randomIntY, 0);
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
