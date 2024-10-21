using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public struct SaveData
    {
        public Vector3 RestPosition;
        public float CandyCornCount;

        public SaveData(Vector3 restPosition, float candyCornCount)
        {
            RestPosition = restPosition;
            CandyCornCount = candyCornCount;
        }
    }

    public static void Save(SaveData data)
    {
        Debug.Log($"Saving data!");
        PlayerPrefs.SetFloat("Candy Corn", data.CandyCornCount);
        PlayerPrefs.SetFloat("Rest.x", data.RestPosition.x);
        PlayerPrefs.SetFloat("Rest.y", data.RestPosition.y);
        PlayerPrefs.SetFloat("Rest.z", data.RestPosition.z);
    }

    public static float LoadSave(string dataName)
    {
        Debug.Log($"Loading save data! Data: {dataName}");
        return PlayerPrefs.GetFloat(dataName);
    }
}
