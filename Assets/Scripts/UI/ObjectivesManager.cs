using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;
using CI.QuickSave;
using UnityEngine.EventSystems;

public class ObjectivesManager : MonoBehaviour
{
    public List<Objective> objectives = new List<Objective>();
    public GameObject objectivePrefab;
    public Transform objectivesParent;
    public TextMeshProUGUI objectiveTitle;
    public TextMeshProUGUI objectiveDescription;
    
    [SerializeField] private GameObject firstSelected;
    public bool areObjectivesVisible;

    private PlayerInput _playerInput;
    private IntroUI _introUI;
    private bool _isCompleting;
    private InputActionMap _previousActionMap;
    
    private void Awake()
    {
        _introUI = GameObject.Find("PlayerArmature").GetComponent<IntroUI>();
        _playerInput = GameObject.Find("PlayerArmature").GetComponent<PlayerInput>();
    }

    private void Start()
    {
        if (!NewGame.Instance.isNewGame) LoadObjectives();
        else
        {
            ResetObjectives();
            LoadObjectives();
            ShowObjective(0);
        }
        
        Subscribe();
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    private void OnEnable()
    {
        Subscribe();
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    private void ToggleObjectives()
    {
        if (areObjectivesVisible)
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
        if (areObjectivesVisible)
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
        _playerInput.SwitchCurrentActionMap(_previousActionMap.name);
        
        _introUI.ShowUI();
        objectivePrefab.SetActive(false);
        Time.timeScale = 1f;
        areObjectivesVisible = false;

        GameManager.Instance.resumed = true;
    }

    private void Pause()
    {
        EventSystem.current.SetSelectedGameObject(firstSelected);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        _previousActionMap = _playerInput.currentActionMap;
        _playerInput.SwitchCurrentActionMap("Objectives");
        Subscribe();
        
        _introUI.HideUI();
        objectivePrefab.SetActive(true);
        Time.timeScale = 0f;
        areObjectivesVisible = true;
    }
    
    public void SaveObjectives()
    {
        var writer = QuickSaveWriter.Create("Objectives");
        
        for (int i = 0; i < objectives.Count; i++)
        {
            writer.Write("objective_" + i + "_completed", objectives[i].isCompleted);
        }
        
        writer.Commit();
    }

    public void ResetObjectives()
    {
        var writer = QuickSaveWriter.Create("Objectives");
        
        for (int i = 0; i < objectives.Count; i++)
        {
            writer.Write("objective_" + i + "_completed", false);
        }
        
        writer.Commit();
    }
    
    public void LoadObjectives()
    {
        var reader = QuickSaveReader.Create("Objectives");

        if (!NewGame.Instance.isNewGame)
        {
            for (int i = 0; i < objectives.Count; i++)
            {
                bool completed = reader.Read<bool>("objective_" + i + "_completed");
                if (completed)
                {
                    objectives[i].isCompleted = true;
                    UpdateObjectiveUI(i, true);

                    if (i + 1 < objectives.Count)
                    {
                        UpdateObjectiveUI(i + 1, false);
                        ShowObjective(i + 1);
                    }
                    else
                    {
                        ShowObjective(i);
                    }
                }
                else
                {
                    objectives[i].isCompleted = false;
                }
            }
        }
        else
        {
            ShowObjective(0);
        }
    }

    public void CompleteObjective(int objectiveIndex)
    {
        if (_isCompleting) 
        {
            return;
        }
        
        if (objectiveIndex < objectives.Count)
        {
            _isCompleting = true;
            
            objectives[objectiveIndex].isCompleted = true;
            objectives[objectiveIndex].objectiveCompleteEvent?.Invoke();
            UpdateObjectiveUI(objectiveIndex, true);
            if (objectiveIndex + 1 < objectives.Count)
            {
                ShowObjective(objectiveIndex + 1);
            }

            _isCompleting = false;
        }
    }

    private void UpdateObjectiveUI(int objectiveIndex, bool completed)
    {
        var buttons = objectivesParent.GetComponentsInChildren<Button>(true); // (true) includes inactive
        if (objectiveIndex < buttons.Length)
        {
            var buttonText = buttons[objectiveIndex].GetComponentInChildren<TextMeshProUGUI>(true);
            buttonText.text = completed ? objectives[objectiveIndex].name + " (Done)" : objectives[objectiveIndex].name;
            buttons[objectiveIndex].gameObject.SetActive(true); // Ensure the current button is visible

            SaveObjectives();
        }
    }

    private void ShowObjective(int objectiveIndex)
    {
        if (objectiveIndex < objectives.Count)
        {
            Objective obj = objectives[objectiveIndex];
            objectiveTitle.text = obj.name;
            objectiveDescription.text = obj.description;
            
            UpdateObjectiveUI(objectiveIndex, obj.isCompleted);
            
            SaveObjectives();
        }
    }

    public void HideObjectives()
    {
        var buttons = objectivesParent.GetComponentsInChildren<Button>(true);

        // Loop through all buttons
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i != 0)
            {
                buttons[i].gameObject.SetActive(false);
            }
            else
            {
                buttons[i].gameObject.SetActive(true);
            }
        }
    }

    public void OnButtonClick(int objectiveIndex)
    {
        if (objectiveIndex >= 0 && objectiveIndex < objectives.Count)
        {
            ShowObjective(objectiveIndex);
        }
        else
        {
            Debug.Log("Objective not found");
        }
    }
    
    private void Subscribe()
    {
        var currentMap = _playerInput.currentActionMap;
        if (currentMap != null)
        {
            currentMap.FindAction("Objectives").performed += _ => ToggleObjectives();
            currentMap.FindAction("Pause").performed += _ => ToPauseMenu();
        }
    }

    private void Unsubscribe()
    {
        var currentMap = _playerInput.currentActionMap;
        if (currentMap != null)
        {
            currentMap.FindAction("Objectives").performed -= _ => ToggleObjectives();
            currentMap.FindAction("Pause").performed -= _ => ToPauseMenu();
        }
    }
}
