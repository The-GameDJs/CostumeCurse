using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
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
    public bool HasSpokenToWitch;

    private CandyCornManager CandyCornManager;

    public static Action<Vector3, int> SaveCheckpoint;

    public static Action<string> GrantAbility;

    void Start()
    {
        ActiveLineIndex = 0;
        DisplayDialogueBubble = false;
        CandyCornManager = GameObject.FindObjectOfType<CandyCornManager>();

        if (SceneManager.GetActiveScene().name == "Epilogue_Scene")
        {
            CinemachineCameraRig.Instance.ChangeCinemachineBrainBlendTime(5.0f);
            CinemachineCameraRig.Instance.SetCinemachineCamera(GameObject.Find("CMTarget (2)").GetComponentInChildren<CinemachineVirtualCamera>());
        }
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
            HasSpokenToWitch = true;
            CurrentSpeaker.GetComponent<InteractiveNPC>().ActivateWitchSummoning();
            LoadNextGameLevel();
        }

        var monkName = Conversation.Lines.Where(x => x.Character == "Mysterious Monk").Select(x => x.Character).FirstOrDefault();
        
        if (monkName != null && Conversation.ShouldGrantAbility)
        {
            var count = Conversation.Lines.Length;
            GrantAbility?.Invoke(Conversation.Lines[count - 1].Character);
        }

        if (Conversation.IsRestPoint && ActiveLineIndex == Conversation.Lines.Length)
            SaveCheckpoint?.Invoke(CurrentSpeaker.transform.position, CandyCornManager.GetTotalCandyCorn());

        if (!Conversation.IsRestPoint && Conversation.CandyCornReward > 0 && ActiveLineIndex == Conversation.Lines.Length)
            CandyCornManager.AddCandyCorn(Conversation.ClaimReward());

        if (Conversation.IsRestPoint &&
            CandyCornManager.GetTotalCandyCorn() < CandyCornManager.GetMaxCandyCorn() / 2 &&
            ActiveLineIndex == Conversation.Lines.Length)
            CandyCornManager.AddCandyCorn(Conversation.ClaimReward());

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
