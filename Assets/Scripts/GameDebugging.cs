using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDebugging : MonoBehaviour
{
    [SerializeField] private bool unlockTelekinesis1;
    [SerializeField] private bool unlockTelekinesis2;
    [SerializeField] private bool unlockTelepathy1;
    [SerializeField] private bool unlockTelepathy2;
    [SerializeField] private bool unlockPsychokinesis1;
    [SerializeField] private bool unlockPsychokinesis2;

    private void Update()
    {
        if(unlockTelekinesis1) PlayerSkills.Instance.UnlockSkill(PlayerSkills.SkillType.Telekinesis1);
        if(unlockTelekinesis2) PlayerSkills.Instance.UnlockSkill(PlayerSkills.SkillType.Telekinesis2);
        if(unlockTelepathy1) PlayerSkills.Instance.UnlockSkill(PlayerSkills.SkillType.Telepathy1);
        if(unlockTelepathy2) PlayerSkills.Instance.UnlockSkill(PlayerSkills.SkillType.Telepathy2);
        if(unlockPsychokinesis1) PlayerSkills.Instance.UnlockSkill(PlayerSkills.SkillType.Psychokinesis1);
        if(unlockPsychokinesis2) PlayerSkills.Instance.UnlockSkill(PlayerSkills.SkillType.Psychokinesis2);
    }
}
