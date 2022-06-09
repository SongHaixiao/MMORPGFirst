using Battle;
using Common.Battle;
using Common.Data;
using Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Entities
{
    public class Creature : Entity
    {
        public NCharacterInfo Info;
        public CharacterDefine Define;
        public Attributes Attributes;
        public SkillManager SkillMgr;

        bool battleState = false;
        public bool BattleStates
        {
            get { return battleState; }
            set
            {
                if (battleState != value)
                {
                    battleState = value;
                    this.SetStandby(value);
                }
            }
        }

        public int Id
        {
            get { return this.Info.Id; }
        }

        public Skill CastringSkill = null;

        public string Name
        {
            get
            {
                if (this.Info.Type == CharacterType.Player)
                    return this.Info.Name;
                else
                    return this.Define.Name;
            }
        }

        public bool IsPlayer
        {
            get
            {
                return this.Info.Type == CharacterType.Player;
            }
        }

        public bool IsCurrentPlayer
        {
            get
            {
                if (!IsPlayer) return false;
                return this.Info.Id == Models.User.Instance.CurrentCharacterInfo.Id;
            }
        }

        public Creature(NCharacterInfo info) : base(info.Entity)
        {
            this.Info = info;
            this.Define = DataManager.Instance.Characters[info.ConfigId];
            this.Attributes = new Attributes();
            this.Attributes.Init(this.Define, this.Info.Level, this.GetEquips(), this.Info.attrDynamic);

            this.SkillMgr = new SkillManager(this);
        }

        public virtual List<EquipDefine> GetEquips()
        {
            return null;
        }

        public void MoveForward()
        {
            Debug.LogFormat("Move Forward");
            this.speed = this.Define.Speed;
        }

        public void MoveBack()
        {
            Debug.LogFormat("Move Back");
            this.speed = -this.Define.Speed;
        }

        public void Stop()
        {
            Debug.LogFormat("Stop");
            this.speed = 0;
        }

        public void SetDirection(Vector3Int direction)
        {
            Debug.LogFormat("SetDirection : {0}", direction);
            this.direction = direction;
        }

        public void SetPosition(Vector3Int position)
        {
            Debug.LogFormat("SetPosition : {0}", position);
            this.SetPosition = position;
        }

        public void CastSkill(int skillId, Creature target, NVector3 pos)
        {
            this.SetStandby(true);
            var skill = this.SkillMgr.GetSkill(skillId);
            skill.BeginCast();
        }

        public void PlayAnim(string name)
        {
            if (this.Controller != null)
                this.Controller.PlayAnim(name);
        }

        public void SetStandby(bool stanby)
        {
            if (this.Controller != null)
                this.Controller.SetStandby(standby);
        }

        public override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);
            this.SkillMgr.OnUpdate(delta);
        }
    }
}