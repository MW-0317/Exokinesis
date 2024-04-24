using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DialogueTrigger : MonoBehaviour
{
    public GameObject promptUI;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] public int AmountToGetCorrect = -1;
    [SerializeField] public List<dialogueString> dialogueStrings = new List<dialogueString>();
    [SerializeField] private Transform NPCTransform;
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private Outline npcOutline;
    [SerializeField] private LayerMask layerMask;
    
    private Transform playerTransform;
    private bool hasSpoken = false;
    private bool ispromptVisible;

    public bool HasSpoken { get { return hasSpoken; } }

    private void Awake()
    {
        if (npcOutline == null)
            npcOutline = GetComponent<Outline>();
    }

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        bool promptVisible = PromptVisible();
        
        Vector3 directionToNPC = NPCTransform.position - Camera.main.transform.position;
        float angleToNPC = Vector3.Angle(Camera.main.transform.forward, directionToNPC);

        bool withinFieldOfView = angleToNPC <= 45;

        bool playerIsClose = Vector3.Distance(playerTransform.position, NPCTransform.position) <= interactionDistance;
        
        if (NPCTransform.gameObject.name == "Warden")
        {
            //Debug.Log("Angle to NPC: " + angleToNPC);
            //Debug.Log("Player is close: " + playerIsClose + " Within field of view: " + withinFieldOfView + " Has (not) spoken: " + !hasSpoken);
        }

        if (playerIsClose && withinFieldOfView && !hasSpoken && !playerTransform.GetComponent<DialogueController>().IsDialogueActive)
        {
            RaycastHit hit;
            
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactionDistance, layerMask))
            {
                if (hit.transform == NPCTransform)
                {
                    if (OutlineManager.Instance.RequestOutline(npcOutline)) npcOutline.enabled = true;

                    if (playerInput.actions["Interact"].ReadValue<float>() > 0)
                    {
                        StartDialogue();
                    }
                }
                else
                {
                    ClearNPCOutline();
                }
            }
            else
            {
                ClearNPCOutline();
            }
        }
        else
        {
            ClearNPCOutline();
        }

        if (promptVisible != ispromptVisible)
        {
            UpdatePrompt(promptVisible);
            ispromptVisible = promptVisible;
        }
    }

    private void ClearNPCOutline()
    {
        if (npcOutline.enabled)
        {
            OutlineManager.Instance.ClearOutline(npcOutline);
            npcOutline.enabled = false;
        }
    }

    public void StartDialogue()
    {
        ClearNPCOutline();
        
        playerTransform.GetComponent<DialogueController>().DialogueStart(dialogueStrings, NPCTransform);
        hasSpoken = true;
    }

    private bool PromptVisible()
    {
        return npcOutline != null && npcOutline.enabled;
    }
    
    private void UpdatePrompt(bool visible)
    {
        if (visible)
        {
            promptUI.gameObject.SetActive(true);
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(npcOutline.transform.position);
            promptUI.transform.position = screenPosition + new Vector3(90, 150, 0); // Adjust as needed
        }
        else
        {
            promptUI.gameObject.SetActive(false);
        }
    }
}

[System.Serializable]
public class dialogueString
{
    public string text;
    public bool isEnd;

    [Header("Branch")]
    public bool isQuestion;
    public string answerOption1;
    public string answerOption2;
    public int option1SuccessIndexJump;
    public int option1FailureIndexJump;
    public int option2SuccessIndexJump;
    public int option2FailureIndexJump;
    [Tooltip("1 or 2")]
    public int correctChoice;
    
    [Header("Level Requirements")]
    public int option1Level;
    public int option2Level;

    [Header("Triggered Events")]
    public UnityEvent startDialogueEvent;
    public UnityEvent endDialogueEvent;

    public bool correctChoicePicked = false;
}