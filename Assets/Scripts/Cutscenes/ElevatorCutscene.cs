using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ElevatorCutscene : MonoBehaviour
{
    [SerializeField] private GameObject elevatorCutscene;
    [SerializeField] private FadingController fadingController;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private MusicManager musicManager;
    
    private void OnTriggerEnter(Collider other)
    {
        playerInput.SwitchCurrentActionMap("Cutscene");
        StartCoroutine(StartCutscene());
    }

    IEnumerator StartCutscene()
    {
        fadingController.FadeOut(2f);
        musicManager.FadeMusicOut(4f);

        yield return new WaitForSeconds(2f);
        
        elevatorCutscene.GameObject().SetActive(true);
        fadingController.FadeIn(2f);

        yield return new WaitForSeconds(22f); // Duration of cutscene
        
        musicManager.FadeMusicIn();
        playerInput.SwitchCurrentActionMap("Player");
    }
}
