using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class SkillTreeManager : MonoBehaviour
{
    public GameObject skillsPrefab;
    
    [SerializeField] private GameObject uiElements;
    [SerializeField] private GameObject firstSelected;
    public bool areSkillsVisible;
    
    private IntroUI _introUI;
    private PlayerInput _playerInput;
    
    private InputActionMap _previousActionMap;

    private void Awake()
    {
        _introUI = GameObject.Find("PlayerArmature").GetComponent<IntroUI>();
        _playerInput = GameObject.Find("PlayerArmature").GetComponent<PlayerInput>();
    }

    private void Start()
    {
        Subscribe();
    }

    private void ToggleObjectives()
    {
        if (areSkillsVisible)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    // If Escape/Start is pressed
    private void ToPauseMenu()
    {
        if (areSkillsVisible)
        {
            Resume();
        }
    }
    
    private void Resume()
    {
        EventSystem.current.SetSelectedGameObject(null);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        Unsubscribe();
        if(_playerInput != null) _playerInput.SwitchCurrentActionMap(_previousActionMap.name);
        
        _introUI.ShowUI();
        skillsPrefab.SetActive(false);
        Time.timeScale = 1f;
        areSkillsVisible = false;

        GameManager.Instance.resumed = true;
    }

    private void Pause()
    {
        EventSystem.current.SetSelectedGameObject(firstSelected);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        _previousActionMap = _playerInput.currentActionMap;
        if (_playerInput != null) _playerInput.SwitchCurrentActionMap("Skills");
        Subscribe();
        
        _introUI.HideUI();
        if (skillsPrefab != null) skillsPrefab.SetActive(true);
        Time.timeScale = 0f;
        areSkillsVisible = true;
    }
    
    private void Subscribe()
    {
        if (_playerInput == null)
        {
            Debug.LogError("playerInput is null");
            return;
        }

        var currentMap = _playerInput.currentActionMap;
        if (currentMap == null)
        {
            Debug.LogError("currentActionMap is null");
            return;
        }
        
        currentMap.FindAction("Skills").performed += _ => ToggleObjectives();
        currentMap.FindAction("Pause").performed += _ => ToPauseMenu();
    }

    private void Unsubscribe()
    {
        var currentMap = _playerInput.currentActionMap;
        if (currentMap != null)
        {
            currentMap.FindAction("Skills").performed -= _ => ToggleObjectives();
            currentMap.FindAction("Pause").performed -= _ => ToPauseMenu();
        }
    }
}
