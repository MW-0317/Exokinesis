using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ElevatorReached : MonoBehaviour
{
    public IntroUI introUI;
    public bool elevator1;
    public bool elevator2;
    public bool level3Elevator;
    private bool _objectiveComplete;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject level1Complete;
    [SerializeField] private Animator _elevatorAnimator;
    [SerializeField] private float doorAnimationSpeed = 1f;
    private DialogueQueue _dialogueQueue;

    private void Start()
    {
        // Initialize blend tree to closed
        _elevatorAnimator.SetFloat("openAmount", 0);

        var warden = GameObject.Find("Warden");
        if (warden != null) _dialogueQueue = warden.GetComponent<DialogueQueue>();
    }

    private void UpdateAnimation(float target)
    {
        // Transition from open/closed to closed/open
        float current = _elevatorAnimator.GetFloat("openAmount");
        float next = Mathf.MoveTowards(current, target, doorAnimationSpeed * Time.deltaTime);
        _elevatorAnimator.SetFloat("openAmount", next);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_objectiveComplete && elevator1)
        {
            introUI.Objective4Complete();
            _objectiveComplete = true;
            
            //StartCoroutine(DemoComplete());
        }

        if (level3Elevator)
        {
            if (!GameManager.Instance.hasKeycard3) return;
            StopAllCoroutines();
            StartCoroutine(UpdateDoor(true));
        }

        if (elevator2)
        {
            if (!_dialogueQueue.allowedPassage) return;
            StopAllCoroutines();
            StartCoroutine(UpdateDoor(true));
        }

        if (GameManager.Instance.puzzleCompleted || !elevator1 && !level3Elevator)
        {
            StopAllCoroutines();
            StartCoroutine(UpdateDoor(true));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (GameManager.Instance.puzzleCompleted || !elevator1)
        {
            StopAllCoroutines();
            StartCoroutine(UpdateDoor(false));
        }
    }

    private IEnumerator UpdateDoor(bool open)
    {
        float target = open ? 1f : 0f;
        while (_elevatorAnimator.GetFloat("openAmount") != target)
        {
            UpdateAnimation(target);
            yield return null;
        }
    }

    private IEnumerator DemoComplete()
    {
        yield return new WaitForSeconds(6);
        
        playerInput.actions.Disable();
        
        level1Complete.SetActive(true);
        Time.timeScale = 0f;
    }
}
