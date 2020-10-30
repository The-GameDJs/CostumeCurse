using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Costume : MonoBehaviour
{
    [SerializeField] private GameObject abilitiesUI;

    // TODO: Stop displaying abilities UI after a choice

    public void DisplayAbilities(bool boolean)
    {
        abilitiesUI.SetActive(boolean);
    }
}
