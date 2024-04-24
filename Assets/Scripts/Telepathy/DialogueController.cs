using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

// Code for moving the camera towards an NPC is not currently used

public class DialogueController : MonoBehaviour
{
    public IntroUI introUI;
    
    //[SerializeField] private GameObject playerFollowCamera;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject dialogueParent;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject firstSelected;
    [SerializeField] private Button option1Button;
    [SerializeField] private Button option2Button;
    
    [SerializeField] private Animator dialogueAnimator;

    [SerializeField] private float typingSpeed = 0.05f;
    //[SerializeField] private float turnSpeed = 2f;
    
    public bool IsDialogueActive { get; private set; } = false;

    private List<dialogueString> dialogueList;
    
    //private Quaternion initialCameraRotation;

    private int currentDialogueIndex = 0;
    private Coroutine printDialogueCoroutine = null;
    private Coroutine typeTextCoroutine = null;
    
    private void Start()
    {
        dialogueParent.SetActive(false);
        ResetDialogueUI();
    }

    public void DialogueStart(List<dialogueString> textToPrint, Transform NPC)
    {
        if(IsDialogueActive) return;
        IsDialogueActive = true;
        
        playerInput.SwitchCurrentActionMap("Telepathy");
        
        dialogueParent.SetActive(true);
        
        dialogueParent.transform.localScale = Vector3.one;
        dialogueText.transform.localScale = Vector3.one;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //initialCameraRotation = playerFollowCamera.transform.rotation;

        //StartCoroutine(TurnCameraTowardsNPC(NPC));
        
        // Reset the animator to its default state
        dialogueAnimator.Rebind();

        dialogueList = textToPrint;
        currentDialogueIndex = 0;

        DisableButtons();
        
        StartCoroutine(PlayAnimationAndWait("Dialogue Appearing", () =>
        {
            StartCoroutine(PrintDialogue());
        }));
    }
    
    private void DisableButtons()
    {
        StartCoroutine(PlayAnimationAndWait("Question Disappearing", () =>
        {
            EventSystem.current.SetSelectedGameObject(null);
            //option1Button.gameObject.SetActive(false);
            //option2Button.gameObject.SetActive(false);
        }));
    }

    //private IEnumerator TurnCameraTowardsNPC(Transform NPC)
    //{
        //Quaternion startRotation = playerFollowCamera.transform.rotation;
        //Quaternion targetRotation = Quaternion.LookRotation(NPC.position - playerFollowCamera.transform.position);

        //float elapsedTime = 0f;
        //while (elapsedTime < 1f)
        //{
            //playerFollowCamera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime);
            //elapsedTime += Time.deltaTime * turnSpeed;
            //yield return null;
        //}

        //playerFollowCamera.transform.rotation = targetRotation;
    //}

    private bool optionSelected = false;

    private IEnumerator PrintDialogue()
    {
        while (currentDialogueIndex < dialogueList.Count && IsDialogueActive)
        {
            dialogueString line = dialogueList[currentDialogueIndex];
            
            var telepathyLevel = 0;
            if (PlayerSkills.Instance.IsSkillUnlocked(PlayerSkills.SkillType.Telepathy2))
                telepathyLevel = 2;
            else if (PlayerSkills.Instance.IsSkillUnlocked(PlayerSkills.SkillType.Telepathy1))
                telepathyLevel = 1;
            
            line.startDialogueEvent?.Invoke();

            if (line.isQuestion)
            {
                EventSystem.current.SetSelectedGameObject(firstSelected);
                
                yield return StartCoroutine(TypeText(line.text));
                
                option1Button.gameObject.SetActive(true);
                option2Button.gameObject.SetActive(true);
                option1Button.interactable = true;
                option2Button.interactable = true;

                option1Button.GetComponentInChildren<TMP_Text>().text = line.answerOption1;
                option2Button.GetComponentInChildren<TMP_Text>().text = line.answerOption2;
                
                option1Button.onClick.RemoveAllListeners();
                option2Button.onClick.RemoveAllListeners();
                
                if (telepathyLevel >= line.option1Level)
                    option1Button.onClick.AddListener(() => HandleOptionSelected(line.option1SuccessIndexJump));
                else
                    option1Button.onClick.AddListener(() => HandleOptionSelected(line.option1FailureIndexJump));
                
                if (telepathyLevel >= line.option2Level)
                    option2Button.onClick.AddListener(() => HandleOptionSelected(line.option2SuccessIndexJump));
                else
                    option2Button.onClick.AddListener(() => HandleOptionSelected(line.option2FailureIndexJump));

                option1Button.onClick.AddListener(() =>
                {
                    line.correctChoicePicked = line.correctChoice == 1;
                });

                option2Button.onClick.AddListener(() =>
                {
                    line.correctChoicePicked = line.correctChoice == 2;
                });

                dialogueAnimator.SetTrigger("questionStarting");
                yield return new WaitForSeconds(dialogueAnimator.GetCurrentAnimatorStateInfo(0).length);

                yield return new WaitUntil(() => optionSelected);
                
            }
            else
            {
                yield return StartCoroutine(TypeText(line.text));
            }
            
            line.endDialogueEvent?.Invoke();

            optionSelected = false;

            if (line.isEnd || !IsDialogueActive)
            {
                DialogueStop();
                break;
            }
        }

        currentDialogueIndex++;
    }

    private void HandleOptionSelected(int indexJump)
    {
        optionSelected = true;
        DisableButtons();

        currentDialogueIndex = indexJump;
    }

    private IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        string displayedText = "";
        bool wasSkipped = false;

        if (IsDialogueActive)
        {
            foreach (char letter in text.ToCharArray())
            {
                if (!IsDialogueActive) yield break;
                
                displayedText += letter;
                dialogueText.text = displayedText;
        
                // If left mouse/button south is pressed while typing, skip typing
                if (playerInput.actions["Advance"].IsPressed())
                {
                    dialogueText.text = "";
                    dialogueText.text += text;
                    wasSkipped = true;
                    break;
                }
                
                yield return new WaitForSeconds(typingSpeed); // Typed text
            }

            if (!dialogueList[currentDialogueIndex].isQuestion)
            {
                if (wasSkipped) // If skipped, require two clicks
                {
                    yield return new WaitUntil(() => playerInput.actions["Advance"].ReadValue<float>() > 0);
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitUntil(() => playerInput.actions["Advance"].WasPressedThisFrame());

                if (IsDialogueActive)
                {
                    dialogueAnimator.SetBool("isDialogueAdvanced", true);
                    
                    yield return new WaitForSeconds(0.1f); // Wait a short amount before closing
                    
                    dialogueAnimator.SetBool("isDialogueAdvanced", false);
                }
            }

            if (dialogueList[currentDialogueIndex].isEnd)
            {
                DialogueStop();
            }

            currentDialogueIndex++;
        }
    }

    private void DialogueStop()
    {
        if (printDialogueCoroutine != null)
        {
            StopCoroutine(printDialogueCoroutine);
            printDialogueCoroutine = null;
        }

        if (typeTextCoroutine != null)
        {
            StopCoroutine(typeTextCoroutine);
            typeTextCoroutine = null;
        }
        
        ResetDialogueUI();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        EventSystem.current.SetSelectedGameObject(null);
        playerInput.SwitchCurrentActionMap("Player");

        StartCoroutine(PlayAnimationAndWait("Dialogue Disappearing", () =>
        {
            //playerFollowCamera.transform.rotation = initialCameraRotation;
            
            IsDialogueActive = false;
            ResetDialogueUI();

            if (dialogueAnimator != null)
            {
                dialogueAnimator.Rebind();
            }

            dialogueParent.SetActive(false);
        }));
    }

    private void ResetDialogueUI()
    {
        var color = dialogueParent.GetComponent<Image>().color;
        color.a = 170f / 255f;
        dialogueParent.GetComponent<Image>().color = color;
        
        option1Button.gameObject.SetActive(false);
        option2Button.gameObject.SetActive(false);
        dialogueText.text = "";
    }

    public void TelepathyXP(int xp)
    {
        int xpAmount = xp;
        LevelManager.Instance.AddXP("Telepathy", xpAmount);
    }
    
    private IEnumerator PlayAnimationAndWait(string animationName, Action onComplete)
    {
        if (dialogueAnimator.gameObject.activeInHierarchy)
        {
            dialogueAnimator.Play(animationName);
            yield return new WaitUntil(() => 
                dialogueAnimator.GetCurrentAnimatorStateInfo(0).IsName(animationName));

            float waitTime = dialogueAnimator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(waitTime);

            onComplete?.Invoke(); // Invoke the callback action
        }
    }
}
