using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeAudio : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _audioSource.Play();
            _audioSource.volume = 0f;
            StartCoroutine(FadeInMusic(_audioSource, 0f, 4f));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(FadeOutMusic(_audioSource, 0f, 4f));
        }
    }
    
    IEnumerator FadeInMusic(AudioSource audioSource, float delay, float fadeInDuration)
    {
        yield return new WaitForSeconds(delay);

        float currentTime = 0;
        float start = 0f;
        float targetVolume = 0.25f;

        while (currentTime < fadeInDuration)
        {
            currentTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(start, targetVolume, currentTime / fadeInDuration);
            audioSource.volume = newVolume;
            yield return null; // Wait a frame and continue
        }

        audioSource.volume = targetVolume;
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
        audioSource.Stop();
    }
}
