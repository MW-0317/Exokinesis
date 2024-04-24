using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnlockBars : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private TelekinesisController controller;
    [SerializeField] private ObjectGrabbable objectGrabbable;
    private Animator _animator;
    public IntroUI introUI;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _animator.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key"))
        {
            OpenBars(other);
        }
    }
    
    public void OpenBars(Collider keyObject)
    {
        _animator.enabled = true;
        
        objectGrabbable.PrisonCellOpened();

        controller.waitForInputRelease = true; // Enable player movement
        introUI.CloseTelekinesisControls();

        float animationLength = _animator.GetCurrentAnimatorStateInfo(0).length;
        
        introUI.TriggerIntroControls(); // Show controls
        introUI.Objective1Complete();
        controller.ExitTelekinesis();
        
        Destroy(gameObject, animationLength);
    }
}
