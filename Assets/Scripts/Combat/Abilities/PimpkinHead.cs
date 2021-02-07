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
        gameObject.SetActive(true);
        randomIntX = Random.Range(-1, 1) + 0.5f; 
        randomIntY = Random.Range(0, 1) + 0.5f;
    }

    void Update()
    {
        MovePimpkinHead();
    }

    private void MovePimpkinHead()
    {
        Switch++;

        if (Switch % 120 == 0)
        {
            randomIntX = Random.Range(-2, 2);
            randomIntY = Random.Range(0, 2);
        }

        PimpkinBody.transform.position += new Vector3(randomIntX, randomIntY, 0);
    }

    public void DestroyPimpkin()
    {
        Debug.Log("Hit pimpkin!!!!!");
        PipmkinDeathSource.Play();
        gameObject.SetActive(false);
        IsHit = true;
    }

    public void ResetPimpkinValues()
    {
        IsHit = false;
        Switch = 0;
    }

    public bool GetHit()
    {
        return IsHit;
    }
}
