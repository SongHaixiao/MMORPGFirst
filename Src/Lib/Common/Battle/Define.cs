using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Battle
{
    public enum AttributeType
    {
        None = -1,
        MaxHP = 0,
        MaxMP = 1,
        STR = 2,
        INT = 3,
        DEX = 4,
        AD = 5,
        AP = 6,
        DEF = 7,
        MDEF = 8,
        SPD = 9,
        CRI = 10,
        MAX
    }

    public enum SkillType
    {
        Normal = 0,
        Skill = 1
    }

    public enum SkillResult
    {
        OK = 0,
        InvalidTarget = 1,
        OutOfMP,
        Cooldown
    }

    public enum TargetType
    {
        None = 0,
        Target = 1,
        Self = 2,
        Position
    }

    public enum BuffEffect
    {
        None = 0,
        Stun = 1,
        Invincible = 2
    }

    public enum TriggerType
    {
        None = 0,
        SkillCast = 1,
        SkillHit = 2
    }
}