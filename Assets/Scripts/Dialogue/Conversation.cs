using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Line
{
    [TagSelector]
    public string Character;

    [TextArea(2,5)]
    public string text;
}

[CreateAssetMenu(fileName="New Conversation", menuName = "Conversation")]
public class Conversation : ScriptableObject
{
    public Line[] Lines;
}
