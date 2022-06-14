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
        All = -1,
        Normal = 0,
        Skill = 2,
        Passive = 4,

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

    public enum BattleState
    {
        None,
        Idle,
        InBattle,
    }
}