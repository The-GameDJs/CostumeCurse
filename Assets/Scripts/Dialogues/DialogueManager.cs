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
    
    private bool DisplayDialogueBubble;

    // Start is called before the first frame update
    void Start()
    {
        ActiveLineIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(DisplayDialogueBubble)
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
            DialogueUI.Close();
            ActiveLineIndex = 0;
            DisplayDialogueBubble = false;
            return false;
        }
    }

    void DisplayLine()
    {
        ActiveLine = Conversation.Lines[ActiveLineIndex];
        CurrentSpeaker = GameObject.FindWithTag(ActiveLine.Character);

        DialogueUI.Display(ActiveLine.text);
    }

    // Displays the bubble on top of the NPC
    void UpdateDialogueBubblePosition() 
    {
        var yOffset = CurrentSpeaker.GetComponent<Collider>().bounds.size.y * 1.45f;
        Vector3 offsetPos = new Vector3(CurrentSpeaker.transform.position.x, CurrentSpeaker.transform.position.y + yOffset, CurrentSpeaker.transform.position.z);
        Vector3 relativeScreenPosition = Camera.main.WorldToScreenPoint(offsetPos);
        DialogueUI.transform.position = relativeScreenPosition;
    }
}
