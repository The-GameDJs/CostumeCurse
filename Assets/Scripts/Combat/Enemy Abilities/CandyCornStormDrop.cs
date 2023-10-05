using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyCornStormDrop : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private float InitialDroppingVelocity;
    [SerializeField] private float DestinationHeightDrop;
    private Vector3 InitialHeightDropPosition;
    
    
    // Start is called before the first frame update
    void Start()
    {
        InitialHeightDropPosition = transform.position;
        Destroy(gameObject, 2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.Translate(Vector3.down * (Time.deltaTime * InitialDroppingVelocity));
    }
}
