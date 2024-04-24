using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using RaycastHit = UnityEngine.RaycastHit;

public class TelekinesisController : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float pickupDistance = 20f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float promptCooldown = 1.0f;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private AudioSource errorSound;
    public bool waitForInputRelease;

    private Vector3 _roomMinBoundary;
    private Vector3 _roomMaxBoundary;
    private Vector3 _centerPosition;

    public IntroUI introUI;
    public GameObject promptUI;
    public TMP_Text promptText;

    private Vector3 _initialPosition;
    private bool _isTelekinesisActive = false;
    private bool _isCooldownActive = false;
    private float _cooldownTimer = 0f;
    
    private ObjectGrabbable _objectGrabbable;
    private Outline _outlinedObject;
    
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        PlayerSubscribe();
    }

    private void Update()
    {
        if (_objectGrabbable != null)
        {
            _roomMinBoundary = _objectGrabbable.roomMinBoundary;
            _roomMaxBoundary = _objectGrabbable.roomMaxBoundary;
            
            ObjectMovement();
            Respawn();
        }

        if (waitForInputRelease)
        {
            if (!IsMovementKeyPressed())
            {
                waitForInputRelease = false;
                playerInput.SwitchCurrentActionMap("Player");
            }
        }
    }

    private void LateUpdate()
    {
        if (_isCooldownActive)
        {
            _cooldownTimer += Time.deltaTime;
            if (_cooldownTimer >= promptCooldown)
            {
                _isCooldownActive = false;
                _cooldownTimer = 0f;
            }
        }

        if (!_isCooldownActive)
        {
            // if (_objectGrabbable == null) // Only highlight objects if none are currently grabbed
            // {
            //     OutlinedObject();
            // }
        
            UpdatePrompt();
        }
        
        OutlinedObject();
    }

    private void TelekinesisInteract()
    {
        if (_objectGrabbable == null && _outlinedObject != null && _outlinedObject.objectMass <= GameManager.Instance.maxTelekinesisWeight)
        {
            if (playerCameraTransform != null && Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit, pickupDistance, layerMask))
            {
                if (raycastHit.transform.TryGetComponent(out _objectGrabbable))
                {
                    objectGrabPointTransform.position = raycastHit.transform.position;
                    StartCoroutine(StartTelekinesis());
                }
                else CannotPickUp();
            }
            else CannotPickUp();
        }
        else
        {
            if (_outlinedObject != null && _outlinedObject.objectMass > GameManager.Instance.maxTelekinesisWeight)
            {
                CannotPickUp();
            }
            else
            {
                if (_isTelekinesisActive)
                {
                    // Exit telekinesis, set cooldown
                    ExitTelekinesis();
                }
            }
        }
    }

    private bool IsMovementKeyPressed()
    {
        bool isMoving = playerInput.actions["MoveObject"].ReadValue<Vector2>().magnitude > 0;
        return isMoving;
    }

    private void ObjectMovement()
    {
        Vector3 moveDirection = (playerCameraTransform.forward * Input.GetAxis("Vertical")) + (playerCameraTransform.right * Input.GetAxis("Horizontal"));
        moveDirection = new Vector3(moveDirection.x, 0, moveDirection.z);
        
        if (playerInput.actions["MoveUp"].ReadValue<float>() > 0)
        {
            moveDirection += Vector3.up;
        }
        else if (playerInput.actions["MoveDown"].ReadValue<float>() > 0)
        {
            moveDirection -= Vector3.up;
        }
        
        objectGrabPointTransform.position += moveDirection * (moveSpeed * Time.deltaTime);
    }

    private void PlayerRotation()
    {
        Vector3 directionToTarget = (objectGrabPointTransform.position - playerTransform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);

        float yOffset = 60f; // Offset to the right (for the animation)
        // Extract only the y-component of the rotation
        Quaternion yRotation = Quaternion.Euler(0, lookRotation.eulerAngles.y + yOffset, 0);

        playerTransform.rotation = yRotation;
    }

    private void Respawn()
    {
        _centerPosition = (_roomMinBoundary + _roomMaxBoundary) / 2;
        
        if (_objectGrabbable != null)
        {
            Vector3 currentPosition = objectGrabPointTransform.position;
            // Check if the object's position is outside the specified boundaries
            if (currentPosition.x < _roomMinBoundary.x || currentPosition.x > _roomMaxBoundary.x ||
                currentPosition.y < _roomMinBoundary.y || currentPosition.y > _roomMaxBoundary.y ||
                currentPosition.z < _roomMinBoundary.z || currentPosition.z > _roomMaxBoundary.z)
            {
                if (_objectGrabbable != null)
                {
                    Debug.Log("Respawning");
                    // Use center position
                    Vector3 respawnPosition = _centerPosition;
                    
                    // Reset the object to the initial position
                    objectGrabPointTransform.position = respawnPosition;
                    _objectGrabbable.transform.position = respawnPosition;
                    
                    // Exit telekinesis, play error and set cooldown
                    ExitTelekinesis();
                    errorSound.Play();
                }
            }
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // Calculate the center position of the boundary box
        Vector3 center = (_roomMinBoundary + _roomMaxBoundary) * 0.5f;

        // Calculate the size of the boundary box
        Vector3 size = new Vector3(
            Mathf.Abs(_roomMaxBoundary.x - _roomMinBoundary.x),
            Mathf.Abs(_roomMaxBoundary.y - _roomMinBoundary.y),
            Mathf.Abs(_roomMaxBoundary.z - _roomMinBoundary.z));
        
        Gizmos.DrawWireCube(center, size);
    }

    public void ExitTelekinesis()
    {
        if (_objectGrabbable != null)
        {
            _objectGrabbable.AddKinesisXp();
        
            _objectGrabbable.Drop();
            _objectGrabbable = null;
        }
        
        waitForInputRelease = true;
        
        introUI.CloseTelekinesisControls();
        _animator.SetBool("Telekinesis", false);
        
        _isCooldownActive = true;
        _cooldownTimer = 0f;

        _isTelekinesisActive = false;
    }

    public void CannotPickUp()
    {
        Debug.LogError("Cannot pick up");
        waitForInputRelease = true;
        // _isCooldownActive = true;
        // _cooldownTimer = 0f;
        errorSound.Play();
    }

    private void OutlinedObject()
    {
        if (!_isCooldownActive && !_isTelekinesisActive && playerCameraTransform != null && Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit, pickupDistance, layerMask))
        {
            Outline outline = raycastHit.collider.GetComponent<Outline>();
            if (outline != null && OutlineManager.Instance.RequestOutline(outline))
            {
                if (_outlinedObject != null) _outlinedObject.enabled = false;
                
                _outlinedObject = outline;
                _outlinedObject.enabled = true;
            }
            else
            {
                if (_outlinedObject != null)
                {
                    Debug.Log("Clearing outline");
                    OutlineManager.Instance.ClearOutline(_outlinedObject);
                    _outlinedObject.enabled = false;
                    _outlinedObject = null;
                }
            }
        }
        else
        {
            if (_outlinedObject != null)
            {
                OutlineManager.Instance.ClearOutline(_outlinedObject);
                _outlinedObject.enabled = false;
                _outlinedObject = null;
            }
        }
    }

    private void UpdatePrompt()
    {
        if (_outlinedObject != null && _outlinedObject.enabled)
        {
            promptUI.gameObject.SetActive(true);
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(_outlinedObject.transform.position);
            promptUI.transform.position = screenPosition + new Vector3(42, -50, 0); // Adjust as needed
            promptText.text = $"Mass: {_outlinedObject.objectMass}/{GameManager.Instance.maxTelekinesisWeight} g";
        }
        else
        {
            promptUI.gameObject.SetActive(false);
        }
    }
    
    private void PlayerSubscribe()
    {
        var currentMap = playerInput.currentActionMap;
        if (currentMap != null)
        {
            currentMap.FindAction("Interact").performed += _ => TelekinesisInteract();
        }
    }

    private void PlayerUnsubscribe()
    {
        var currentMap = playerInput.currentActionMap;
        if (currentMap != null)
        {
            currentMap.FindAction("Interact").performed -= _ => TelekinesisInteract();
        }
    }
    
    private void TelekinesisSubscribe()
    {
        var currentMap = playerInput.currentActionMap;
        if (currentMap != null)
        {
            currentMap.FindAction("Interact").performed += _ => TelekinesisInteract();
        }
    }

    private void TelekinesisUnsubscribe()
    {
        var currentMap = playerInput.currentActionMap;
        if (currentMap != null)
        {
            currentMap.FindAction("Interact").performed -= _ => TelekinesisInteract();
        }
    }

    private IEnumerator StartTelekinesis()
    {
        _isTelekinesisActive = true;
        // Ensure the outline is disabled
        if (_outlinedObject != null)
        {
            _outlinedObject.enabled = false;
            _outlinedObject = null;
        }
        
        PlayerUnsubscribe();
        // Disable character movement
        playerInput.SwitchCurrentActionMap("Telekinesis");
        TelekinesisSubscribe();
        
        PlayerRotation();
        
        // Enable the animator bool
        _animator.SetBool("Telekinesis", true);
        yield return new WaitForSeconds(1); // Delays enabling movement
        
        _objectGrabbable.Grab(objectGrabPointTransform);
        introUI.OpenTelekinesisControls();
    }
}
