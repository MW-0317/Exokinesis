using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateVisibility : MonoBehaviour
{
    [SerializeField] private TMP_Text skillText;
    [SerializeField] private bool isDefaultAvailable;
    [SerializeField] private bool isDefaultLocked;
    private Color32 _lockedColor;
    private Color32 _availableColor;
    private Color32 _unlockedColor;

    private void Awake()
    {
        _lockedColor = skillText.faceColor;
        _lockedColor.a = 40; // out of 255
        _availableColor = skillText.faceColor;
        _availableColor.a = 155;
        _unlockedColor = skillText.faceColor;
        _unlockedColor.a = 255;
        
        UpdateSkillVisibility(isDefaultLocked, isDefaultAvailable);
    }

    public void UpdateSkillVisibility(bool isLocked, bool isAvailable)
    {
        if (isLocked) skillText.faceColor = _lockedColor;
        else if (isAvailable) skillText.faceColor = _availableColor;
        else skillText.faceColor = _unlockedColor;
    }
}
