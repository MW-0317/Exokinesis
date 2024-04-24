using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class AutoDoor : MonoBehaviour
{
    private PlayerInput _playerInput;
    public Animator doorAnim;
    public AudioSource openSound;
    public AudioSource closeSound;
    public bool isOpen;
    public bool isLevel3 = false;
    public bool isLevel2 = false;
    public bool isLevel1 = false;
    public bool isManualAccess = false;
    public bool alwaysLocked = false;
    public bool isBroken = false;
    // private bool canToggle = false; // Initialize as false

    private List<Collider> _colliders = new List<Collider>(); // Track all colliders inside

    private void Start()
    {
        GameObject playerArmature = GameObject.FindGameObjectWithTag("Player");
        if (playerArmature != null)
        {
            _playerInput = playerArmature.GetComponent<PlayerInput>();
        }
        
        // Disable audio sources at the start to prevent them from playing automatically
        if (openSound != null)
            openSound.Stop();
        if (closeSound != null)
            closeSound.Stop();
    }

    private void Update()
    {
        if (_colliders.Count == 0) return;

        foreach (var collider in _colliders)
        {
            bool isPlayerInteracting = _playerInput.actions["Interact"].ReadValue<float>() > 0;
            BaseNPC? npc = collider.GetComponent<BaseNPC>();
            bool isNPCInteracting = false;
            if (npc == null) isNPCInteracting = false;
            else isNPCInteracting = npc.Interact();

            if (collider.CompareTag("Player") && isLevel3)
            {
                if (!GameManager.Instance.hasKeycard1)
                {
                    return;
                }
            }
            if (collider.CompareTag("Player") && isLevel2)
            {
                if (!GameManager.Instance.hasKeycard2)
                {
                    return;
                }
            }
            if (collider.CompareTag("Player") && isLevel1)
            {
                if (!GameManager.Instance.hasKeycard3)
                {
                    return;
                }
            }
            if (collider.CompareTag("Player") && isManualAccess)
            {
                if (!GameManager.Instance.puzzleCompleted)
                {
                    return;
                }
            }
            if (collider.CompareTag("Player") && alwaysLocked)
            {
                return;
            }

            if ((collider.CompareTag("Player") && isPlayerInteracting) || (collider.CompareTag("NPC") && isNPCInteracting))
            {
                ToggleDoor(true);
                isOpen = true;
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("NPC"))
        {
            if (!_colliders.Contains(other))
            {
                _colliders.Add(other);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_colliders.Contains(other))
        {
            _colliders.Remove(other); // Remove collider from list

            if (isOpen && _colliders.Count == 0) // Close door if it's open and no colliders are inside
            {
                isOpen = false;
                ToggleDoor(false);
            }
        }
    }

    private void ToggleDoor(bool open)
    {
        if (doorAnim != null)
        {
            if (open)
            {
                doorAnim.SetBool("isOpen", true);
                if (openSound != null)
                    openSound.Play();
            }
            else
            {
                doorAnim.SetBool("isOpen", false);
                if (closeSound != null)
                    closeSound.Play();
            }
        }
    }
}
