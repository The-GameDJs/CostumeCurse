using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveNPC : MonoBehaviour
{
    private bool IsConversationActive;
    private bool IsPlayerInRange;
    private float DistanceBetweenPlayer;
    public float MinDistance = 3.5f;
    public float MinDistanceForCheck = 5f;
    private DialogueManager DialogueManager;
    public Conversation Conversation;
    //private GameObject Player;
    private GameObject Sield;

    
    private Animator DialogueIndicatorAnim;
    private GameObject DialogueIndicatorUI;
    private const float IndicatorOffsetScale = 1.35f;

    private AudioSource NPCSpeakSound;

    void Start()
    {
        DialogueManager = GameObject.FindGameObjectWithTag("DialogueManager").GetComponent<DialogueManager>();
        //Player = GameObject.FindWithTag("Player");
        Sield = GameObject.Find("Sield");
        IsConversationActive = false;
        IsPlayerInRange = false;
        DistanceBetweenPlayer = 0f;

        DialogueIndicatorUI = GameObject.FindGameObjectWithTag("DialogueIndicator");
        DialogueIndicatorAnim = DialogueIndicatorUI.GetComponent<Animator>();

        NPCSpeakSound = GetComponentInChildren<AudioSource>();
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
        DistanceBetweenPlayer = Vector3.Distance(Sield.transform.position, transform.position);

        // Prevents other InteractableNPC objects from checking if not in a
        // minimum check distance
        if (DistanceBetweenPlayer <= MinDistanceForCheck)
        {
            if (DistanceBetweenPlayer <= MinDistance)
            {
                IsPlayerInRange = true;
                DialogueIndicatorAnim.SetBool("InRange", true);
                UpdateDialogueIndicatorPosition();
                if (IsConversationActive)
                {
                    DialogueIndicatorAnim.SetBool("InRange", false);
                }
            }
            else 
            {
                IsPlayerInRange = false;
                DialogueIndicatorAnim.SetBool("InRange", false);
                if (IsConversationActive)
                {
                    DialogueManager.CloseDialogue();
                    IsConversationActive = false;
                }
            }
        }
    }

    void UpdateDialogueIndicatorPosition()
    {
        var yOffset = Sield.GetComponent<Collider>().bounds.size.y * IndicatorOffsetScale;
        var xOffset = Sield.GetComponent<Collider>().bounds.size.x * IndicatorOffsetScale;
        Vector3 offsetPos = new Vector3(Sield.transform.position.x + xOffset, Sield.transform.position.y + yOffset, Sield.transform.position.z);
        Vector3 relativeScreenPosition = Camera.main.WorldToScreenPoint(offsetPos);
        DialogueIndicatorUI.transform.position = relativeScreenPosition;
    }
}
