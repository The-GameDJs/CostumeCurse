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
    private const float YOffsetScale = 1.45f;

    private bool DisplayDialogueBubble;

    private CandyCornManager CandyCornManager;

    void Start()
    {
        ActiveLineIndex = 0;
        DisplayDialogueBubble = false;
        CandyCornManager = GameObject.FindObjectOfType<CandyCornManager>();
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
        CurrentSpeaker = GameObject.Find(ActiveLine.Character);

        CurrentSpeaker.transform.Find("NPCSpeakSound").GetComponent<AudioSource>().Play();

        DialogueUI.Display(ActiveLine.text);
    }

    public void CloseDialogue()
    {
        DisplayDialogueBubble = false;
        ActiveLineIndex = 0;
        DialogueUI.Close();

        if (!Conversation.IsCandyCornRewardClaimed())
            CandyCornManager.AddCandyCorn(Conversation.ClaimReward());
    }

    // Displays the bubble on top of the NPC
    void UpdateDialogueBubblePosition()
    {
        var yOffset = CurrentSpeaker.GetComponent<Collider>().bounds.size.y * YOffsetScale;
        Vector3 offsetPos = new Vector3(CurrentSpeaker.transform.position.x, CurrentSpeaker.transform.position.y + yOffset, CurrentSpeaker.transform.position.z);
        Vector3 relativeScreenPosition = Camera.main.WorldToScreenPoint(offsetPos);
        DialogueUI.transform.position = relativeScreenPosition;
    }
}
