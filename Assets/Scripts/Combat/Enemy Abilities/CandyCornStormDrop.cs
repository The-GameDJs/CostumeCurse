using System;
using System.Collections;
using System.Collections.Generic;
using Combat.Enemy_Abilities;
using UnityEngine;

public class CandyCornStormDrop : MonoBehaviour
{
    private CandyStorm AbilityReference;
    private RectTransform SieldHatTransform;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private float InitialDroppingVelocity;
    [SerializeField] private float DestinationHeightDrop;
    private Vector3 InitialHeightDropPosition;
    private bool isCaught;
    
    
    // Start is called before the first frame update
    void Start()
    {
        InitialHeightDropPosition = transform.position;
        Destroy(gameObject, 2.0f);
    }

    public void SetCandyStormComponents(CandyStorm candyStorm, RectTransform sieldHat)
    {
        AbilityReference = candyStorm;
        SieldHatTransform = sieldHat;
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.Translate(Vector3.down * (Time.deltaTime * InitialDroppingVelocity));

        // If the distance between the hat and the drop is less than 57, it means it is ready to be collected
        if (Mathf.Abs(Vector3.Distance(SieldHatTransform.position, rectTransform.position)) <= 100.0f && !isCaught)
        {
            // Avoiding multiple calls, even if object is about to be destroyed
            isCaught = true;
            AbilityReference.CatchCandy();
            Destroy(gameObject);
        }
    }
}
