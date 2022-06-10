using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UUnityEngine;

public class UISkillSlots : MonoBehavior
{
    public UISkillSlots[] slots;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    public void UpdateSkills()
    {
        if (User.Instance.CurrentCharacter == null) return;
        var Skills = User.Instance.CurrentCharacter.SkillMgr.Skills;
        int skillIdx = 0;
        foreach (var skill in Skills)
        {
            slots[skillIdx].SetSkill(skill);
            skillIdx++;
        }
    }
}