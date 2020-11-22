using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public DialogueBubble DialogueUI;
    public Conversation Conversation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // Start dialogue
    void StartDialogue() 
    {
        // Display line will be handled by DialogueBubble
        
        //DialogueUI.StartDialogue(DialogueText);

        //DialogueUI.DisplayNextLine();
    }

    
    void UpdateDialogueBubblePosition() 
    {
        // Get current speaker and make sure dialogue bubble is updated
        // according to that speaker's position
    }
}
