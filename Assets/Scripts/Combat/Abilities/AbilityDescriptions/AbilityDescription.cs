using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability Description", menuName = "Ability Description")]
public class AbilityDescription : ScriptableObject
{
    public string Name;
    [TextArea(2, 5)]
    public string Description;
}
