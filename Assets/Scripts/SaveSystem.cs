using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public struct SaveData
    {
        public Vector3 RestPosition;
        public int CandyCornCount;
        public int SieldAbilityIndex;
        public int GanielAbilityIndex;

        public SaveData(Vector3 restPosition, int candyCornCount, int sieldAbilityIndex, int ganielAbilityIndex)
        {
            RestPosition = restPosition;
            CandyCornCount = candyCornCount;
            SieldAbilityIndex = sieldAbilityIndex;
            GanielAbilityIndex = ganielAbilityIndex;
        }
    }

    public static void Save(SaveData data)
    {
        Debug.Log($"Saving data!");
        PlayerPrefs.SetInt("Candy Corn", data.CandyCornCount);
        PlayerPrefs.SetFloat("Rest.x", data.RestPosition.x);
        PlayerPrefs.SetFloat("Rest.y", data.RestPosition.y);
        PlayerPrefs.SetFloat("Rest.z", data.RestPosition.z);
        PlayerPrefs.SetInt("Sield Ability Index", data.SieldAbilityIndex);
        PlayerPrefs.SetInt("Ganiel Ability Index", data.GanielAbilityIndex);
    }

    public static SaveData Load()
    {
        Debug.Log($"Loading save data!");
        var candyCount = PlayerPrefs.GetInt("Candy Corn");
        var restPoint = new Vector3(PlayerPrefs.GetFloat("Rest.x"),
            PlayerPrefs.GetFloat("Rest.y"),
            PlayerPrefs.GetFloat("Rest.z"));
        
        var sieldAbilityIndex = PlayerPrefs.GetInt("Sield Ability Index");
        var ganielAbilityIndex = PlayerPrefs.GetInt("Ganiel Ability Index");

        var saveData = new SaveData(restPoint, candyCount, sieldAbilityIndex, ganielAbilityIndex);
        return saveData;
    }
}
