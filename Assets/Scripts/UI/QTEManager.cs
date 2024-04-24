using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class QTEManager : MonoBehaviour
{
    public GameObject qtePrompt;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Animator scientistAnimator;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject successCutscene;
    [SerializeField] private GameObject failureCutscene;
    private float _successDuration = 7.5f;
    private float _failureDuration = 6f;
    private bool _entered;
    
    void Start()
    {
        qtePrompt.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
        qtePrompt.SetActive(false);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_entered)
        {
            StartQte();
            _entered = true;
        }
    }
    
    private void StartQte()
    {
        Time.timeScale = 0;
        qtePrompt.SetActive(true);
        
        StartCoroutine(WaitForInput(2f));
    }

    private IEnumerator WaitForInput(float duration)
    {
        float timePassed = 0;

        while (timePassed < duration)
        {
            if (playerInput.actions["Jump"].IsPressed())
            {
                QteSuccess();
                yield break; // Exit the coroutine
            }

            // Since Time.timeScale is 0, use Time.unscaledDeltaTime
            timePassed += Time.unscaledDeltaTime;
            yield return null;
        }

        // Time ran out
        QteFail();
    }

    private void QteSuccess()
    {
        Time.timeScale = 1;
        qtePrompt.SetActive(false);
        scientistAnimator.SetBool("isDefeated", true);

        StartCoroutine(StartSuccess());
    }

    private void QteFail()
    {
        Time.timeScale = 1;
        qtePrompt.SetActive(false);

        StartCoroutine(StartFailure());
    }
    
    IEnumerator StartSuccess()
    {
        playerInput.SwitchCurrentActionMap("Cutscene");
        playerAnimator.SetBool("Crouch", false);
        successCutscene.SetActive(true);

        yield return new WaitForSeconds(_successDuration); // Duration of cutscene
        
        playerInput.SwitchCurrentActionMap("Player");
    }
    
    IEnumerator StartFailure()
    {
        playerInput.SwitchCurrentActionMap("Cutscene");
        playerAnimator.SetBool("Crouch", false);
        failureCutscene.SetActive(true);

        yield return new WaitForSeconds(_failureDuration); // Duration of cutscene
        
        playerInput.SwitchCurrentActionMap("Player");
        GameManager.Instance.PrepareForRespawn();
        NewGame.Instance.shouldReload = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }
}
