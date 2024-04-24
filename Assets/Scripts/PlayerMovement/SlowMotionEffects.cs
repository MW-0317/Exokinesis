using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SlowMotionEffects : MonoBehaviour
{
    [Header("Slow Motion Parameters")] 
    [SerializeField] private float slowDownFactor = 0.5f;
    [SerializeField] private float transitionDuration = 1f;
    
    [Header("Slow Motion Audio")] 
    [SerializeField] private AudioMixer musicMixer;
    [SerializeField] private AudioSource swooshSource;
    [SerializeField] private float musicSlowDownFactor;
    [SerializeField] private float sfxSlowDownFactor;

    [Header("Slow Motion Post Processing")] 
    private bool _showSaturation = false;
    private bool _showAberration = false;
    [SerializeField] private Volume postProcessingVolume;

    [Header("Slow Motion Input")] 
    [SerializeField] private PlayerInput playerInput;
    
    private ChromaticAberration _chromaticAberration;
    private ColorAdjustments _colorAdjustments;
    private bool _slowMotionActive;

    private void Start()
    {
        // Ensure everything is at normal speed
        Time.timeScale = 1f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        musicMixer.SetFloat("MusicPitch", 1f);
        musicMixer.SetFloat("SFXPitch", 1f);
        
        if (postProcessingVolume.profile.TryGet(out ChromaticAberration chromatic))
        {
            _chromaticAberration = chromatic;
        }

        if (postProcessingVolume.profile.TryGet(out ColorAdjustments color))
        {
            _colorAdjustments = color;
        }
    }

    private void Update()
    {
        float input = playerInput.actions["SlowMotion"].ReadValue<float>();
        bool isInputActive = input > 0;

        if (isInputActive && !_slowMotionActive)
        {
            StartCoroutine(SlowMotionEffect());
        }
    }
    

    private void SwooshSound(bool slowIn)
    {
        if (slowIn)
        {
            swooshSource.pitch = 1f;
            swooshSource.timeSamples = 0;
            swooshSource.Play();
        }
        else
        {
            swooshSource.pitch = -1f;
            swooshSource.timeSamples = swooshSource.clip.samples - 1;
            swooshSource.Play();
        }
    }

    private void TogglePostProcessingEffects()
    {
        // Toggle chromatic aberration
        if (_chromaticAberration != null)
        {
            _showAberration = !_showAberration;
            float targetIntensity = _showAberration ? 0.805f : 0f;
            StartCoroutine(ChangeAberration(targetIntensity, transitionDuration));
        }

        // Toggle saturation
        if (_colorAdjustments != null)
        {
            _showSaturation = !_showSaturation;
            float targetSaturation = _showSaturation ? -84f : 0f;
            StartCoroutine(ChangeSaturation(targetSaturation, transitionDuration));
        }
    }

    IEnumerator SlowMotionEffect()
    {
        _slowMotionActive = true;

        StartCoroutine(TimeTransition(slowDownFactor, transitionDuration));
        
        // SwooshSound(true);
        StartCoroutine(ChangeMusicPitch(musicSlowDownFactor, transitionDuration));
        StartCoroutine(ChangeSfxPitch(sfxSlowDownFactor, transitionDuration));
        
        TogglePostProcessingEffects();

        yield return new WaitForSeconds(GameManager.Instance.slowMotionDuration - transitionDuration);
        
        // Start transitioning back to normal
        // SwooshSound(false);
        StartCoroutine(ChangeMusicPitch(1f, transitionDuration));
        StartCoroutine(ChangeSfxPitch(1f, transitionDuration));
        TogglePostProcessingEffects();
        StartCoroutine(TimeTransition(1f, transitionDuration));
        
        yield return new WaitForSeconds(transitionDuration);

        _slowMotionActive = false;
    }

    IEnumerator StopSlowMotion()
    {
        StartCoroutine(ChangeMusicPitch(1f, transitionDuration));
        StartCoroutine(ChangeSfxPitch(1f, transitionDuration));
        TogglePostProcessingEffects();
        StartCoroutine(TimeTransition(1f, transitionDuration));
        
        yield return new WaitForSeconds(transitionDuration);

        _slowMotionActive = false;
    }

    IEnumerator TimeTransition(float slowDown, float duration)
    {
        float initialTimeScale = Time.timeScale;
        float elapsedTime = 0f;
    
        while (elapsedTime < transitionDuration)
        {
            Time.timeScale = Mathf.Lerp(initialTimeScale, slowDown, elapsedTime / duration);
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = slowDown;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }
    
    IEnumerator ChangeMusicPitch(float targetPitch, float duration)
    {
        float currentTime = 0;
        musicMixer.GetFloat("MusicPitch", out float currentPitch);
    
        while (currentTime < duration)
        {
            currentTime += Time.unscaledDeltaTime;
            float newPitch = Mathf.Lerp(currentPitch, targetPitch, currentTime / duration);
            musicMixer.SetFloat("MusicPitch", newPitch);
            yield return null;
        }

        musicMixer.SetFloat("MusicPitch", targetPitch);
    }

    IEnumerator ChangeSfxPitch(float targetPitch, float duration)
    {
        float currentTime = 0;
        musicMixer.GetFloat("SFXPitch", out float currentPitch);
    
        while (currentTime < duration)
        {
            currentTime += Time.unscaledDeltaTime;
            float newPitch = Mathf.Lerp(currentPitch, targetPitch, currentTime / duration);
            musicMixer.SetFloat("SFXPitch", newPitch);
            yield return null;
        }

        musicMixer.SetFloat("SFXPitch", targetPitch);
    }
    
    public IEnumerator ChangeAberration(float targetIntensity, float duration)
    {
        float currentTime = 0;
        float initialIntensity = _chromaticAberration.intensity.value;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            _chromaticAberration.intensity.value = Mathf.Lerp(initialIntensity, targetIntensity, currentTime / duration);
            yield return null;
        }

        _chromaticAberration.intensity.value = targetIntensity;
    }

    public IEnumerator ChangeSaturation(float targetSaturation, float duration)
    {
        float currentTime = 0;
        float initialSaturation = _colorAdjustments.saturation.value;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            _colorAdjustments.saturation.value = Mathf.Lerp(initialSaturation, targetSaturation, currentTime / duration);
            yield return null;
        }

        _colorAdjustments.saturation.value = targetSaturation;
    }
}
