using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveNPC : MonoBehaviour
{
    public string DialogueText;
    public DialogueBubble Dialogue;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Dialogue.Show(DialogueText);
            Debug.Log("Player activated dialogue");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Dialogue.Close();
            Debug.Log("Player deactivated dialogue");
        }
    }
}
