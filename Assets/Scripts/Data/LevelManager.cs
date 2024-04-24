using System;
using UnityEngine;
using CI.QuickSave;

public class LevelManager : MonoBehaviour
{
    [Serializable]
    public class AbilityLevel
    {
        public int currentLevel = 1;
        public int currentXP = 0;
        public int xpToNextLevel = 100;
    }

    public AbilityLevel telekinesis = new AbilityLevel();
    public AbilityLevel telepathy = new AbilityLevel();
    public AbilityLevel psychokinesis = new AbilityLevel();

    public static LevelManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if (NewGame.Instance.isNewGame && !GameManager.Instance.isLevel2)
        {
            telekinesis.currentXP = 0;
            telepathy.currentXP = 0;
            psychokinesis.currentXP = 0;
            telekinesis.currentLevel = 0;
            telepathy.currentLevel = 0;
            psychokinesis.currentLevel = 0;
        }
        else LoadAbilities();
    }

    public void AddXP(string abilityName, int xpToAdd)
    {
        AbilityLevel abilityLevel = GetAbilityLevelByName(abilityName);
        if (abilityLevel != null)
        {
            abilityLevel.currentXP += xpToAdd;
            CheckForLevelUp(abilityLevel, abilityName);
            UpdateProgressBar(abilityName);
            
            SaveAbilityStats();
        }
    }
    
    private void UpdateProgressBar(string abilityName)
    {
        ProgressBar[] progressBars = FindObjectsOfType<ProgressBar>();
        foreach (var progressBar in progressBars)
        {
            if (progressBar.abilityName == abilityName)
            {
                progressBar.UpdateProgressBar();
            }
        }
    }
    
    private void ProgressBarLvlUp(string abilityName)
    {
        ProgressBar[] progressBars = FindObjectsOfType<ProgressBar>();
        foreach (var progressBar in progressBars)
        {
            if (progressBar.abilityName == abilityName)
            {
                progressBar.LevelUpProgressBar();
            }
        }
    }

    public AbilityLevel GetAbilityLevelByName(string name)
    {
        switch (name)
        {
            case "Telekinesis": return telekinesis;
            case "Telepathy": return telepathy;
            case "Psychokinesis": return psychokinesis;
            default: return null;
        }
    }

    private void CheckForLevelUp(AbilityLevel abilityLevel, string abilityName)
    {
        if (abilityLevel.currentXP >= abilityLevel.xpToNextLevel)
        {
            GameManager.Instance.AddSkillPoint(abilityName);
            
            abilityLevel.currentLevel++;
            abilityLevel.currentXP -= abilityLevel.xpToNextLevel;
            // Adjust xpToNextLevel as needed for the next level
            abilityLevel.xpToNextLevel += 50;
            // Show level up prompt
            ProgressBarLvlUp(abilityName);
            
            SaveAbilityStats();
        }
    }
    
    public void SaveAbilityStats()
    {
        var writer = QuickSaveWriter.Create("PlayerAbilities");

        writer.Write("Telekinesis_CurrentLevel", telekinesis.currentLevel);
            // .Write("Telekinesis_CurrentXP", telekinesis.currentXP)
            // .Write("Telekinesis_XPToNextLevel", telekinesis.xpToNextLevel);

        writer.Write("Telepathy_CurrentLevel", telepathy.currentLevel);
            // .Write("Telepathy_CurrentXP", telepathy.currentXP)
            // .Write("Telepathy_XPToNextLevel", telepathy.xpToNextLevel);

        writer.Write("Psychokinesis_CurrentLevel", psychokinesis.currentLevel);
            // .Write("Psychokinesis_CurrentXP", psychokinesis.currentXP)
            // .Write("Psychokinesis_XPToNextLevel", psychokinesis.xpToNextLevel);

        writer.Commit();
    }

    public void LoadAbilities()
    {
        if(NewGame.Instance.isNewGame) return;
        
        var reader = QuickSaveReader.Create("PlayerAbilities");
        
        telekinesis.currentLevel = reader.Read<int>("Telekinesis_CurrentLevel");
        // telekinesis.currentXP = reader.Read<int>("Telekinesis_CurrentXP");
        // telekinesis.xpToNextLevel = reader.Read<int>("Telekinesis_XPToNextLevel");
        
        telepathy.currentLevel = reader.Read<int>("Telepathy_CurrentLevel");
        // telepathy.currentXP = reader.Read<int>("Telepathy_CurrentXP");
        // telepathy.xpToNextLevel = reader.Read<int>("Telepathy_XPToNextLevel");
        
        psychokinesis.currentLevel = reader.Read<int>("Psychokinesis_CurrentLevel");
        // psychokinesis.currentXP = reader.Read<int>("Psychokinesis_CurrentXP");
        // psychokinesis.xpToNextLevel = reader.Read<int>("Psychokinesis_XPToNextLevel");
    }
}