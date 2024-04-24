using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public string abilityName;
    public Image mask;
    public GameObject progressBar;
    public GameObject levelUpPrompt;
    public TMP_Text levelUpText;
    public Animator levelUpAnimator;
    public Animator animator;
    private int _minimum;
    private int _maximum;
    private int _current;
    private bool _barShowing = false;

    private void Update()
    {
        if (GameManager.Instance.resumed && _barShowing)
        {
            StartCoroutine(CloseBar());
        }
    }

    public void UpdateProgressBar()
    {
        LevelManager.AbilityLevel abilityLevel = LevelManager.Instance.GetAbilityLevelByName(abilityName);
        if (abilityLevel != null)
        {
            _maximum = abilityLevel.xpToNextLevel;
            _current = abilityLevel.currentXP;
            GetCurrentFill();

            if (_barShowing)
            {
                StopAllCoroutines();
            }
            
            progressBar.SetActive(true);
            StartCoroutine(CloseBar());
        }
    }
    
    public void LevelUpProgressBar()
    {
        LevelManager.AbilityLevel abilityLevel = LevelManager.Instance.GetAbilityLevelByName(abilityName);
        if (abilityLevel != null)
        {
            _minimum = abilityLevel.currentXP;
            _maximum = abilityLevel.xpToNextLevel;
            _current = abilityLevel.currentXP;
            GetCurrentFill();
            
            progressBar.SetActive(true);
            
            levelUpPrompt.SetActive(true);
            levelUpAnimator.Play("LevelUpIn");
            levelUpText.text = $"{abilityName} - Level {abilityLevel.currentLevel}";
            
            StartCoroutine(CloseBar());
        }
    }
    
    private void GetCurrentFill()
    {
        float currentOffset = _current - _minimum;
        float maximumOffset = _maximum - _minimum;
        float fillAmount = currentOffset / maximumOffset;
        mask.fillAmount = fillAmount;

        // fill.color = color;
    }

    IEnumerator CloseBar()
    {
        _barShowing = true;
        yield return new WaitForSeconds(2f);
        if (abilityName == "Telekinesis") animator.Play("TelekinesisXPClose");
        if (abilityName == "Telepathy") animator.Play("TelepathyXPClose");
        if (abilityName == "Psychokinesis") animator.Play("PsychokinesisXPClose");
        
        yield return new WaitForSeconds(2f);
        progressBar.SetActive(false);
        _barShowing = false;
        if (!levelUpPrompt.GameObject().activeInHierarchy) GameManager.Instance.resumed = false;
        
        if (levelUpPrompt.GameObject().activeInHierarchy)
        {
            levelUpAnimator.Play("LevelUpOut");
            yield return new WaitForSeconds(1f);
            levelUpPrompt.SetActive(false);
            GameManager.Instance.resumed = false;
        }
    }
}
