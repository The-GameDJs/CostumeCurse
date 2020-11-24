using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveNPC : MonoBehaviour
{
    private bool IsDisplayingDialogue;
    private bool IsPlayerInRange;
    private float DistanceBetweenPlayer;
    public float MinDistance = 3.5f;
    public DialogueManager DialogueManager;
    public Conversation Conversation;

    void Start()
    {
        IsDisplayingDialogue = false;
        IsPlayerInRange = false;
        DistanceBetweenPlayer = 0f;
    }

    void Update()
    {
        CheckIfInRange();

        if(IsDisplayingDialogue & Input.GetButtonDown("Action Command"))
        {
            IsDisplayingDialogue = DialogueManager.AdvanceConversation();
        }
        else if (Input.GetButtonDown("Action Command") && IsPlayerInRange)
        {
            IsDisplayingDialogue = !IsDisplayingDialogue;
            DialogueManager.StartDialogue(Conversation);
        }
    }

    void CheckIfInRange()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player)
        {
            DistanceBetweenPlayer = Vector3.Distance(player.transform.position, transform.position);
        }

        if (DistanceBetweenPlayer <= MinDistance)
        {
            IsPlayerInRange = true;
        }
        else
        {
            IsPlayerInRange = false;
            if(IsDisplayingDialogue)
            {
                DialogueManager.CloseDialogue();
            }
        }
    }
}
