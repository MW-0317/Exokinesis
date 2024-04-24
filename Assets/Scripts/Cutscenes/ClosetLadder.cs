using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClosetLadder : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject closetCutscene;
    [SerializeField] private GameObject closetObjects;
    [SerializeField] private float cutsceneDuration = 7f;
    
    private void OnTriggerEnter(Collider other)
    {
        playerInput.SwitchCurrentActionMap("Cutscene");
        playerAnimator.SetBool("Crouch", false);
        // playerAnimator.SetFloat("Speed", 2.0f);
        StartCoroutine(StartCutscene());
    }
    
    IEnumerator StartCutscene()
    {
        closetCutscene.GameObject().SetActive(true);
        closetObjects.GameObject().SetActive(true);

        yield return new WaitForSeconds(cutsceneDuration); // Duration of cutscene
        
        playerInput.SwitchCurrentActionMap("Player");
        // closetCutscene.GameObject().SetActive(false);
    }
}
