using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatDebugVisualization : MonoBehaviour
{
    private MeshRenderer MeshRenderer;

    public void Start()
    {
        MeshRenderer = GetComponent<MeshRenderer>();
        MeshRenderer.enabled = false;
    }

}
