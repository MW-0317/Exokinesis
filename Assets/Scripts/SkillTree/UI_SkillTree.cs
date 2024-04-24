using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_SkillTree : MonoBehaviour
{
    public bool canSaveSkills = false;
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private TMP_Text skillDescription;
    [SerializeField] private TMP_Text skillRequirements;
    private string _selectedSkill;

    private void Start()
    {
        UpdateDescription("Please select a skill.");
        skillRequirements.text = "N/A";
        
        if (!NewGame.Instance.isNewGame) GameManager.Instance.LoadSkillPoints();
        
        InitializeSkillsUI();

        canSaveSkills = true;
    }

    public void SelectedSkill(string currentSkill)
    {
        _selectedSkill = currentSkill;
    }

    public void UpdateDescription(string description) { skillDescription.text = description; }

    public void UpdateRequirements(string skillType)
    {
        if (PlayerSkills.Instance.SkillRequirementsFromString(skillType) <= 1f)
            skillRequirements.text = $"{PlayerSkills.Instance.SkillPointsFromString(skillType)}/{PlayerSkills.Instance.SkillRequirementsFromString(skillType)} point";
        else skillRequirements.text = $"{PlayerSkills.Instance.SkillPointsFromString(skillType)}/{PlayerSkills.Instance.SkillRequirementsFromString(skillType)} points";
    }

    public void UpdateSelectedRequirements()
    {
        if (PlayerSkills.Instance.SkillRequirementsFromString(_selectedSkill) <= 1f)
            skillRequirements.text = $"{PlayerSkills.Instance.SkillPointsFromString(_selectedSkill)}/{PlayerSkills.Instance.SkillRequirementsFromString(_selectedSkill)} point";
        else skillRequirements.text = $"{PlayerSkills.Instance.SkillPointsFromString(_selectedSkill)}/{PlayerSkills.Instance.SkillRequirementsFromString(_selectedSkill)} points";
    }

    public void UnlockPlayerSkill()
    {
        switch (_selectedSkill)
        {
            // (false, false) is for an unlocked skill, (false, true) for an available skill, (true, false) for locked
            case "Telekinesis1":
                if (PlayerSkills.Instance.TryUnlockSkill(PlayerSkills.SkillType.Telekinesis1))
                {
                    buttons[0].GetComponent<UpdateVisibility>().UpdateSkillVisibility(false, false);
                    buttons[1].GetComponent<UpdateVisibility>().UpdateSkillVisibility(false, true);
                    GameManager.Instance.SaveSkillPoints();
                    PlayerSkills.Instance.SaveUnlockedSkills();
                }
                break;
            case "Telekinesis2":
                if (PlayerSkills.Instance.TryUnlockSkill(PlayerSkills.SkillType.Telekinesis2))
                {
                    buttons[1].GetComponent<UpdateVisibility>().UpdateSkillVisibility(false, false);
                    buttons[2].GetComponent<UpdateVisibility>().UpdateSkillVisibility(false, true);
                    GameManager.Instance.SaveSkillPoints();
                    PlayerSkills.Instance.SaveUnlockedSkills();
                }
                break;
            case "Telepathy1":
                if (PlayerSkills.Instance.TryUnlockSkill(PlayerSkills.SkillType.Telepathy1))
                {
                    buttons[3].GetComponent<UpdateVisibility>().UpdateSkillVisibility(false, false);
                    buttons[4].GetComponent<UpdateVisibility>().UpdateSkillVisibility(false, true);
                    GameManager.Instance.SaveSkillPoints();
                    PlayerSkills.Instance.SaveUnlockedSkills();
                }
                break;
            case "Telepathy2":
                if (PlayerSkills.Instance.TryUnlockSkill(PlayerSkills.SkillType.Telepathy2))
                {
                    buttons[4].GetComponent<UpdateVisibility>().UpdateSkillVisibility(false, false);
                    buttons[5].GetComponent<UpdateVisibility>().UpdateSkillVisibility(false, true);
                    GameManager.Instance.SaveSkillPoints();
                    PlayerSkills.Instance.SaveUnlockedSkills();
                }
                break;
            case "Psychokinesis1":
            {
                if (PlayerSkills.Instance.TryUnlockSkill(PlayerSkills.SkillType.Psychokinesis1))
                {
                    buttons[6].GetComponent<UpdateVisibility>().UpdateSkillVisibility(false, false);
                    buttons[7].GetComponent<UpdateVisibility>().UpdateSkillVisibility(false, true);
                    GameManager.Instance.SaveSkillPoints();
                    PlayerSkills.Instance.SaveUnlockedSkills();
                }
                break;
            }
            case "Psychokinesis2":
                if (PlayerSkills.Instance.TryUnlockSkill(PlayerSkills.SkillType.Psychokinesis2))
                {
                    buttons[7].GetComponent<UpdateVisibility>().UpdateSkillVisibility(false, false);
                    buttons[8].GetComponent<UpdateVisibility>().UpdateSkillVisibility(false, true); 
                    GameManager.Instance.SaveSkillPoints();
                    PlayerSkills.Instance.SaveUnlockedSkills();
                }
                break;
        }
    }

    private void InitializeSkillsUI()
    {
        if (PlayerSkills.Instance.IsSkillUnlocked(PlayerSkills.SkillType.Telekinesis1))
        {
            buttons[0].GetComponent<UpdateVisibility>().UpdateSkillVisibility(false, false);
            buttons[1].GetComponent<UpdateVisibility>().UpdateSkillVisibility(false, true);
        }
        if (PlayerSkills.Instance.IsSkillUnlocked(PlayerSkills.SkillType.Telekinesis2))
        {
            buttons[1].GetComponent<UpdateVisibility>().UpdateSkillVisibility(false, false);
            buttons[2].GetComponent<UpdateVisibility>().UpdateSkillVisibility(false, true);
        }
        if (PlayerSkills.Instance.IsSkillUnlocked(PlayerSkills.SkillType.Telepathy1))
        {
            buttons[3].GetComponent<UpdateVisibility>().UpdateSkillVisibility(false, false);
            buttons[4].GetComponent<UpdateVisibility>().UpdateSkillVisibility(false, true);
        }
        if (PlayerSkills.Instance.IsSkillUnlocked(PlayerSkills.SkillType.Telepathy2))
        {
            buttons[4].GetComponent<UpdateVisibility>().UpdateSkillVisibility(false, false);
            buttons[5].GetComponent<UpdateVisibility>().UpdateSkillVisibility(false, true);
        }
        if (PlayerSkills.Instance.IsSkillUnlocked(PlayerSkills.SkillType.Psychokinesis1))
        {
            buttons[6].GetComponent<UpdateVisibility>().UpdateSkillVisibility(false, false);
            buttons[7].GetComponent<UpdateVisibility>().UpdateSkillVisibility(false, true);
        }
        if (PlayerSkills.Instance.IsSkillUnlocked(PlayerSkills.SkillType.Psychokinesis2))
        {
            buttons[7].GetComponent<UpdateVisibility>().UpdateSkillVisibility(false, false);
            buttons[8].GetComponent<UpdateVisibility>().UpdateSkillVisibility(false, true); 
        }
    }
}
