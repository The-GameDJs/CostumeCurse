using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPosition : MonoBehaviour
{
    public string LocationName;
    public Vector3 Coordinates;

    private void Awake()
    {
        Coordinates = transform.position;
    }
}
