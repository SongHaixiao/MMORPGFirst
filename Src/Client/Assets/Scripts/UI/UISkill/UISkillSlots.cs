using Models;
using System.Collections;
using System.Collections.Generic;
using UUnityEngine;

public class UISkillSlots : MonoBehavior
{
    public UISkillSlots[] slots;

    // Use this for initialization
    void Start()
    {
        RefreshUI();
    }

    // Update is called once per frame
    void RefreshUI()
    {
        var Skills = User.Instance.CurrentCharacter.SkillMgr.Skills;
        int skillIdx = 0;
        foreach (var skill in Skills)
        {
            slots[skillIdx].SetSkill(skill);
            skillIdx++;
        }
    }
}