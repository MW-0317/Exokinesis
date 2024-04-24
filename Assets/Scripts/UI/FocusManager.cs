using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusManager : MonoBehaviour
{
    private List<AudioSource> playingSourcesBeforePause = new List<AudioSource>();

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Time.timeScale = 1f;
            ResumeAudio();
        }
        else
        {
            PauseAudio();
            Time.timeScale = 0f;
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            PauseAudio();
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
            ResumeAudio();
        }
    }

    private void PauseAudio()
    {
        playingSourcesBeforePause.Clear();
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (var audioSrc in allAudioSources)
        {
            if (audioSrc.isPlaying)
            {
                playingSourcesBeforePause.Add(audioSrc);
                audioSrc.Pause();
            }
        }
    }

    private void ResumeAudio()
    {
        foreach (var audioSrc in playingSourcesBeforePause)
        {
            audioSrc.UnPause();
        }
        playingSourcesBeforePause.Clear();
    }
}
