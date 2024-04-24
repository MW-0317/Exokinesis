using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class IntroUI : MonoBehaviour
{
    [SerializeField] private ObjectivesManager objectivesManager;
    [SerializeField] private PlayerInput playerInput;
    private InputActionMap _previousActionMap;
    // [SerializeField] private Button option1Button;
    // [SerializeField] private Button option2Button;
    
    public GameObject chapter1Prompt;
    public GameObject chapter2Prompt;
    public GameObject chapter3Prompt;
    
    public GameObject objective1Prompt;
    public GameObject objective2Prompt;
    public GameObject objective3Prompt;
    public GameObject objective4Prompt;
    public GameObject objective5Prompt;
    public GameObject objective6Prompt;
    
    public GameObject objective1Complete;
    public GameObject objective2Complete;
    public GameObject objective3Complete;
    public GameObject objective4Complete;
    public GameObject objective5Complete;
    public GameObject objective6Complete;

    public GameObject keycard1Obtained;
    public GameObject keycard2Obtained;
    public GameObject keycard3Obtained;
    public GameObject mapObtained;
    public GameObject hacking1Complete;
    
    public GameObject controlPrompts;
    public GameObject telekinesisControls;
    public GameObject telepathyHolder;
    public GameObject hackingMenu;

    public GameObject blackBars;

    private GameObject _inGameUi;
    
    public float chapterDelay = 1f;
    public float chapterDuration = 4f;
    public float objective1Delay = 1f;
    public float objectiveDuration = 4f;
    public float controlsDelay = 3f;
    public float controlsDuration = 30f;

    private Animator _playerAnimator;
    
    private Animator _chapter1Animator;
    private Animator _chapter2Animator;
    private Animator _chapter3Animator;
    
    private Animator _objective1Animator;
    private Animator _objective2Animator;
    private Animator _objective3Animator;
    private Animator _objective4Animator;
    private Animator _objective5Animator;
    private Animator _objective6Animator;
    
    private Animator _obj1DoneAnimator;
    private Animator _obj2DoneAnimator;
    private Animator _obj3DoneAnimator;
    private Animator _obj4DoneAnimator;
    private Animator _obj5DoneAnimator;
    private Animator _obj6DoneAnimator;

    private Animator _keycard1Animator;
    private Animator _keycard2Animator;
    private Animator _keycard3Animator;
    private Animator _mapAnimator;
    private Animator _hacking1Animator;
    
    private Animator _controlsAnimator;
    private Animator _telekinesisAnimator;
    private Animator _telepathyAnimator;

    private Animator _blackBarsAnimator;

    private bool _objective1Done;
    private bool _objective2Done;
    private bool _objective3Done;
    private bool _objective4Done;
    private bool _objective5Done;
    private bool _objective6Done;

    private float _idleTime;
    private bool _controlsHidden = true;
    private bool _escapedCell = false;
    
    void Start()
    {
        _inGameUi = GameObject.Find("InGame");
        
        _playerAnimator = gameObject.GetComponent<Animator>();
        
        _chapter1Animator = chapter1Prompt.GetComponent<Animator>();
        _chapter2Animator = chapter2Prompt.GetComponent<Animator>();
        _chapter3Animator = chapter3Prompt.GetComponent<Animator>();
        
        _objective1Animator = objective1Prompt.GetComponent<Animator>();
        _objective2Animator = objective2Prompt.GetComponent<Animator>();
        _objective3Animator = objective3Prompt.GetComponent<Animator>();
        _objective4Animator = objective3Prompt.GetComponent<Animator>();
        _objective5Animator = objective5Prompt.GetComponent<Animator>();
        _objective6Animator = objective6Prompt.GetComponent<Animator>();
        
        _obj1DoneAnimator = objective1Complete.GetComponent<Animator>();
        _obj2DoneAnimator = objective2Complete.GetComponent<Animator>();
        _obj3DoneAnimator = objective3Complete.GetComponent<Animator>();
        _obj4DoneAnimator = objective3Complete.GetComponent<Animator>();
        _obj5DoneAnimator = objective5Complete.GetComponent<Animator>();
        _obj6DoneAnimator = objective6Complete.GetComponent<Animator>();

        _keycard1Animator = keycard1Obtained.GetComponent<Animator>();
        _keycard2Animator = keycard2Obtained.GetComponent<Animator>();
        _keycard3Animator = keycard3Obtained.GetComponent<Animator>();
        _mapAnimator = mapObtained.GetComponent<Animator>();
        _hacking1Animator = hacking1Complete.GetComponent<Animator>();
        
        _controlsAnimator = controlPrompts.GetComponent<Animator>();
        _telekinesisAnimator = telekinesisControls.GetComponent<Animator>();
        _telepathyAnimator = telepathyHolder.GetComponent<Animator>();

        _blackBarsAnimator = blackBars.GetComponent<Animator>();

        if (!NewGame.Instance.isNewGame) _escapedCell = true;

        if (GameManager.Instance.isLevel2)
        {
            // ShowChapter2();
            // ShowLvl2Obj1();
            _escapedCell = true;
        }
    }

    private void Update()
    {
        bool isIdle = _playerAnimator.GetFloat("Speed") < 0.01f;
        if (isIdle && _escapedCell)
        {
            _idleTime += Time.deltaTime;

            if (_idleTime > 0.25f && _controlsHidden) // Show controls after idle for > 0.25 seconds
            {
                _controlsHidden = false;
                controlPrompts.SetActive(true);
            }
        }
        else
        {
            _idleTime = 0;
            if (_controlsAnimator != null && !_controlsHidden && _escapedCell) // Hide controls if moving
            {
                _controlsHidden = true;
                StartCoroutine(HideControls());
            }
        }
    }

    public void HideUI()
    {
        GameManager.Instance.isUiHidden = true;
        StopAllCoroutines();
        blackBars.SetActive(false);
        
        chapter1Prompt.SetActive(false);
        chapter2Prompt.SetActive(false);
        chapter3Prompt.SetActive(false);
        objective1Prompt.SetActive(false);
        objective2Prompt.SetActive(false);
        objective3Prompt.SetActive(false);
        objective4Prompt.SetActive(false);
        objective5Prompt.SetActive(false);
        objective6Prompt.SetActive(false);
    
        objective1Complete.SetActive(false);
        objective2Complete.SetActive(false);
        objective3Complete.SetActive(false);
        objective4Complete.SetActive(false);
        objective5Complete.SetActive(false);
        objective6Complete.SetActive(false);
    
        keycard1Obtained.SetActive(false);
        keycard2Obtained.SetActive(false);
        keycard3Obtained.SetActive(false);
        mapObtained.SetActive(false);
        hacking1Complete.SetActive(false);
    
        controlPrompts.SetActive(false);
        telekinesisControls.SetActive(false);
        
        if (_inGameUi != null) _inGameUi.SetActive(false);
    }

    public void ShowUI()
    {
        if (_inGameUi != null) _inGameUi.SetActive(true);
        GameManager.Instance.isUiHidden = false;
    }

    public void TriggerIntroPrompts()
    {
        StartCoroutine(ShowIntroPrompts());
    }

    public void ShowChapter2()
    {
        StartCoroutine(Chapter2());
    }

    public void ShowChapter3()
    {
        StartCoroutine(Chapter3());
    }

    public void ShowLvl2Obj1()
    {
        StartCoroutine(Lvl2Objective1());
    }

    public void Objective1Complete()
    {
        StartCoroutine(Objective1Done());
        objectivesManager.CompleteObjective(0);
    }

    public void Objective2Complete()
    {
        StartCoroutine(Objective2Done());
        objectivesManager.CompleteObjective(1);
    }
    
    public void Objective3Complete()
    {
        StartCoroutine(Objective3Done());
        objectivesManager.CompleteObjective(2);
    }

    public void Objective4Complete()
    {
        StartCoroutine(Objective4Done());
        objectivesManager.CompleteObjective(3);
    }

    public void Objective4Complete2()
    {
        StartCoroutine(Objective4Done2());
        objectivesManager.CompleteObjective(3);
    }
    
    public void Objective5Complete()
    {
        StartCoroutine(Objective5Done());
        objectivesManager.CompleteObjective(4);
    }
    
    public void Objective6Complete()
    {
        StartCoroutine(Objective6Done());
        objectivesManager.CompleteObjective(5);
    }

    public void Keycard1Obtained()
    {
        StartCoroutine(Keycard1());
    }

    public void Keycard2Obtained()
    {
        StartCoroutine(Keycard2());
    }

    public void Keycard3Obtained()
    {
        StartCoroutine(Keycard3());
    }

    public void MapObtained()
    {
        StartCoroutine(Map());
    }

    public void Hacking1Complete()
    {
        StartCoroutine(Hacking1());
    }

    // public void TriggerControls()
    // {
    //     StartCoroutine(ShowControls());
    // }

    public void TriggerIntroControls()
    {
        _idleTime = 0;

        StartCoroutine(IntroControls());
    }

    public void OpenTelekinesisControls()
    {
        telekinesisControls.SetActive(true);
        _telekinesisAnimator.Play("TelekinesisControlsOpen");
    }
    
    public void CloseTelekinesisControls()
    {
        StartCoroutine(CloseTelekinesis());
    }

    public void ShowHackingMenu()
    {
        hackingMenu.SetActive(true);
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        _previousActionMap = playerInput.currentActionMap;
        if (playerInput != null)
        {
            playerInput.SwitchCurrentActionMap("Paused");
        }
    }

    public void CloseHackingMenu()
    {
        hackingMenu.SetActive(false);
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        playerInput.SwitchCurrentActionMap(_previousActionMap.name);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void FadeOutBars()
    {
        StartCoroutine(CloseBlackBars());
    }

    // public void QuestionAppearing()
    // {
    //     StartCoroutine(DialogueQuestionAppearing());
    // }
    //
    // public void DialogueAppearing()
    // {
    //     StartCoroutine(EnableDialogue());
    // }
    //
    // public void DialogueDisappearing()
    // {
    //     StartCoroutine(DisableDialogue());
    // }
    //
    // public void DisableButtons()
    // {
    //     StartCoroutine(ButtonsDisappearing("Question Disappearing"));
    // }
    
    IEnumerator ShowIntroPrompts()
    {
        // Chapter 1 Prompt
        yield return new WaitForSeconds(chapterDelay);
        
        chapter1Prompt.SetActive(true);

        yield return new WaitForSeconds(chapterDuration);
        
        _chapter1Animator.Play("Chapter1Out");
        
        // Wait for the animation to finish
        AnimatorStateInfo chapterStateInfo = _chapter1Animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(chapterStateInfo.length);
        
        chapter1Prompt.SetActive(false);
        
        // Objective 1 Prompt
        if (!_objective1Done)
        {
            yield return new WaitForSeconds(objective1Delay);
        
            objective1Prompt.SetActive(true);

            yield return new WaitForSeconds(objectiveDuration);
        
            _objective1Animator.Play("Objective1Out");
        
            AnimatorStateInfo objective1StateInfo = _objective1Animator.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(objective1StateInfo.length);
        
            objective1Prompt.SetActive(false);
        }
    }

    IEnumerator Chapter2()
    {
        // Chapter 2 Prompt
        yield return new WaitForSeconds(chapterDelay);
        
        chapter2Prompt.SetActive(true);

        yield return new WaitForSeconds(chapterDuration);
        
        _chapter2Animator.Play("Chapter1Out");
        
        // Wait for the animation to finish
        AnimatorStateInfo chapterStateInfo = _chapter2Animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(chapterStateInfo.length);
        
        chapter2Prompt.SetActive(false);
    }
    
    IEnumerator Chapter3()
    {
        // Chapter 3 Prompt
        yield return new WaitForSeconds(chapterDelay);
        
        chapter3Prompt.SetActive(true);

        yield return new WaitForSeconds(chapterDuration);
        
        _chapter3Animator.Play("Chapter1Out");
        
        // Wait for the animation to finish
        AnimatorStateInfo chapterStateInfo = _chapter3Animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(chapterStateInfo.length);
        
        chapter3Prompt.SetActive(false);
    }

    IEnumerator Lvl2Objective1()
    {
        yield return new WaitForSeconds(8);
        objective1Prompt.SetActive(true);

        yield return new WaitForSeconds(objectiveDuration);

        _objective1Animator.Play("Objective1Out");

        AnimatorStateInfo stateInfo = _objective1Animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);

        objective1Prompt.SetActive(false);
    }
    
    IEnumerator Objective1Done()
    {
        yield return new WaitForSeconds(0.5f);
        
        objective1Complete.SetActive(true);
        
        _obj1DoneAnimator.Play("CheckIn");
        
        yield return new WaitForSeconds(objectiveDuration);
        
        _obj1DoneAnimator.Play("CheckOut");
        
        AnimatorStateInfo stateInfo1 = _obj1DoneAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo1.length);
        
        objective1Complete.SetActive(false);
        _objective1Done = true;
        
        yield return new WaitForSeconds(1f);
        if (!_objective2Done)
        {
            objective2Prompt.SetActive(true);

            yield return new WaitForSeconds(objectiveDuration);
        
            _objective2Animator.Play("Objective2Out");
        
            AnimatorStateInfo stateInfo2 = _objective2Animator.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(stateInfo2.length);
        
            objective2Prompt.SetActive(false);
        }
    }
    
    IEnumerator Objective2Done()
    {
        // Mark objective as complete
        yield return new WaitForSeconds(0.5f);
        
        objective2Complete.SetActive(true);
        
        _obj2DoneAnimator.Play("CheckIn2");
        
        yield return new WaitForSeconds(objectiveDuration);
        
        _obj2DoneAnimator.Play("CheckOut2");
        
        AnimatorStateInfo stateInfo1 = _obj2DoneAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo1.length);
        
        objective2Complete.SetActive(false);
        _objective2Done = true;
        
        // Show next objective
        yield return new WaitForSeconds(1);
        if (!_objective3Done)
        {
            objective3Prompt.SetActive(true);

            yield return new WaitForSeconds(objectiveDuration);
        
            _objective3Animator.Play("Objective3Out");
        
            AnimatorStateInfo stateInfo2 = _objective3Animator.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(stateInfo2.length);
        
            objective3Prompt.SetActive(false);
        }
    }
    
    IEnumerator Objective3Done()
    {
        // Mark objective as complete
        yield return new WaitForSeconds(0.5f);
        
        objective3Complete.SetActive(true);
        
        _obj3DoneAnimator.Play("CheckIn3");
        
        yield return new WaitForSeconds(objectiveDuration);
        
        if (_obj3DoneAnimator != null) _obj3DoneAnimator.Play("CheckOut3");
        
        AnimatorStateInfo stateInfo1 = _obj3DoneAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo1.length);
        
        objective3Complete.SetActive(false);
        _objective3Done = true;
        
        // Show next objective (when needed)
        yield return new WaitForSeconds(1);
        if (!_objective4Done && GameManager.Instance.isLevel2)
        {
            objective4Prompt.SetActive(true);

            yield return new WaitForSeconds(objectiveDuration);
        
            _objective4Animator.Play("Objective4Out");
        
            AnimatorStateInfo stateInfo2 = _objective4Animator.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(stateInfo2.length);
        
            objective4Prompt.SetActive(false);
        }
    }

    IEnumerator ShowObjective4()
    {
         yield return new WaitForSeconds(1);
         if (!_objective4Done)
         {
             objective4Prompt.SetActive(true);

             yield return new WaitForSeconds(objectiveDuration);

             _objective4Animator.Play("Objective4Out");

             AnimatorStateInfo stateInfo = _objective4Animator.GetCurrentAnimatorStateInfo(0);
             yield return new WaitForSeconds(stateInfo.length);

             objective4Prompt.SetActive(false);
         }
    }

    IEnumerator Objective4Done()
    {
        // Mark objective as complete
        yield return new WaitForSeconds(0.5f);
        
        objective4Complete.SetActive(true);
        
        _obj4DoneAnimator.Play("CheckIn4");
        
        yield return new WaitForSeconds(objectiveDuration);
        
        if (_obj4DoneAnimator != null) _obj4DoneAnimator.Play("CheckOut4");
        
        AnimatorStateInfo stateInfo = _obj4DoneAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);
        
        objective4Complete.SetActive(false);
        _objective4Done = true;
    }
    
    IEnumerator Objective4Done2()
    {
        // Mark objective as complete
        yield return new WaitForSeconds(0.5f);
        
        objective4Complete.SetActive(true);
        
        _obj4DoneAnimator.Play("CheckIn4");
        
        yield return new WaitForSeconds(objectiveDuration);
        
        if (_obj4DoneAnimator != null) _obj4DoneAnimator.Play("CheckOut4");
        
        AnimatorStateInfo stateInfo1 = _obj4DoneAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo1.length);
        
        objective4Complete.SetActive(false);
        _objective4Done = true;
        
        // Show next objective (when needed)
        yield return new WaitForSeconds(1);
        if (!_objective5Done)
        {
            objective5Prompt.SetActive(true);

            yield return new WaitForSeconds(objectiveDuration);
        
            _objective5Animator.Play("Objective5Out");
        
            AnimatorStateInfo stateInfo2 = _objective5Animator.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(stateInfo2.length);
        
            objective5Prompt.SetActive(false);
        }
    }
    
    IEnumerator Objective5Done()
    {
        // Mark objective as complete
        yield return new WaitForSeconds(0.5f);
        
        objective5Complete.SetActive(true);
        
        _obj5DoneAnimator.Play("CheckIn5");
        
        yield return new WaitForSeconds(objectiveDuration);
        
        if (_obj5DoneAnimator != null) _obj5DoneAnimator.Play("CheckOut5");
        
        AnimatorStateInfo stateInfo1 = _obj5DoneAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo1.length);
        
        objective5Complete.SetActive(false);
        _objective5Done = true;
        
        // Show next objective (when needed)
        yield return new WaitForSeconds(1);
        if (!_objective6Done)
        {
            objective6Prompt.SetActive(true);

            yield return new WaitForSeconds(objectiveDuration);
        
            _objective6Animator.Play("Objective6Out");
        
            AnimatorStateInfo stateInfo2 = _objective6Animator.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(stateInfo2.length);
        
            objective6Prompt.SetActive(false);
        }
    }

    IEnumerator Objective6Done()
    {
        // Mark objective as complete
        yield return new WaitForSeconds(0.5f);
        
        objective6Complete.SetActive(true);
        
        _obj6DoneAnimator.Play("CheckIn6");
        
        yield return new WaitForSeconds(objectiveDuration);
        
        if (_obj6DoneAnimator != null) _obj6DoneAnimator.Play("CheckOut6");
        
        AnimatorStateInfo stateInfo = _obj6DoneAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);
        
        objective6Complete.SetActive(false);
        _objective6Done = true;
    }

    IEnumerator Keycard1()
    {
        yield return new WaitForSeconds(0.5f);
        
        keycard1Obtained.SetActive(true);
        
        _keycard1Animator.Play("CheckIn");
        
        yield return new WaitForSeconds(objectiveDuration);
        
        if (keycard1Obtained != null) _keycard1Animator.Play("CheckOut");
        
        AnimatorStateInfo stateInfo = _keycard1Animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);
        
        keycard1Obtained.SetActive(false);
    }
    
    IEnumerator Keycard2()
    {
        yield return new WaitForSeconds(0.5f);
        
        keycard2Obtained.SetActive(true);
        
        _keycard2Animator.Play("CheckIn");
        
        yield return new WaitForSeconds(objectiveDuration);
        
        if (keycard2Obtained != null) _keycard2Animator.Play("CheckOut");
        
        AnimatorStateInfo stateInfo = _keycard2Animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);
        
        keycard2Obtained.SetActive(false);
    }
    
    IEnumerator Keycard3()
    {
        yield return new WaitForSeconds(0.5f);
        
        keycard3Obtained.SetActive(true);
        
        _keycard3Animator.Play("CheckIn");
        
        yield return new WaitForSeconds(objectiveDuration);
        
        if (keycard3Obtained != null) _keycard3Animator.Play("CheckOut");
        
        AnimatorStateInfo stateInfo = _keycard3Animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);
        
        keycard3Obtained.SetActive(false);
    }

    IEnumerator Map()
    {
        yield return new WaitForSeconds(0.5f);
        
        mapObtained.SetActive(true);
        
        _mapAnimator.Play("CheckIn");
        
        yield return new WaitForSeconds(objectiveDuration);
        
        if (mapObtained != null) _mapAnimator.Play("CheckOut");
        
        AnimatorStateInfo stateInfo = _mapAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);
        
        mapObtained.SetActive(false);
    }

    IEnumerator Hacking1()
    {
        yield return new WaitForSeconds(0.5f);
        
        hacking1Complete.SetActive(true);
        
        _hacking1Animator.Play("Hacking1In");
        
        yield return new WaitForSeconds(objectiveDuration);
        
        if (hacking1Complete != null) _hacking1Animator.Play("Hacking1Out");
        
        AnimatorStateInfo stateInfo = _hacking1Animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);
        
        hacking1Complete.SetActive(false);
    }
    
    IEnumerator ShowControls()
    {
        yield return new WaitForSeconds(controlsDelay);
        
        controlPrompts.SetActive(true);
    }

    IEnumerator HideControls()
    {
        _controlsAnimator.Play("ControlsClose");
        
        AnimatorStateInfo stateInfo = _controlsAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);
        
        controlPrompts.SetActive(false);
    }

    IEnumerator IntroControls()
    {
        if (_controlsAnimator != null && !_controlsHidden)
        {
            _controlsHidden = true;
            StartCoroutine(HideControls());
        }
        
        yield return new WaitForSeconds(controlsDelay);
        
        controlPrompts.SetActive(true);

        yield return new WaitForSeconds(controlsDuration);

        StartCoroutine(HideControls());

        _escapedCell = true;
    }

    IEnumerator CloseTelekinesis()
    {
        _telekinesisAnimator.Play("TelekinesisControlsClose");
        
        AnimatorStateInfo stateInfo = _telekinesisAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);
        
        telekinesisControls.SetActive(false);
    }

    IEnumerator CloseBlackBars()
    {
        _blackBarsAnimator.Play("BlackBarsOut");
        
        AnimatorStateInfo stateInfo = _blackBarsAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);
        
        blackBars.SetActive(false);
    }

    // IEnumerator DialogueQuestionAppearing()
    // {
    //     _telepathyAnimator.Play("Question Appearing");
    //     
    //     AnimatorStateInfo stateInfo = _telepathyAnimator.GetCurrentAnimatorStateInfo(0);
    //     yield return new WaitForSeconds(stateInfo.length);
    //     
    //     _telepathyAnimator.Play("Question Idle");
    // }
    //
    // private IEnumerator EnableDialogue()
    // {
    //     telepathyHolder.SetActive(true);
    //     
    //     _telepathyAnimator.Play("Dialogue Appearing");
    //     
    //     AnimatorStateInfo stateInfo = _telepathyAnimator.GetCurrentAnimatorStateInfo(0);
    //     yield return new WaitForSeconds(stateInfo.length);
    //
    //     _telepathyAnimator.Play("Dialogue Idle");
    // }
    //
    // private IEnumerator DisableDialogue()
    // {
    //     _telepathyAnimator.Play("Dialogue Disappearing");
    //     
    //     AnimatorStateInfo stateInfo = _telepathyAnimator.GetCurrentAnimatorStateInfo(0);
    //     yield return new WaitForSeconds(stateInfo.length);
    //     
    //     telepathyHolder.SetActive(false);
    // }
    //
    // private IEnumerator ButtonsDisappearing(string animationName)
    // {
    //     _telepathyAnimator.Play(animationName);
    //     yield return new WaitUntil(() => 
    //         _telepathyAnimator.GetCurrentAnimatorStateInfo(0).IsName(animationName) &&
    //         !_telepathyAnimator.IsInTransition(0));
    //
    //     float waitTime = _telepathyAnimator.GetCurrentAnimatorStateInfo(0).length;
    //     waitTime += _telepathyAnimator.GetAnimatorTransitionInfo(0).duration;
    //
    //     yield return new WaitForSeconds(waitTime);
    //     
    //     option1Button.gameObject.SetActive(false);
    //     option2Button.gameObject.SetActive(false);
    // }
}
