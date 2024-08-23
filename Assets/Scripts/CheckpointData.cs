using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Checkpoint Data")]
public class CheckpointData : ScriptableObject
{
    [SerializeField] private Vector3 CurrentCheckpoint;

    public void SetCurrentCheckpoint(Vector3 position)
    {
        CurrentCheckpoint = position;
    }

    public Vector3 GetCurrentCheckpoint()
    {
        return CurrentCheckpoint;
    }
}
