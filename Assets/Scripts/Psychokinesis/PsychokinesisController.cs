using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PsychokinesisController : MonoBehaviour
{
    public GameObject promptUI;
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float pickupDistance = 20f;
    private PlayerInput _playerInput;
    private bool _psychokinesisActivated;
    private Outline _outlinedObject;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (_playerInput.actions["Interact"].WasPressedThisFrame())
        {
            RaycastAndMoveObject();
        }

        OutlinedObject();
        UpdatePrompt();
    }

    private void RaycastAndMoveObject()
    {
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit, pickupDistance, layerMask))
        {
            if (raycastHit.transform.TryGetComponent<Outline>(out Outline outline) && outline.enabled)
            {
                if (raycastHit.transform.TryGetComponent<ObjectControllable>(out ObjectControllable objectControllable) && !objectControllable.HasBeenMoved)
                {
                    if (objectControllable.isKeycard1)
                    {
                        objectControllable.GetKeycard1();
                        GameManager.Instance.hasKeycard1 = true;
                    }

                    if (objectControllable.isKeycard2)
                    {
                        objectControllable.GetKeycard2();
                        GameManager.Instance.hasKeycard2 = true;
                    }

                    if (objectControllable.isKeycard3)
                    {
                        objectControllable.GetKeycard3();
                        GameManager.Instance.hasKeycard3 = true;
                    }

                    if (objectControllable.isHackable)
                    {
                        objectControllable.StartHacking();
                    }

                    if (objectControllable.isMap)
                    {
                        objectControllable.GetMap1();
                    }

                    if (objectControllable.isLore1)
                    {
                        objectControllable.ShowLore(1);
                    }
                    
                    if (objectControllable.isLore2)
                    {
                        objectControllable.ShowLore(2);
                    }
                    
                    if (objectControllable.isLore3)
                    {
                        objectControllable.ShowLore(3);
                    }

                    if (objectControllable.isBossLore)
                    {
                        objectControllable.ShowLore(4);
                    }
                    
                    else objectControllable.Move();

                    _outlinedObject.activated = true;
                }
            }
        }
    }

    private void OutlinedObject()
    {
        if (playerCameraTransform != null && Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit, pickupDistance, layerMask))
        {
            Outline outline = raycastHit.collider.GetComponent<Outline>();
            if (outline != null && OutlineManager.Instance.RequestOutline(outline))
            {
                if (_outlinedObject != null) _outlinedObject.enabled = false;
                
                _outlinedObject = outline;
                if (!_outlinedObject.activated) _outlinedObject.enabled = true;
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
                Debug.Log("Clearing outline");
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
            string promptText = "";
            switch (_outlinedObject.tag)
            {
                case "Disruption":
                    promptText = "Cause Disruption";
                    break;
                case "Openable":
                    promptText = "Open";
                    break;
                case "Hackable":
                    promptText = "Hack";
                    break;
                case "PickUp":
                    promptText = "Pick Up";
                    break;
                case "Lore":
                    promptText = "Read";
                    break;
                default:
                    promptText = "Interact";
                    break;
            }
            promptUI.GetComponentInChildren<TextMeshProUGUI>().text = promptText;
            
            promptUI.gameObject.SetActive(true);
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(_outlinedObject.transform.position);
            promptUI.transform.position = screenPosition + new Vector3(42, -50, 0);
        }
        else
        {
            promptUI.gameObject.SetActive(false);
        }
    }
}
