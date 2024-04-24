
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using CI.QuickSave;
using StarterAssets;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool firstLevel2 = false;
    public bool isLevel2 = false;
    public bool guardChasing;
    public bool hasKeycard1 = false;
    public bool hasKeycard2 = false;
    public bool hasKeycard3 = false;
    public bool hasMap = false;
    public bool storageCutscenePlayed = false;
    public bool level2Loaded = false;
    public bool altEnding = false;
    public bool isHacking = false;
    public bool puzzleCompleted = false;
    public bool isUiHidden = false;
    public bool resumed = false;
    public float telekinesisSkillPoints = 0f;
    public float telepathySkillPoints = 0f;
    public float psychokinesisSkillPoints = 0f;
    public float maxTelekinesisWeight = 10f;
    public float currentTelekinesisWeight;
    public float slowMotionDuration = 4f;

    public Dictionary<string, object> GetAllSettings()
    {
        return new Dictionary<string, object>
        {
            { "firstLevel2", firstLevel2 },
            { "isLevel2", isLevel2 },
            { "guardChasing", guardChasing },
            { "hasKeycard1", hasKeycard1 },
            { "hasKeycard2", hasKeycard2 },
            { "hasKeycard3", hasKeycard3 },
            { "hasMap", hasMap },
            { "storageCutscenePlayed", storageCutscenePlayed },
            { "level2Loaded", level2Loaded },
            { "altEnding", altEnding },
            { "isHacking", isHacking },
            { "puzzleCompleted", puzzleCompleted },
            { "isUiHidden", isUiHidden },
            { "telekinesisSkillPoints", telekinesisSkillPoints },
            { "telepathySkillPoints", telepathySkillPoints },
            { "psychokinesisSkillPoints", psychokinesisSkillPoints },
            { "maxTelekinesisWeight", maxTelekinesisWeight },
            { "currentTelekinesisWeight", currentTelekinesisWeight },
            { "slowMotionDuration", slowMotionDuration }
        };
    }

    [SerializeField] private GameObject uiElements;
    [SerializeField] private GameObject fadingCanvas;
    [SerializeField] private FadingController fadingController;
    [SerializeField] private PlayerInput playerInput;
    private ThirdPersonController _playerController;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (NewGame.Instance.isNewGame && !isLevel2)
        {
            ResetGameManager();
            SaveManager.Instance.SaveGameManager();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindUIElements();
        _playerController = FindObjectOfType<ThirdPersonController>();
        if (playerInput == null) playerInput = FindObjectOfType<PlayerInput>();
        EnableGameplayControls(playerInput != null);
    }

    private void FindUIElements()
    {
        uiElements = GameObject.FindWithTag("UIElements");
        fadingCanvas = GameObject.FindWithTag("FadingCanvas");
        fadingController = fadingCanvas != null ? fadingCanvas.GetComponent<FadingController>() : null;
    }

    public void ResetGameManager()
    {
        firstLevel2 = true;
        isLevel2 = false;
        guardChasing = false;
        hasKeycard1 = false;
        hasKeycard2 = false;
        hasKeycard3 = false;
        hasMap = false;
        storageCutscenePlayed = false;
        level2Loaded = false;
        altEnding = false;
        isHacking = false;
        puzzleCompleted = false;
        isUiHidden = false;
        telekinesisSkillPoints = 0;
        telepathySkillPoints = 0;
        psychokinesisSkillPoints = 0;
        maxTelekinesisWeight = 10;
        currentTelekinesisWeight = 0;
        slowMotionDuration = 2;
    }

    public void GameOverReset()
    {
        firstLevel2 = true;
        isLevel2 = false;
        guardChasing = false;
        hasKeycard1 = false;
        hasKeycard2 = false;
        hasKeycard3 = false;
        hasMap = false;
        storageCutscenePlayed = false;
        level2Loaded = false;
        altEnding = false;
        isHacking = false;
        puzzleCompleted = false;
        isUiHidden = false;
    }
    
    public void SetAllSettings(Dictionary<string, object> settings)
    {
        if (settings == null) return;

        // Use TryGetValue to safely obtain values from the dictionary
        object value;

        if (settings.TryGetValue("firstLevel2", out value))
            firstLevel2 = Convert.ToBoolean(value);
        
        if (settings.TryGetValue("isLevel2", out value))
            isLevel2 = Convert.ToBoolean(value);
        
        if (settings.TryGetValue("guardChasing", out value))
            guardChasing = Convert.ToBoolean(value);
        
        if (settings.TryGetValue("hasKeycard1", out value))
            hasKeycard1 = Convert.ToBoolean(value);
        
        if (settings.TryGetValue("hasKeycard2", out value))
            hasKeycard2 = Convert.ToBoolean(value);
        
        if (settings.TryGetValue("hasKeycard3", out value))
            hasKeycard3 = Convert.ToBoolean(value);
        
        if (settings.TryGetValue("hasMap", out value))
            hasMap = Convert.ToBoolean(value);
        
        if (settings.TryGetValue("storageCutscenePlayed", out value))
            storageCutscenePlayed = Convert.ToBoolean(value);
        
        if (settings.TryGetValue("level2Loaded", out value))
            level2Loaded = Convert.ToBoolean(value);
        
        if (settings.TryGetValue("altEnding", out value))
            altEnding = Convert.ToBoolean(value);
        
        if (settings.TryGetValue("isHacking", out value))
            isHacking = Convert.ToBoolean(value);
        
        if (settings.TryGetValue("puzzleCompleted", out value))
            puzzleCompleted = Convert.ToBoolean(value);
        
        if (settings.TryGetValue("isUiHidden", out value))
            isUiHidden = Convert.ToBoolean(value);
        
        if (settings.TryGetValue("telekinesisSkillPoints", out value))
            telekinesisSkillPoints = Convert.ToSingle(value);
        
        if (settings.TryGetValue("telepathySkillPoints", out value))
            telepathySkillPoints = Convert.ToSingle(value);
        
        if (settings.TryGetValue("psychokinesisSkillPoints", out value))
            psychokinesisSkillPoints = Convert.ToSingle(value);
        
        if (settings.TryGetValue("maxTelekinesisWeight", out value))
            maxTelekinesisWeight = Convert.ToSingle(value);
        
        if (settings.TryGetValue("currentTelekinesisWeight", out value))
            currentTelekinesisWeight = Convert.ToSingle(value);
        
        if (settings.TryGetValue("slowMotionDuration", out value))
            slowMotionDuration = Convert.ToSingle(value);
    }

    
    public void EnableGameplayControls(bool enable)
    {
        // Enable/disable player input
        if (enable)
        {
            if (playerInput != null) playerInput.SwitchCurrentActionMap("Player");
        }
        else
        {
            if (playerInput != null) playerInput.SwitchCurrentActionMap("NoInput");
        }

        // Enable/disable UI elements
        if (uiElements != null)
        {
            uiElements.SetActive(enable);
        }
    }
    
    public void CutsceneInputSwitcher(bool enable)
    {
        // Enable/disable player input
        if (enable)
        {
            playerInput.SwitchCurrentActionMap("Player");
            _playerController.Sprinting();
            _playerController.sprintDisabled = false;
        }
        else
        {
            _playerController.sprintDisabled = true;
            _playerController.wasSprinting = false;
            playerInput.SwitchCurrentActionMap("Cutscene");
        }

        // Enable/disable UI elements
        if (uiElements != null)
        {
            uiElements.SetActive(enable);
        }
    }
    
    public void AddSkillPoint(string abilityName)
    {
        if (abilityName == "Telekinesis")
        {
            telekinesisSkillPoints += 1;
            SaveSkillPoints();
        }

        if (abilityName == "Telepathy")
        {
            telepathySkillPoints += 1;
            SaveSkillPoints();
        }

        if (abilityName == "Psychokinesis")
        {
            psychokinesisSkillPoints += 1;
            SaveSkillPoints();
        }
    }
    
    public void SaveSkillPoints()
    {
        var writer = QuickSaveWriter.Create("Skills");
        
        writer.Write("telekinesisSkillPoints", telekinesisSkillPoints);
        writer.Write("telepathySkillPoints", telepathySkillPoints);
        writer.Write("psychokinesisSkillPoints", psychokinesisSkillPoints);
            
        writer.Commit();
        Debug.Log($"Saved {telekinesisSkillPoints}, {telepathySkillPoints}, {psychokinesisSkillPoints}");
    }
    
    public void LoadSkillPoints()
    {
        var reader = QuickSaveReader.Create("Skills");

        if (!NewGame.Instance.isNewGame)
        {
            telekinesisSkillPoints = reader.Read<float>("telekinesisSkillPoints");
            telepathySkillPoints = reader.Read<float>("telepathySkillPoints");
            psychokinesisSkillPoints = reader.Read<float>("psychokinesisSkillPoints");
            
            Debug.Log($"Loaded, {telekinesisSkillPoints}, {telepathySkillPoints}, {psychokinesisSkillPoints}");
        }
        
        if (NewGame.Instance.isNewGame)
        {
            telekinesisSkillPoints = 0;
            telepathySkillPoints = 0;
            psychokinesisSkillPoints = 0;
        }
    }

    public void PrepareForRespawn()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        LevelManager.Instance.telekinesis.currentXP = 0;
        LevelManager.Instance.telepathy.currentXP = 0;
        LevelManager.Instance.psychokinesis.currentXP = 0;
        // Additional reset logic
    }
}
