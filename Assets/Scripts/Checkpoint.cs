using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void Start()
    {
        DialogueManager.SaveCheckpoint += OnCampfireRest;
    }

    private void OnCampfireRest(Vector3 checkpoint, int candyCornCount)
    {
        SaveSystem.SaveData data = new SaveSystem.SaveData(checkpoint, candyCornCount);
        SaveSystem.Save(data);
    }
}
