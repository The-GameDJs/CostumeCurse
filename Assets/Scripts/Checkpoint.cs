using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private SaveSystem _saveSystem;
    
    private void Start()
    {
        DialogueManager.SaveCheckpoint += OnCampfireRest;
        _saveSystem = FindObjectOfType<SaveSystem>();
    }

    private void OnCampfireRest(Vector3 checkpoint, int candyCornCount)
    {
        SaveSystem.SaveData data = new SaveSystem.SaveData(checkpoint, candyCornCount);
        _saveSystem.Save(data);
    }
}
