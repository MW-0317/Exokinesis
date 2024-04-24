using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoverManager : MonoBehaviour
{
    public bool IsInCover { get; private set; }
    public Vector3 CoverDirection { get; private set; }
    public float CoverSpeed { get; private set; }
    
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Animator animator;
    
    public float detectionRange = 5f;
    public float coverMoveSpeed = 5f;
    public Vector3 movementDirection;
    public bool isInCover;
    private Transform coverObject; // Current cover object
    private Vector3 _coverHitPoint;
    private Vector3 _velocity = Vector3.zero;
    private Quaternion _originalRotation;
    
    private bool coverInputReleased = true;
    private bool isFrontSide;
    private float coverToggleCooldown = 0.5f;
    private float lastCoverToggleTime = -1f;
    private float coverDirection = 0f;

    public void SetCoverStatus(bool inCover, Vector3 direction, float speed)
    {
        IsInCover = inCover;
        CoverDirection = direction;
        CoverSpeed = speed;
    }

    private void LateUpdate()
    {
        float coverInput = playerInput.actions["Cover"].ReadValue<float>();
        bool isCoverInputActive = coverInput > 0;
    
        // Toggle cover cooldown
        bool canToggleCover = (Time.time - lastCoverToggleTime) > coverToggleCooldown;
        
        if (isCoverInputActive && coverInputReleased && canToggleCover)
        {
            if (!isInCover)
            {
                TryEnterCover();
            }
            else
            {
                ExitCover();
            }
            lastCoverToggleTime = Time.time;
            coverInputReleased = false;
        }
        else if (!isCoverInputActive)
        {
            coverInputReleased = true;
        }

        if (isInCover)
        {
            MoveAlongCover();
        }
    }

    private void TryEnterCover()
    {
        float verticalOffset = 1.5f;
        RaycastHit hit;
        Vector3 direction = playerTransform.forward;
        Vector3 rayOrigin = playerTransform.position + Vector3.up * verticalOffset;
        
        if (Physics.Raycast(rayOrigin, direction, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("isCover"))
            {
                Debug.DrawLine(rayOrigin, hit.point, Color.green, 2f);
                
                Vector3 toCover = (hit.point - playerTransform.position).normalized;
                float dot = Vector3.Dot(hit.transform.forward, toCover);

                // Determine cover direction based on dot product
                isFrontSide = dot < 0;
                
                _coverHitPoint = hit.point;
                isInCover = true;
                coverObject = hit.transform;
                
                AlignWithCover(isFrontSide);
                
                Vector3 coverDirection = isFrontSide ? coverObject.forward * -1 : coverObject.forward;
                SetCoverStatus(true, coverDirection, coverMoveSpeed);
            }
            else
            {
                Debug.DrawLine(rayOrigin, hit.point, Color.red, 2f);
            }
        }
        else
        {
            Debug.DrawLine(rayOrigin, rayOrigin + direction * detectionRange, Color.blue, 2f);
        }
    }

    private void AlignWithCover(bool isFrontSide)
    {
        playerInput.SwitchCurrentActionMap("Cover");
        StartCoroutine(MoveToCover(isFrontSide));
    }

    private void MoveAlongCover()
    {
        if (!isInCover) return;
        
        float horizontalInput = Input.GetAxis("Horizontal");
        horizontalInput *= -1; // Invert input direction
        Vector3 movementDirection = coverObject.forward;
        if (!isFrontSide) movementDirection *= -1; // Invert movement direction

        // Calculate the desired movement vector
        Vector3 movement = movementDirection * (horizontalInput * coverMoveSpeed * Time.deltaTime);
        Vector3 newPosition = playerTransform.position + movement;
        
        // Bounds check
        // Collider coverCollider = coverObject.GetComponent<Collider>();
        // if (coverCollider != null)
        // {
        //     Bounds coverBounds = coverCollider.bounds;
        //     
        //     float playerWidthHalf = 0.5f;
        //     if (newPosition.x - playerWidthHalf < coverBounds.min.x || newPosition.x + playerWidthHalf > coverBounds.max.x)
        //     {
        //         // If moving would take the player out of bounds, adjust newPosition.x to stay within bounds
        //         newPosition.x = Mathf.Clamp(newPosition.x, coverBounds.min.x + playerWidthHalf, coverBounds.max.x - playerWidthHalf);
        //     }
        // }

        playerTransform.position = newPosition;
        
        UpdateCoverAnimation(horizontalInput);
    }

    private void ExitCover()
    {
        StartCoroutine(MoveFromCover());
        SetCoverStatus(false, Vector3.zero, 0f);
    }

    private void UpdateCoverAnimation(float horizontalInput)
    {
        // Determine the direction of movement
        if (horizontalInput < 0) 
        {
            coverDirection = 0; // Left
        }
        else if (horizontalInput > 0)
        {
            coverDirection = 1; // Right
        }
        animator.SetFloat("CoverDirection", coverDirection);

        float coverSpeed = Mathf.Abs(horizontalInput) * 4;
        animator.SetFloat("CoverSpeed", coverSpeed);
    }

    IEnumerator MoveToCover(bool isFrontSide)
    {
        animator.SetBool("Cover", true);
        yield return new WaitForSeconds(0.5f); // Wait until about halfway through animation
        
        Vector3 startPosition = playerTransform.position;
        Vector3 coverDirection = (_coverHitPoint - startPosition).normalized; // Direction from player to cover
        float coverDistance = 0.4f; // How far back from the cover hit point the player should be
        Vector3 endPosition = _coverHitPoint - coverDirection * coverDistance;
        endPosition.y = startPosition.y; // Maintain the player's current height in the end position

        float animationDuration = 0.5f;
        float time = 0;

        _originalRotation = playerTransform.rotation;
        // Determine the target rotation
        Quaternion targetRotation = isFrontSide ? Quaternion.LookRotation(-coverObject.forward) : Quaternion.LookRotation(coverObject.forward);
        targetRotation *= Quaternion.Euler(0, 90, 0);

        while (time < animationDuration)
        {
            playerTransform.position = Vector3.Lerp(startPosition, endPosition, time / animationDuration);
            // Smoothly rotate the player to face the cover
            playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, targetRotation, time / animationDuration);
            time += Time.deltaTime;
            yield return null;
        }

        playerTransform.position = endPosition; // Ensure the player is exactly at the end position after the loop
        playerTransform.rotation = targetRotation; // Ensure the player is exactly facing the target rotation after the loop

        isInCover = true;
    }

    IEnumerator MoveFromCover()
    {
        isInCover = false;
        coverObject = null;
        animator.SetBool("Cover", false);

        yield return new WaitForSeconds(1f);
        
        playerTransform.rotation = _originalRotation;
        
        playerInput.SwitchCurrentActionMap("Player");
    }
}
