using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public DialogueBubble DialogueUI;
    private Conversation Conversation;

    private Line ActiveLine;
    private int ActiveLineIndex;
    private GameObject CurrentSpeaker;
    private const float YOffsetScaler = 1.45f;

    private bool DisplayDialogueBubble;

    void Start()
    {
        ActiveLineIndex = 0;
        DisplayDialogueBubble = false;
    }

    void Update()
    {
        if (DisplayDialogueBubble)
        {
            UpdateDialogueBubblePosition();
        }
    }

    // Start dialogue
    public void StartDialogue(Conversation conversation)
    {
        DisplayDialogueBubble = true;
        Conversation = conversation;
        AdvanceConversation();
    }

    public bool AdvanceConversation()
    {
        if (ActiveLineIndex < Conversation.Lines.Length)
        {
            DisplayLine();
            ActiveLineIndex++;
            return true;
        }
        else
        {
            CloseDialogue();
            return false;
        }
    }

    void DisplayLine()
    {
        ActiveLine = Conversation.Lines[ActiveLineIndex];
        CurrentSpeaker = GameObject.FindWithTag(ActiveLine.Character);

        DialogueUI.Display(ActiveLine.text);
    }

    public void CloseDialogue()
    {
        DisplayDialogueBubble = false;
        ActiveLineIndex = 0;
        DialogueUI.Close();
    }

    // Displays the bubble on top of the NPC
    void UpdateDialogueBubblePosition()
    {
        var yOffset = CurrentSpeaker.GetComponent<Collider>().bounds.size.y * YOffsetScaler;
        Vector3 offsetPos = new Vector3(CurrentSpeaker.transform.position.x, CurrentSpeaker.transform.position.y + yOffset, CurrentSpeaker.transform.position.z);
        Vector3 relativeScreenPosition = Camera.main.WorldToScreenPoint(offsetPos);
        DialogueUI.transform.position = relativeScreenPosition;
    }
}
