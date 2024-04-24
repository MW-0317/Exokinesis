using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadingController : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    private Canvas _canvas;
    [SerializeField] private float initialFadeDuration;
    [SerializeField] private bool fadeIn = false;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        _canvas.enabled = true;
        
        if (fadeIn)
        {
            FadeIn(initialFadeDuration);
        }
        else
        {
            FadeOut(initialFadeDuration);
        }
    }

    public void FadeIn(float fadeDuration)
    {
        StartCoroutine(FadeCanvasGroup(_canvasGroup, _canvasGroup.alpha, 0, fadeDuration, true));
    }
    
    public void FadeOut(float fadeDuration)
    {
        StartCoroutine(FadeCanvasGroup(_canvasGroup, _canvasGroup.alpha, 1, fadeDuration));
    }
    
    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration, bool isFadingIn = false)
    {
        if (isFadingIn)
        {
            cg.alpha = start;
            yield return new WaitForSeconds(1); // Hold for 1 second at full opacity
            duration -= 1; // Adjust duration to account for the delay
        }
        
        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / duration;
            if (fadeIn)
            {
                // Use a non-linear interpolation function
                t = Mathf.SmoothStep(0.0f, 1.0f, t);
            }
            cg.alpha = Mathf.Lerp(start, end, t);
            yield return null;
        }

        cg.alpha = end;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FadeIn(initialFadeDuration);
    }
    
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

