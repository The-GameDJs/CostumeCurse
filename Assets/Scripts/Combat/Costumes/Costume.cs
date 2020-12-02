using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Costume : MonoBehaviour
{
    [SerializeField] private GameObject AbilitiesUIPanel;
    private bool IsDisplayingAbilities;

    // TODO: Stop displaying abilities UI after a choice

    private void Update()
    {
        if (IsDisplayingAbilities)
        {
            Vector3 relativeScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
            AbilitiesUIPanel.transform.position = relativeScreenPosition;
        }
    }

    public void DisplayAbilities(bool displayAbilities)
    {
        IsDisplayingAbilities = displayAbilities;
        AbilitiesUIPanel.SetActive(displayAbilities);
    }
}
