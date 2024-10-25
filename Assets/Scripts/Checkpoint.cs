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
        var sieldCostume = GameObject.Find("Sield").GetComponent<Costume>();
        var ganielCostume = GameObject.Find("Ganiel").GetComponent<Costume>();
        SaveSystem.SaveData data = new SaveSystem.SaveData(checkpoint, candyCornCount, sieldCostume.GetAbilityIndex(), ganielCostume.GetAbilityIndex());
        SaveSystem.Save(data);
    }

    private void OnDestroy()
    {
        DialogueManager.SaveCheckpoint -= OnCampfireRest;
    }
}
