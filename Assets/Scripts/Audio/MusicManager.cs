using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource audioSourceLow;
    public AudioSource audioSourceMedium;
    public AudioSource audioSourceHigh;
    public Transform playerTransform;
    public float mediumIntensityDistance = 30f;
    public float highIntensityDistance = 10f;
    public float crossfadeSpeed = 1f;
    public float detectionRadius = 50f; // Radius for detecting guards
    public LayerMask enemyLayer;
    private bool _isMusicPlaying;

    private Transform closestEnemyTransform;
    
    public AudioSource introCutscene;

    private void Awake()
    {
        introCutscene = GameObject.Find("IntroCutscene").GetComponent<AudioSource>();
    }

    private void Start()
    {
        audioSourceHigh.volume = 0;
        audioSourceMedium.volume = 0;
        audioSourceLow.volume = 0;
        audioSourceHigh.Play();
        audioSourceMedium.Play();
        audioSourceLow.Play();
        
        if (NewGame.Instance.isNewGame && !GameManager.Instance.isLevel2)
        {
            introCutscene.enabled = true;
            
            // Fade in music after a delay (medium intensity)
            StartCoroutine(FadeInMusic(audioSourceLow, 27f, 4f));
            StartCoroutine(FadeInMusic(audioSourceMedium, 27f, 4f));
        }
        else
        {
            StartCoroutine(FadeInMusic(audioSourceLow, 0f, 4f));
            // StartCoroutine(FadeInMusic(audioSourceMedium, 0f, 4f));
        }
    }

    void Update()
    {
        FindClosestEnemy();
        if (closestEnemyTransform != null)
        {
            float distance = Vector3.Distance(playerTransform.position, closestEnemyTransform.position);
            
            if (_isMusicPlaying) UpdateMusicIntensity(distance);
        }
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && !NewGame.Instance.isNewGame)
        {
            audioSourceLow.Pause();
            audioSourceMedium.Pause();
            audioSourceHigh.Pause();
        }
        else if (!NewGame.Instance.isNewGame)
        {
            if (_isMusicPlaying)
            {
                audioSourceLow.UnPause();
                audioSourceMedium.UnPause();
                audioSourceHigh.UnPause();
            }
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && !NewGame.Instance.isNewGame)
        {
            audioSourceLow.Pause();
            audioSourceMedium.Pause();
            audioSourceHigh.Pause();
        }
        else if (!NewGame.Instance.isNewGame)
        {
            if (_isMusicPlaying)
            {
                audioSourceLow.UnPause();
                audioSourceMedium.UnPause();
                audioSourceHigh.UnPause();
            }
        }
    }

    void FindClosestEnemy()
    {
        Collider[] hitColliders = Physics.OverlapSphere(playerTransform.position, detectionRadius, enemyLayer);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hitCollider in hitColliders)
        {
            float distance = Vector3.Distance(playerTransform.position, hitCollider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = hitCollider.transform;
            }
        }

        closestEnemyTransform = closestEnemy;
    }

    void UpdateMusicIntensity(float distance)
    {
        // Low intensity by default
        float lowVolume = 1;
        float mediumVolume = 0;
        float highVolume = 0;

        if (distance < highIntensityDistance)
        {
            // Close to an enemy, add high intensity
            highVolume = 1;
            mediumVolume = 1; // Maintain medium intensity
        }
        else if (distance < mediumIntensityDistance)
        {
            // In the vicinity of an enemy, add medium intensity
            float normalizedDistance = (distance - highIntensityDistance) / (mediumIntensityDistance - highIntensityDistance);
            mediumVolume = 1 - normalizedDistance;
        }

        // Smoothly crossfade between tracks
        audioSourceLow.volume = Mathf.Lerp(audioSourceLow.volume, lowVolume, crossfadeSpeed * Time.deltaTime);
        audioSourceMedium.volume = Mathf.Lerp(audioSourceMedium.volume, mediumVolume, crossfadeSpeed * Time.deltaTime);
        audioSourceHigh.volume = Mathf.Lerp(audioSourceHigh.volume, highVolume, crossfadeSpeed * Time.deltaTime);
    }

    public void FadeMusicOut(float duration)
    {
        StartCoroutine(FadeOutMusic(audioSourceLow, 0f, duration));
        StartCoroutine(FadeOutMusic(audioSourceMedium, 0f, duration));
        StartCoroutine(FadeOutMusic(audioSourceHigh, 0f, duration));
    }

    public void FadeMusicIn()
    {
        StartCoroutine(FadeInMusic(audioSourceLow, 0f, 4f));
    }

    IEnumerator FadeInMusic(AudioSource audioSource, float delay, float fadeInDuration)
    {
        yield return new WaitForSeconds(delay);

        float currentTime = 0;
        float start = 0f;
        float targetVolume = 1f;

        while (currentTime < fadeInDuration)
        {
            currentTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(start, targetVolume, currentTime / fadeInDuration);
            audioSource.volume = newVolume;
            yield return null; // Wait a frame and continue
        }

        audioSource.volume = targetVolume;

        _isMusicPlaying = true;

        if (NewGame.Instance.isNewGame)
        {
            NewGame.Instance.isNewGame = false; // Should only be true when loading game from menu
        }
    }
    
    IEnumerator FadeOutMusic(AudioSource audioSource, float delay, float fadeOutDuration)
    {
        yield return new WaitForSeconds(delay);

        float currentTime = 0f;
        float start = audioSource.volume;
        float targetVolume = 0f;

        while (currentTime < fadeOutDuration)
        {
            currentTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(start, targetVolume, currentTime / fadeOutDuration);
            audioSource.volume = newVolume;
            yield return null; // Wait a frame and continue
        }

        audioSource.volume = targetVolume;

        _isMusicPlaying = false;
    }
}
