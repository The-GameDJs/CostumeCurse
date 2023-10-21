using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EpilogueInteraction : MonoBehaviour
{
    private DialogueManager DialogueManager;
    public Conversation Conversation;
    public float PlayerMovementSpeed;
    public GameObject Sield;
    public GameObject Ganiel;
    private Animator DialogueIndicatorAnim;
    private GameObject DialogueIndicatorUI;
    
    private bool IsConversationActive;
    private bool hasConversationStarted;
    private bool ArePlayersMoving;
    private bool AreCreditsRolling;
    public RectTransform CreditsPanelRectTransform;
    public float ScrollSpeed;

    public Animator CrossfadeAnimator;

    void Start()
    {
        DialogueManager = GameObject.FindGameObjectWithTag("DialogueManager").GetComponent<DialogueManager>();
        Sield = GameObject.Find("Sield");
        IsConversationActive = false;

        DialogueIndicatorUI = GameObject.FindGameObjectWithTag("DialogueIndicator");
        DialogueIndicatorAnim = DialogueIndicatorUI.GetComponent<Animator>();

        //NPCSpeakSound = GetComponentInChildren<AudioSource>();
        StartCoroutine(ActivateConversation());
    }

    private IEnumerator ActivateConversation()
    {
        yield return new WaitForSeconds(5.0f);
        IsConversationActive = true;
        DialogueManager.StartDialogue(Conversation);
    }

    private void Update()
    {
        if (IsConversationActive && Input.GetButtonDown("Action Command"))
        {
            IsConversationActive = DialogueManager.AdvanceConversation();
        }

        if (ArePlayersMoving)
        {
            Sield.transform.Translate(0.0f, 0.0f, Time.deltaTime * PlayerMovementSpeed);
            Ganiel.transform.Translate(0.0f, 0.0f, Time.deltaTime * PlayerMovementSpeed);
        }

        if (AreCreditsRolling)
        {
            CreditsPanelRectTransform.position += new Vector3(0.0f, Time.deltaTime * ScrollSpeed, 0.0f);
        }
    }

    public IEnumerator MovePlayersAWayFromScreen()
    {
        ArePlayersMoving = true;
        Sield.transform.rotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
        Ganiel.transform.rotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
        AreCreditsRolling = true;
        
        yield return new WaitForSeconds(5.0f);

        ArePlayersMoving = false;
        
        yield return new WaitForSeconds(83f);
        
        AreCreditsRolling = false;
        
        CrossfadeAnimator.Play("CrossfadeAnimationStart");

        yield return new WaitForSeconds(3.0f);

        SceneManager.LoadScene("Title_Screen");
    }
}
