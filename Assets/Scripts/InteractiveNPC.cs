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
            // TODO: Maybe change the hardcoded value to something else?
            var yOffset = transform.position.y + (GetComponent<Collider>().bounds.size.y) + 1.0f;
            Vector3 offsetPos = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);
            Vector3 relativeScreenPosition = Camera.main.WorldToScreenPoint(offsetPos);
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
