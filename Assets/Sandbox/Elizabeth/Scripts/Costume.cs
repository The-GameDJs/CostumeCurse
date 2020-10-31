using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Costume : MonoBehaviour
{
    [SerializeField] private GameObject abilitiesUI;
    private bool displayAbilities;

    // TODO: Stop displaying abilities UI after a choice

    private void Update()
    {
        if (displayAbilities)
        {
            Vector3 relativeScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
            abilitiesUI.transform.position = relativeScreenPosition;
        }
    }

    public void DisplayAbilities(bool displayAbilities)
    {
        this.displayAbilities = displayAbilities;
        abilitiesUI.SetActive(displayAbilities);
    }
}
