using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerControls playerControls;
    private PlayerLocomotion playerLocomotion;
    private AnimatorManager animatorManager;

    public Vector2 movementInput;
    public Vector2 cameraInput;

    public float cameraInputX;
    public float cameraInputY;
    
    public float moveAmount;
    public float verticalInput;
    public float horizontalInput;

    public bool run_Input;
    public bool jump_Input;
    public bool crouch_Input;
    public bool dialogue_Input;

    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    private void OnEnable() 
    {
        if (playerControls == null) 
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
            
            playerControls.PlayerActions.Running.performed += i => run_Input = true;
            playerControls.PlayerActions.Running.canceled += i => run_Input = false;

            playerControls.PlayerActions.Jumping.performed += i => jump_Input = true;

            playerControls.PlayerActions.Crouching.performed += i => crouch_Input = true;

            playerControls.PlayerActions.StartingDialogue.performed += i => dialogue_Input = true;
        }
        
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleRunningInput();
        HandleJumpingInput();
        HandleCrouchingInput();
        // Handle_____Input as needed
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
        
        cameraInputY = cameraInput.y;
        cameraInputX = cameraInput.x;
        
        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));
        animatorManager.UpdateAnimatorValues(0, moveAmount, playerLocomotion.isSprinting);
    }
    
    private void HandleRunningInput()
    {
        if (run_Input && moveAmount > 0.5f)
        {
            playerLocomotion.isSprinting = true;
        }
        else
        {
            playerLocomotion.isSprinting = false;
        }
    }

    private void HandleJumpingInput()
    {
        if (jump_Input)
        {
            jump_Input = false;
            playerLocomotion.HandleJumping();
        }
    }

    private void HandleCrouchingInput()
    {
        if (crouch_Input)
        {
            crouch_Input = false;
            playerLocomotion.HandleCrouching();
        }
    }

    private void HandleDialogueInput()
    {
        if (dialogue_Input)
        {
            dialogue_Input = false;
            //FindObjectOfType<DialogueTrigger>().StartDialogue();
        }
    }
}