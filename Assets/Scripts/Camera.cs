using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public GameObject player;
    public float height;
    public float offsetX;
    public float offsetZ;

    void Update()
    {
        this.transform.position = player.transform.position + new Vector3(offsetX, height, offsetZ);
    }
}
