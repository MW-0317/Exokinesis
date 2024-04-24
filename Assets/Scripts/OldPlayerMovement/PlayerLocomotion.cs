using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class PlayerLocomotion : MonoBehaviour
{
    private PlayerManager playerManager;
    private AnimatorManager animatorManager;
    private InputManager inputManager;
    
    private Vector3 moveDirection;
    private Vector3 headPosition;
    private Transform cameraObject;
    private CapsuleCollider characterCollider;
    public Rigidbody playerRigidbody;

    private bool canStand;

    [Header("Falling")] 
    public float inAirTimer;
    public float leapingVelocity;
    public float fallingVelocity;
    public float rayCastHeightOffSet = 0.5f;
    public LayerMask groundLayer;
    
    [Header("Movement Flags")]
    public bool isSprinting;
    public bool isGrounded;
    public bool isJumping;
    public bool isCrouching;
    
    [Header("Movement Speeds")]
    public float sneakingSpeed = 1.5f;
    [FormerlySerializedAs("movementSpeed")] public float walkingSpeed = 3;
    public float sprintingSpeed = 5;
    public float rotationSpeed = 50;

    [Header("Jump Speeds")] 
    public float jumpHeight = 3;
    public float gravityIntensity = -15; // Must be negative
    
    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        animatorManager = GetComponent<AnimatorManager>();
        inputManager = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;
    }

    public void HandleAllMovement()
    {
        HandleFallingAndLanding();
        
        if (playerManager.isInteracting)
            return;
        
        HandleMovement();
        HandleRotation();
    }
    
    private void HandleMovement()
    {
        if (isJumping)
            return;
        
        moveDirection = cameraObject.forward * inputManager.verticalInput;
        moveDirection = moveDirection + cameraObject.right * inputManager.horizontalInput;
        moveDirection.y = 0;
        moveDirection.Normalize();

        if (isSprinting)
        {
            moveDirection = moveDirection * sprintingSpeed;
        }

        if (isCrouching)
        {
            moveDirection = moveDirection * sneakingSpeed;
        }
        
        else
        {
            if (inputManager.moveAmount >= 0.5f)
            {
                moveDirection = moveDirection * walkingSpeed;
            }
            else
            {
                moveDirection = moveDirection * sneakingSpeed;
            }
        }
        
        moveDirection = moveDirection * walkingSpeed;

        Vector3 movementVelocity = moveDirection;
        playerRigidbody.velocity = movementVelocity;
    }

    private void HandleRotation()
    {
        if (isJumping)
            return;
        
        Vector3 targetDirection = Vector3.zero;

        targetDirection = cameraObject.forward * inputManager.verticalInput;
        targetDirection = targetDirection + cameraObject.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation =
            Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    private void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        rayCastOrigin.y = rayCastOrigin.y + rayCastHeightOffSet;

        if (!isGrounded && !isJumping)
        {
            if (!playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Falling", true);
            }

            inAirTimer = inAirTimer + Time.deltaTime;
            playerRigidbody.AddForce(transform.forward * leapingVelocity);
            playerRigidbody.AddForce(-Vector3.up * (fallingVelocity * inAirTimer));
        }

        if (Physics.SphereCast(rayCastOrigin, 0.2f, -Vector3.up, out hit, groundLayer))
        {
            if (!isGrounded && !playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Landing", true);
            }
            
            inAirTimer = 0;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    public void HandleJumping()
    {
        if (isGrounded)
        {
            animatorManager.animator.SetBool("isJumping", true);
            animatorManager.PlayTargetAnimation("Jumping", false);

            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y = jumpingVelocity;
            playerRigidbody.velocity = playerVelocity;
        }
    }
    
    public void HandleCrouching()
    {
        characterCollider = GetComponent<CapsuleCollider>();
        
        if (isCrouching)
        {
            isCrouching = false;
            animatorManager.animator.SetBool("isCrouching", false);
            characterCollider.height = 1.78f;
            characterCollider.center = new Vector3(0, 0.89f, 0);
            characterCollider.radius = 0.22f;
        }
        else
        {
            isCrouching = true;
            animatorManager.animator.SetBool("isCrouching", true);
            characterCollider.height = 1.53f;
            characterCollider.center = new Vector3(0, 0.75f, 0);
            characterCollider.radius = 0.5f;
        }
    }
}
