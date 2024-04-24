using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputUIManager : MonoBehaviour
{
    public Sprite keyboardSprite;
    public Sprite xboxSprite;
    public Sprite ps5Sprite;
    public Sprite ps4Sprite;
    public Sprite switchSprite;
    public Sprite steamSprite;
    public Sprite stadiaSprite;
    
    public Vector3 controllerScale = new Vector3(1f, 1f, 1f);
    public Vector3 controllerPosition = new Vector3(1f, 1f, 1f);
    public bool useControllerScale = false;
    public bool useControllerPosition = false;

    public string childImageObjectName = "E";
    private Image promptImage;
    private Color _showing = new(1f, 1f, 1f, 1f);
    private Color _hidden = new(1f, 1f, 1f, 0f);

    private void Awake()
    {
        var childObject = transform.Find(childImageObjectName);
        if (childObject != null)
        {
            promptImage = childObject.GetComponent<Image>();
        }
        else
        {
            Debug.LogError("Child object not found");
        }
    }

    private void OnEnable()
    {
        InputSystem.onActionChange += OnActionChange;
    }

    private void OnDisable()
    {
        InputSystem.onActionChange -= OnActionChange;
    }
    
    private void OnActionChange(object obj, InputActionChange change)
    {
        if (change == InputActionChange.ActionPerformed)
        {
            var inputAction = (InputAction)obj;
            var lastControl = inputAction.activeControl;
            var lastDevice = lastControl.device;
        
            UpdatePromptImage(lastDevice);
        }
    }

    private void UpdatePromptImage(InputDevice device)
    {
        promptImage.color = _hidden; // Default, if no sprite assigned
        
        if (device.displayName.Contains("Keyboard") || device.displayName.Contains("Mouse"))
        {
            promptImage.sprite = keyboardSprite;
            if (keyboardSprite != null) promptImage.color = _showing;
        }
        if (device.displayName.Contains("Xbox"))
        {
            promptImage.sprite = xboxSprite;
            if (xboxSprite != null) promptImage.color = _showing;
            if (useControllerScale) promptImage.transform.localScale = controllerScale;
            if (useControllerPosition) promptImage.transform.localPosition = controllerPosition;
        }
        else if (device.displayName.Contains("DualSense"))
        {
            promptImage.sprite = ps5Sprite;
            if (ps5Sprite != null) promptImage.color = _showing;
            if (useControllerScale) promptImage.transform.localScale = controllerScale;
            if (useControllerPosition) promptImage.transform.localPosition = controllerPosition;
        }
        else if (device.displayName.Contains("DualShock"))
        {
            promptImage.sprite = ps4Sprite;
            if (ps4Sprite != null) promptImage.color = _showing;
            if (useControllerScale) promptImage.transform.localScale = controllerScale;
            if (useControllerPosition) promptImage.transform.localPosition = controllerPosition;
        }
        else if (device.displayName.Contains("Switch"))
        {
            promptImage.sprite = switchSprite;
            if (switchSprite != null) promptImage.color = _showing;
            if (useControllerScale) promptImage.transform.localScale = controllerScale;
            if (useControllerPosition) promptImage.transform.localPosition = controllerPosition;
        }
        else if (device.displayName.Contains("Steam"))
        {
            promptImage.sprite = steamSprite;
            if (steamSprite != null) promptImage.color = _showing;
            if (useControllerScale) promptImage.transform.localScale = controllerScale;
            if (useControllerPosition) promptImage.transform.localPosition = controllerPosition;
        }
        else if (device.displayName.Contains("Stadia"))
        {
            promptImage.sprite = stadiaSprite;
            if (stadiaSprite != null) promptImage.color = _showing;
            if (useControllerScale) promptImage.transform.localScale = controllerScale;
            if (useControllerPosition) promptImage.transform.localPosition = controllerPosition;
        }
    }
}
