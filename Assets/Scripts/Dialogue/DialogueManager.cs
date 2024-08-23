using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private Animator Animator;
    public DialogueBubble DialogueUI;
    private Conversation Conversation;

    private Line ActiveLine;
    private int ActiveLineIndex;
    private GameObject CurrentSpeaker;
    private const float YOffsetScale = 1.45f;

    private bool DisplayDialogueBubble;

    private CandyCornManager CandyCornManager;

    public static Action<Vector3, int> SaveCheckpoint;

    void Start()
    {
        ActiveLineIndex = 0;
        DisplayDialogueBubble = false;
        CandyCornManager = GameObject.FindObjectOfType<CandyCornManager>();
    }

    void Update()
    {
        if (DisplayDialogueBubble)
        {
            UpdateDialogueBubblePosition();
        }
    }

    // Start dialogue
    public void StartDialogue(Conversation conversation)
    {
        DisplayDialogueBubble = true;
        Conversation = conversation;
        AdvanceConversation();
    }

    public bool AdvanceConversation()
    {
        if (ActiveLineIndex < Conversation.Lines.Length)
        {
            DisplayLine();
            ActiveLineIndex++;
            return true;
        }
        else
        {
            CloseDialogue();
            return false;
        }
    }

    void DisplayLine()
    {
        ActiveLine = Conversation.Lines[ActiveLineIndex];
        CurrentSpeaker = GameObject.Find(ActiveLine.Character);
        CurrentSpeaker.transform.Find("NPCSpeakSound").GetComponent<AudioSource>().Play();
        
        bool isHumanoid = true;
        if (CurrentSpeaker.TryGetComponent<InteractiveNPC>(out var interactiveNPC))
        {
            interactiveNPC.NPCSpeakSoundSource.Play();
        }
        DialogueUI.Display(ActiveLine.text, Conversation.ShouldShowCharacterName ? ActiveLine.Character : String.Empty, isHumanoid);
    }

    public void CloseDialogue()
    {
        Debug.Log(CurrentSpeaker.gameObject.name);
        if (CurrentSpeaker.gameObject.name == "The Witch" && ActiveLineIndex == Conversation.Lines.Length)
        {
            CurrentSpeaker.GetComponent<InteractiveNPC>().ActivateWitchSummoning();
            LoadNextGameLevel();
        }

        if (Conversation.IsRestPoint && ActiveLineIndex == Conversation.Lines.Length)
        {
            CandyCornManager.AddCandyCorn(Conversation.ClaimReward());
            SaveCheckpoint?.Invoke(CurrentSpeaker.transform.position, CandyCornManager.GetTotalCandyCorn());
        }

        DisplayDialogueBubble = false;
        ActiveLineIndex = 0;
        DialogueUI.Close();

        if (FindObjectOfType<EpilogueInteraction>() != null)
        {
            var epilogue = FindObjectOfType<EpilogueInteraction>();
            StartCoroutine(epilogue.MovePlayersAWayFromScreen());
        }
    }

    // Displays the bubble on top of the NPC
    void UpdateDialogueBubblePosition()
    {
        var yOffset = CurrentSpeaker.GetComponent<Collider>().bounds.size.y * YOffsetScale;
        Vector3 offsetPos = new Vector3(CurrentSpeaker.transform.position.x, CurrentSpeaker.transform.position.y + yOffset, CurrentSpeaker.transform.position.z);
        Vector3 relativeScreenPosition = Camera.main.WorldToScreenPoint(offsetPos);
        DialogueUI.transform.position = relativeScreenPosition;
    }

    public void LoadNextGameLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        yield return new WaitForSeconds(1.0f);
        Animator.SetTrigger("Start");
        yield return new WaitForSeconds(2.0f);

        SceneManager.LoadScene(levelIndex);
    }
}
