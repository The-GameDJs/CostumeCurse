using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveNPC : MonoBehaviour
{
    private bool DisplayDialogueBubble;
    private bool PlayerInRange;
    private float DistanceBetweenPlayer;
    public float MinDistance = 3.5f;
    public DialogueManager DialogueManager;
    public Conversation Conversation;

    void Start()
    {
        DisplayDialogueBubble = false;
        PlayerInRange = false;
        DistanceBetweenPlayer = 0f;
    }

    void Update()
    {
        CheckIfInRange();

        if(DisplayDialogueBubble & Input.GetButtonDown("Action Command"))
        {
            DisplayDialogueBubble = DialogueManager.AdvanceConversation();
        }

        else if (Input.GetButtonDown("Action Command") && PlayerInRange)
        {
            DisplayDialogueBubble = !DisplayDialogueBubble;
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
            PlayerInRange = true;
        }
        else
        {
            PlayerInRange = false;
        }
    }
}
