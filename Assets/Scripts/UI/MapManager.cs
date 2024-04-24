using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapManager : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject mapPrompt;
    
    [SerializeField] private ObjectivesManager objectivesManager;
    [SerializeField] private GameObject uiElements;
    
    private PlayerInput _playerInput;
    private IntroUI _introUI;
    private bool _canPause = true;
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

    private void OnEnable()
    {
        Subscribe();
    }

    private void Update()
    {
        if (GameManager.Instance.hasMap)
        {
            mapPrompt.SetActive(true);
        }
    }

    private void TogglePausing()
    {
        if (!_canPause || objectivesManager.areObjectivesVisible || !GameManager.Instance.hasMap) return;
        
        if (GameIsPaused)
        {
            Resume();
        }
        else
        {
            Pause();
            Animator animator = pauseMenuUI.GetComponent<Animator>();
            if (animator != null) animator.GetComponent<Animator>().enabled = true;
        }
    }

    public void Resume()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        Unsubscribe();
        _playerInput.SwitchCurrentActionMap(_previousActionMap.name);
        
        _introUI.ShowUI();
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        GameManager.Instance.resumed = true;
    }

    void Pause()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        _previousActionMap = _playerInput.currentActionMap;
        if (_playerInput != null)
        {
            _playerInput.SwitchCurrentActionMap("Map");
            Subscribe();
        }
        
        _introUI.HideUI();
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
    
    private void Subscribe()
    {
        var currentMap = _playerInput.currentActionMap;
        if (currentMap != null)
        {
            currentMap.FindAction("Map").performed += _ => TogglePausing();
        }
    }

    private void Unsubscribe()
    {
        var currentMap = _playerInput.currentActionMap;
        if (currentMap != null)
        {
            currentMap.FindAction("Map").performed -= _ => TogglePausing();
        }
    }
    
    private IEnumerator PauseCooldown() {
        _canPause = false;
        yield return new WaitForSeconds(0.1f); // Cooldown period
        _canPause = true;
    }
}
