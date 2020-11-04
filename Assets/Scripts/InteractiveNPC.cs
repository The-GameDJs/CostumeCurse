using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveNPC : MonoBehaviour
{
    public string DialogueText;
    public DialogueBubble DialogueUI;
    private bool displayDialogueBubble;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (displayDialogueBubble)
        {
            Vector3 relativeScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
            DialogueUI.transform.position = relativeScreenPosition;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            displayDialogueBubble = true;
            DialogueUI.Show(DialogueText);
            Debug.Log("Player activated dialogue");
            Debug.Log(transform.position);
            Debug.Log(DialogueUI.gameObject.transform.position);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            displayDialogueBubble = false;
            DialogueUI.Close();
            Debug.Log("Player deactivated dialogue");
        }
    }
}
