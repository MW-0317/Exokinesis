using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArmoryLadder : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject armoryCutscene;
    [SerializeField] private GameObject armoryObjects;
    
    private void OnTriggerEnter(Collider other)
    {
        playerInput.SwitchCurrentActionMap("Cutscene");
        playerAnimator.SetBool("Crouch", false);
        StartCoroutine(StartCutscene());
    }
    
    IEnumerator StartCutscene()
    {
        armoryCutscene.GameObject().SetActive(true);
        armoryObjects.GameObject().SetActive(true);

        yield return new WaitForSeconds(5f); // Duration of cutscene
        
        playerInput.SwitchCurrentActionMap("Player");
        armoryCutscene.GameObject().SetActive(false);
    }
}