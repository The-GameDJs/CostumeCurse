using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private CheckpointData CheckpointData;
    
    private void Start()
    {
        DialogueManager.SaveCheckpoint += OnCampfireRest;
    }

    private void OnCampfireRest(Vector3 checkpoint)
    {
        CheckpointData.SetCurrentCheckpoint(checkpoint);
    }
}
