using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CI.QuickSave;

public class PlayerSkills: MonoBehaviour
{
    public static PlayerSkills Instance { get; private set; }
    
    public enum SkillType
    {
        None,
        Telekinesis1,
        Telepathy1,
        Psychokinesis1,
        Telekinesis2,
        Telepathy2,
        Psychokinesis2,
    }

    public enum SkillPointType
    {
        Telekinesis,
        Telepathy,
        Psychokinesis
    }
    
    public Dictionary<SkillType, int> SkillPointRequirements = new()
    {
        { SkillType.Telekinesis1, 1 },
        { SkillType.Telepathy1, 1 },
        { SkillType.Psychokinesis1, 1 },
        { SkillType.Telekinesis2, 2 },
        { SkillType.Telepathy2, 2 },
        { SkillType.Psychokinesis2, 2 },
    };

    private int _currentSkillPoints;
    private bool _savedSkillPoints;

    private List<SkillType> _unlockedSkillTypeList = new List<SkillType>();
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        if (!NewGame.Instance.isNewGame && !GameManager.Instance.isLevel2) LoadUnlockedSkills();
        else if (NewGame.Instance.isNewGame && GameManager.Instance.isLevel2) LoadUnlockedSkills();
        else _unlockedSkillTypeList.Clear();
    }

    public void UnlockSkill(SkillType skillType)
    {
        if (!IsSkillUnlocked(skillType))
        {
            _unlockedSkillTypeList.Add(skillType);
            switch (skillType)
            {
                case SkillType.Telekinesis1: 
                    GameManager.Instance.maxTelekinesisWeight = 25f;
                    break;
                case SkillType.Telekinesis2:
                    GameManager.Instance.maxTelekinesisWeight = 50f;
                    break;
                case SkillType.Telepathy1:
                    // Handled in DialogueController
                    break;
                case SkillType.Telepathy2:
                    // Handled in DialogueController
                    break;
                case SkillType.Psychokinesis1:
                    GameManager.Instance.slowMotionDuration = 4f;
                    break;
                case SkillType.Psychokinesis2:
                    GameManager.Instance.slowMotionDuration = 7f;
                    break;
            }
        }
    }
    
    public void SaveUnlockedSkills()
    {
        var writer = QuickSaveWriter.Create("Skills");

        // Serialize _unlockedSkillTypeList to a comma-separated string
        string serializedSkills = string.Join(",", _unlockedSkillTypeList.Select(skill => skill.ToString()));

        writer.Write("unlockedSkills", serializedSkills);
        
        writer.Commit();
    }
    
    public void LoadUnlockedSkills()
    {
        var reader = QuickSaveReader.Create("Skills");
        
        Debug.Log("Trying to load skills");
        string serializedSkills = reader.Read<string>("unlockedSkills");

        // Deserialize the string back into a list of SkillType
        _unlockedSkillTypeList = serializedSkills.Split(',')
            .Select(str => StringToSkillType(str))
            .Where(skill => skill != SkillType.None)
            .ToList();

        foreach (var skill in _unlockedSkillTypeList)
        {
            UnlockSkill(skill);
        }
    }

    public bool IsSkillUnlocked(SkillType skillType)
    {
        return _unlockedSkillTypeList.Contains(skillType);
    }

    public SkillType StringToSkillType(string skillName)
    {
        switch (skillName)
        {
            case "Telekinesis1": return SkillType.Telekinesis1;
            case "Telekinesis2": return SkillType.Telekinesis2;
            case "Telepathy1": return SkillType.Telepathy1;
            case "Telepathy2": return SkillType.Telepathy2;
            case "Psychokinesis1": return SkillType.Psychokinesis1;
            case "Psychokinesis2": return SkillType.Psychokinesis2;
            default: return SkillType.None;
        }
    }

    public float SkillPointsFromString(string skillType)
    {
        SkillType skill = StringToSkillType(skillType);
        float skillPoints = GetSkillPointType(skill);
        return skillPoints;
    }

    public float SkillRequirementsFromString(string skillType)
    {
        SkillType skill = StringToSkillType(skillType);
        float skillReq = SkillPointRequirements[skill];
        return skillReq;
    }

    public SkillType GetSkillRequirement(SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.Psychokinesis2: return SkillType.Psychokinesis1;
            case SkillType.Telekinesis2: return SkillType.Telekinesis1;
            case SkillType.Telepathy2: return SkillType.Telepathy1;
        }

        return SkillType.None;
    }

    public float GetSkillPointType(SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.Telekinesis1:
            case SkillType.Telekinesis2:
                return GameManager.Instance.telekinesisSkillPoints;
            case SkillType.Telepathy1:
            case SkillType.Telepathy2:
                return GameManager.Instance.telepathySkillPoints;
            case SkillType.Psychokinesis1:
            case SkillType.Psychokinesis2:
                return GameManager.Instance.psychokinesisSkillPoints;
            default:
                return 0f;
        }
    }

    public bool CanUnlock(SkillType skillType)
    {
        SkillType skillRequirement = GetSkillRequirement(skillType);
        if (skillRequirement != SkillType.None)
        {
            if (IsSkillUnlocked(skillRequirement) && SkillPointRequirements.ContainsKey(skillType))
            {
                return GetSkillPointType(skillType) >= SkillPointRequirements[skillType];
            }
            else
            {
                return false;
            }
        }
        if (SkillPointRequirements.ContainsKey(skillType))
        {
            return GetSkillPointType(skillType) >= SkillPointRequirements[skillType];
        }

        return false;
    }

    public bool TryUnlockSkill(SkillType skillType)
    {
        if (CanUnlock(skillType))
        {
            UnlockSkill(skillType);
            switch (skillType)
            {
                case SkillType.Telekinesis1:
                case SkillType.Telekinesis2:
                    GameManager.Instance.telekinesisSkillPoints -= SkillPointRequirements[skillType];
                    break;
                case SkillType.Telepathy1:
                case SkillType.Telepathy2:
                    GameManager.Instance.telepathySkillPoints -= SkillPointRequirements[skillType];
                    break;
                case SkillType.Psychokinesis1:
                case SkillType.Psychokinesis2:
                    GameManager.Instance.psychokinesisSkillPoints -= SkillPointRequirements[skillType];
                    break;
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}
