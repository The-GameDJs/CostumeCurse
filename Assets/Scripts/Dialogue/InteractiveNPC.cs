using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveNPC : MonoBehaviour
{
    private bool IsConversationActive;
    private bool IsPlayerInRange;
    private float DistanceBetweenPlayer;
    public float MinDistance = 3.5f;
    private DialogueManager DialogueManager;
    public Conversation Conversation;
    private GameObject Player;


    void Start()
    {
        DialogueManager = GameObject.FindGameObjectWithTag("DialogueManager").GetComponent<DialogueManager>();
        Player = GameObject.FindWithTag("Player");
        IsConversationActive = false;
        IsPlayerInRange = false;
        DistanceBetweenPlayer = 0f;
    }

    void Update()
    {
        CheckIfInRange();

        if (IsConversationActive && Input.GetButtonDown("Action Command"))
        {
            IsConversationActive = DialogueManager.AdvanceConversation();
        }
        else if (Input.GetButtonDown("Action Command") && IsPlayerInRange)
        {
            IsConversationActive = true;
            DialogueManager.StartDialogue(Conversation);
        }
    }

    void CheckIfInRange()
    {
        if (Player)
        {
            DistanceBetweenPlayer = Vector3.Distance(Player.transform.position, transform.position);
        }

        if (DistanceBetweenPlayer <= MinDistance)
        {
            IsPlayerInRange = true;
        }
        else
        {
            IsPlayerInRange = false;
            if (IsConversationActive)
            {
                DialogueManager.CloseDialogue();
                IsConversationActive = false;
            }
        }
    }
}
