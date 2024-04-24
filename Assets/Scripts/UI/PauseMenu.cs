using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    
    public GameObject playerArmature;
    public string menuName;
    public GameObject pauseMenuUI;
    
    private PlayerInput _playerInput;
    private ObjectivesManager _objectivesManager;
    [SerializeField] private GameObject firstSelected;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject firstSettingsSelected;

    private IntroUI _introUI;
    
    private bool _canPause = true;
    private InputActionMap _previousActionMap;

    private void Start()
    {
        _introUI = GameObject.Find("PlayerArmature").GetComponent<IntroUI>();
        _playerInput = GameObject.Find("PlayerArmature").GetComponent<PlayerInput>();
        _objectivesManager = GameObject.Find("Canvas").GetComponent<ObjectivesManager>();
        Subscribe();
    }

    private void OnEnable()
    {
        _introUI = GameObject.Find("PlayerArmature").GetComponent<IntroUI>();
        _playerInput = GameObject.Find("PlayerArmature").GetComponent<PlayerInput>();
        _objectivesManager = GameObject.Find("Canvas").GetComponent<ObjectivesManager>();
        Subscribe();
    }

    private void TogglePausing()
    {
        if (!_canPause || _objectivesManager.areObjectivesVisible) return;
        if (GameIsPaused)
        {
            Resume();
        }
        else
        {
            Pause();
            pauseMenuUI.GetComponent<Animator>().enabled = true;
        }
    }

    public void Resume()
    {
        EventSystem.current.SetSelectedGameObject(null);
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        Unsubscribe();
        _playerInput.SwitchCurrentActionMap(_previousActionMap.name);
        
        _introUI.ShowUI();
        pauseMenuUI.SetActive(false);
        if (settingsMenu.activeInHierarchy) settingsMenu.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;

        GameManager.Instance.resumed = true;
    }

    private void Pause()
    {
        GameManager.Instance.isUiHidden = true;
        
        EventSystem.current.SetSelectedGameObject(firstSelected);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        _previousActionMap = _playerInput.currentActionMap;
        if (_playerInput != null)
        {
            _playerInput.SwitchCurrentActionMap("Paused");
            Subscribe();
        }
        
        _introUI.HideUI();
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        Unsubscribe();
        _playerInput.SwitchCurrentActionMap(_previousActionMap.name);
        
        _introUI.ShowUI();
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        
        SaveManager.Instance.SaveGameManager(); // Save progress (not location)
        GameManager.Instance.isLevel2 = false; // Reset but don't save
        
        SceneManager.LoadScene(menuName);
    }

    public void QuitGame()
    {
        EventSystem.current.SetSelectedGameObject(null);
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void OpenSettings()
    {
        EventSystem.current.SetSelectedGameObject(firstSettingsSelected);
    }

    public void CloseSettings()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }
    
    private void Subscribe()
    {
        var currentMap = _playerInput.currentActionMap;
        if (currentMap != null)
        {
            currentMap.FindAction("Pause").performed += _ => TogglePausing();
        }
    }

    private void Unsubscribe()
    {
        var currentMap = _playerInput.currentActionMap;
        if (currentMap != null)
        {
            currentMap.FindAction("Pause").performed -= _ => TogglePausing();
        }
    }
    
    private IEnumerator PauseCooldown() {
        _canPause = false;
        yield return new WaitForSeconds(0.1f); // Cooldown period
        _canPause = true;
    }
}
