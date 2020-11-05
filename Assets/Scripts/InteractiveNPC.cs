using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveNPC : MonoBehaviour
{
    public string DialogueText;
    public DialogueBubble DialogueUI;
    private bool DisplayDialogueBubble;
    private bool PlayerInRange;
    private float DistanceBetweenPlayer;
    public float MinDistance = 3.5f;

    void Start()
    {
        DisplayDialogueBubble = false;
        PlayerInRange = false;
        DistanceBetweenPlayer = 0f;
    }

    void Update()
    {
        CheckIfInRange();

        if (Input.GetButtonDown("Action Command") && PlayerInRange)
        {
            DisplayDialogueBubble = !DisplayDialogueBubble;
            DisplayTextBubble(DisplayDialogueBubble);
        }

        if (DisplayDialogueBubble)
        {
            UpdateDialogueBubblePosition();
        }
    }

    void DisplayTextBubble(bool isActive)
    {
        if (isActive)
        {
            DialogueUI.Show(DialogueText);
        }
        else
        {
            DialogueUI.Close();
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

    // Displays the bubble on top of the NPC
    void UpdateDialogueBubblePosition()
    {

        // TODO: Maybe change the hardcoded value to something else?
        var yOffset = transform.position.y + (GetComponent<Collider>().bounds.size.y) + 1.0f;
        Vector3 offsetPos = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);
        Vector3 relativeScreenPosition = Camera.main.WorldToScreenPoint(offsetPos);
        DialogueUI.transform.position = relativeScreenPosition;

        if (!PlayerInRange)
        {
            DialogueUI.Close();
            DisplayDialogueBubble = false;
        }
    }
}
