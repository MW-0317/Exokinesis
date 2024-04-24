using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables; // Namespace for Timeline and PlayableDirector

public class CutsceneController : MonoBehaviour
{
    public PlayableDirector playableDirector;
    public GameObject cutsceneGameObject;
    public GameObject skipPrompt;
    public GameObject blackBars;
    [SerializeField] private GameObject playerArmature;
    [SerializeField] private FadingController fadingController;
    [SerializeField] private bool firstCutscene;
    [SerializeField] private bool isSkippable;
    [SerializeField] private bool showBars;
    [SerializeField] private bool setPositionAfterSkip;
    [SerializeField] private Vector3 postSkipPosition;
    private PlayerInput _playerInput;
    private MusicManager _musicManager;
    private IntroUI _introUI;
    private bool _skipShown;
    private bool _canSkip;

    private void Awake()
    {
        if (!NewGame.Instance.isNewGame)
        {
            if (firstCutscene)
            {
                gameObject.SetActive(false);
            }
        }
        if (playerArmature != null)
        {
            _playerInput = playerArmature.GetComponent<PlayerInput>();
            _musicManager = playerArmature.GetComponent<MusicManager>();
            _introUI = playerArmature.GetComponent<IntroUI>();
        }
    }

    void Start()
    {
        if (playableDirector != null)
        {
            _introUI.HideUI();
            GameManager.Instance.CutsceneInputSwitcher(false);
            // Subscribe to the PlayableDirector's played and stopped events
            playableDirector.stopped += OnPlayableDirectorStopped;
        }

        if (skipPrompt != null)
        {
            skipPrompt.SetActive(false);
        }
        
        if (showBars) blackBars.SetActive(true);
    }

    private void Update()
    {
        if (_playerInput.actions["ShowSkip"].WasPressedThisFrame() && !_skipShown && isSkippable)
        {
            ShowSkipPrompt();
        }
        else if (_skipShown && _playerInput.actions["Skip"].WasPressedThisFrame() && isSkippable)
        {
            SkipCutscene();
        }
    }

    private void ShowSkipPrompt()
    {
        if (skipPrompt != null)
        {
            skipPrompt.SetActive(true);
            _skipShown = true;
            StartCoroutine(EnableSkip(0.25f));
        }
    }

    private void SkipCutscene()
    {
        if (_canSkip && playableDirector != null)
        {
            playableDirector.time = playableDirector.duration; // Advance to the end of the Timeline
            playableDirector.Evaluate(); // Update to new time
            playableDirector.Stop(); // Stop the timeline
            skipPrompt.SetActive(false);

            if (setPositionAfterSkip) StartCoroutine(SetPositionAfterFrame());
            
            if (firstCutscene)
            {
                _musicManager.introCutscene.enabled = false;
                _musicManager.StopAllCoroutines();
                _musicManager.FadeMusicIn();
            }
        }
    }

    private void OnPlayableDirectorStopped(PlayableDirector director)
    {
        if (director == playableDirector)
        {
            if (showBars) _introUI.FadeOutBars();
            
            skipPrompt.SetActive(false);
            cutsceneGameObject.SetActive(false);
            gameObject.SetActive(false);
            
            GameManager.Instance.CutsceneInputSwitcher(true);
            _introUI.ShowUI();
            GameManager.Instance.resumed = true;

            if (firstCutscene)
            {
                // Show intro UI prompts
                _introUI.TriggerIntroPrompts();
            }
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe
        if (playableDirector != null)
        {
            playableDirector.stopped -= OnPlayableDirectorStopped;
        }
    }

    IEnumerator EnableSkip(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        _canSkip = true;
    }
    
    IEnumerator SetPositionAfterFrame()
    {
        yield return null; // Wait for the next frame

        if (setPositionAfterSkip && playerArmature != null)
        {
            playerArmature.GetComponent<CharacterController>().enabled = false;
            playerArmature.transform.position = postSkipPosition;
            playerArmature.GetComponent<CharacterController>().enabled = true;
        }
    }
}