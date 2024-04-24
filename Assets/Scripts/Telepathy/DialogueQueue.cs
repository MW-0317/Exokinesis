using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueQueue : MonoBehaviour
{
    [SerializeField] private List<DialogueTrigger> dialogueTriggers = new List<DialogueTrigger>();
    [SerializeField] private DialogueTrigger correctTrigger;
    [SerializeField] private DialogueTrigger incorrectTrigger;
    private DialogueTrigger currentDialogueTrigger;
    private int index = 0;
    private Transform playerTransform;
    public bool allowedPassage = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!(dialogueTriggers.Count > 0)) return;
        currentDialogueTrigger = dialogueTriggers[index];
        currentDialogueTrigger.enabled = true;

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if ((correctTrigger == null || incorrectTrigger == null) 
            && currentDialogueTrigger != null
            && !currentDialogueTrigger.HasSpoken 
            && !playerTransform.gameObject.GetComponent<DialogueController>().IsDialogueActive)
        {
            if (currentDialogueTrigger.enabled)
            {
                currentDialogueTrigger.StartDialogue();
                currentDialogueTrigger = null;
            }
            return;
        }
        if (currentDialogueTrigger
            && !currentDialogueTrigger.HasSpoken 
            || playerTransform.gameObject.GetComponent<DialogueController>().IsDialogueActive) 
            return;
        if (index + 1 >= dialogueTriggers.Count)
        {
            if (correctTrigger == null || incorrectTrigger == null)
            {
                if (currentDialogueTrigger != null && currentDialogueTrigger.enabled)
                    currentDialogueTrigger.StartDialogue();
                return;
            }

            int correct = 0;
            foreach (dialogueString diaString in dialogueTriggers[dialogueTriggers.Count - 1].dialogueStrings)
            {
                if (diaString.correctChoicePicked)
                    correct++;
            }

            currentDialogueTrigger.enabled = false;
            if (correct >= dialogueTriggers[dialogueTriggers.Count - 1].AmountToGetCorrect)
            {
                currentDialogueTrigger = correctTrigger;
                allowedPassage = true;
            }
            else
                currentDialogueTrigger = incorrectTrigger;
            currentDialogueTrigger.enabled = true;

            correctTrigger = null;
            incorrectTrigger = null;

            return;
        }
        index++;
        currentDialogueTrigger.enabled = false;
        currentDialogueTrigger = dialogueTriggers[index];
        currentDialogueTrigger.enabled = true;
    }
}
