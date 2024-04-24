using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class StorageCutscene : MonoBehaviour
{
    [SerializeField] private GameObject storageCutscene;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private IntroUI ui;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!GameManager.Instance.storageCutscenePlayed)
        {
            playerInput.SwitchCurrentActionMap("Cutscene");
            StartCoroutine(StartCutscene());
        }
    }

    IEnumerator StartCutscene()
    {
        GameManager.Instance.storageCutscenePlayed = true;
        
        storageCutscene.GameObject().SetActive(true);

        yield return new WaitForSeconds(13f); // Duration of cutscene
        
        playerInput.SwitchCurrentActionMap("Player");

        yield return new WaitForSeconds(1f);
        
        ui.Objective2Complete();
    }
}