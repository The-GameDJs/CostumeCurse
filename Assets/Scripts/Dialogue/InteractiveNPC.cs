using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractiveNPC : MonoBehaviour
{
    private bool IsConversationActive;
    private bool IsPlayerInRange;
    private float DistanceBetweenPlayer;
    public float MinDistance = 3.5f;
    public float MinDistanceForCheck = 5f;
    private DialogueManager DialogueManager;
    public Conversation Conversation;
    private GameObject Sield;
    private readonly float TurnSmoothness = 5f;
    private Vector3 LookPosition;
    private Quaternion InitialRotation;
    [SerializeField] bool IsNPC;
    public bool IsInteractiveNPC => IsNPC;

    
    private Animator DialogueIndicatorAnim;
    private GameObject DialogueIndicatorUI;
    private const float IndicatorOffsetScale = 1.35f;

    [SerializeField] private AudioSource NPCSpeakSound;
    public AudioSource NPCSpeakSoundSource => NPCSpeakSound;

    void Start()
    {
        DialogueManager = GameObject.FindGameObjectWithTag("DialogueManager").GetComponent<DialogueManager>();
        
        if(Application.isEditor && SceneManager.GetActiveScene().name != "Epilogue_Scene")
            Conversation.ResetClaimedReward();
        
        Sield = GameObject.Find("Sield");
        IsConversationActive = false;
        IsPlayerInRange = false;
        DistanceBetweenPlayer = 0f;

        DialogueIndicatorUI = GameObject.FindGameObjectWithTag("DialogueIndicator");
        DialogueIndicatorAnim = DialogueIndicatorUI.GetComponent<Animator>();

        if (!NPCSpeakSound)
            NPCSpeakSound = GetComponentInChildren<AudioSource>();

        InitialRotation = transform.rotation;
    }

    private void OnEnable()
    {
        InputManager.ActionCommandPressed += OnDialogueTriggered;
    }

    private void OnDisable()
    {
        InputManager.ActionCommandPressed -= OnDialogueTriggered;
    }
    
    private void OnDialogueTriggered(bool hasPressedActionCommand)
    {
        if (Conversation.Lines[0].Character == "The Witch" && DialogueManager.HasSpokenToWitch)
            return;
        
        if (IsConversationActive && hasPressedActionCommand)
        {
            IsConversationActive = DialogueManager.AdvanceConversation();
        }
        else if (hasPressedActionCommand && IsPlayerInRange)
        {
            IsConversationActive = true;

            if (IsNPC)
                LookPosition = Sield.transform.position - gameObject.transform.position;
     
            DialogueManager.StartDialogue(Conversation);
        }
    }

    void Update()
    {
        // If the NPC is the witch and they are currently summoning, don't update anymore
        if (IsNPC && gameObject.TryGetComponent<Witch>(out var witch) && witch.IsSummoning()) return;
        
        CheckIfInRange();
        
        if (IsNPC && IsConversationActive)
            RotateNPC(LookPosition);
        
        if (IsNPC && !IsConversationActive && Mathf.Abs(transform.rotation.eulerAngles.y - InitialRotation.eulerAngles.y) >= 0.1f)
        {
            var rotation = Quaternion.Lerp(gameObject.transform.rotation, InitialRotation, Time.deltaTime * TurnSmoothness).eulerAngles;
            gameObject.transform.rotation = Quaternion.Euler(0, rotation.y, 0);
        }
    }

    private void RotateNPC(Vector3 lookPosition)
    {
        var npcRotation = Quaternion.LookRotation(lookPosition, Vector3.up);
        var rotation = Quaternion.Lerp(gameObject.transform.rotation, npcRotation, Time.deltaTime * TurnSmoothness).eulerAngles;
        gameObject.transform.rotation = Quaternion.Euler(0, rotation.y, 0);
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

    public void ResetRotation()
    {
        
    }

    public void ActivateWitchSummoning()
    {
        gameObject.GetComponent<Witch>().ActivateWitchSummoning();
    }
}
