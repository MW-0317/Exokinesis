using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class LoreManager : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject loreMenuUI;
    public string lore1Title;
    [Multiline] public string lore1Text;
    public string lore2Title;
    [Multiline] public string lore2Text;
    public string lore3Title;
    [Multiline] public string lore3Text;
    public string bossLoreTitle;
    [Multiline] public string bossLoreText;
    
    [SerializeField] private ObjectivesManager objectivesManager;
    [SerializeField] private GameObject uiElements;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;
    
    private PlayerInput _playerInput;
    private IntroUI _introUI;
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

    private void TogglePausing()
    {
        if (GameIsPaused)
        {
            Resume();
        }
    }

    public void Resume()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        Unsubscribe();
        _playerInput.SwitchCurrentActionMap(_previousActionMap.name);
        
        _introUI.ShowUI();
        loreMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        GameManager.Instance.resumed = true;

        StartCoroutine(AllXP());
    }

    public void Pause()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        _previousActionMap = _playerInput.currentActionMap;
        if (_playerInput != null)
        {
            _playerInput.SwitchCurrentActionMap("Lore");
            Subscribe();
        }
        
        _introUI.HideUI();
        loreMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void SetText(int loreNumber)
    {
        if (loreNumber == 1)
        {
            titleText.text = lore1Title;
            bodyText.text = lore1Text;
        }
        if (loreNumber == 2)
        {
            titleText.text = lore2Title;
            bodyText.text = lore2Text;
        }

        if (loreNumber == 3)
        {
            titleText.text = lore3Title;
            bodyText.text = lore3Text;
        }

        if (loreNumber == 4)
        {
            titleText.text = bossLoreTitle;
            bodyText.text = bossLoreText;
        }
    }
    
    private void Subscribe()
    {
        var currentMap = _playerInput.currentActionMap;
        if (currentMap != null)
        {
            currentMap.FindAction("Pause").performed += _ => TogglePausing();
            currentMap.FindAction("Interact").performed += _ => TogglePausing();
        }
    }

    private void Unsubscribe()
    {
        var currentMap = _playerInput.currentActionMap;
        if (currentMap != null)
        {
            currentMap.FindAction("Pause").performed -= _ => TogglePausing();
            currentMap.FindAction("Interact").performed -= _ => TogglePausing();
        }
    }

    private IEnumerator AllXP()
    {
        yield return new WaitForSeconds(0.5f);
        LevelManager.Instance.AddXP("Telekinesis", 20);
        LevelManager.Instance.AddXP("Telepathy", 20);
        LevelManager.Instance.AddXP("Psychokinesis", 20);
    }
}
