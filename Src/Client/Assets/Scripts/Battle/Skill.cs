using Common.Data;
using Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Common.Battle;

namespace Battle
{
    public class Skill
    {
        public NSkillInfo Info;
        public Creature Owner;
        public SkillDefine Define;
        public NDamageInfo Damage

        private int Hit = 0;
        private float skillTime = 0;
        private float castTime = 0;

        public bool IsCasting = false;

        Dictionary<int, List<NDamageInfo>> HitMap = new Dictionary<int, List<NDamageInfo>>();

        private float cd = 0;
        public float CD
        {
            get { return cd; }
        }

        public Skill(NSkillInfo info, Creature owner)
        {
            this.Info = info;
            this.Owner = owner;
            this.Define = DataManager.Instance.Skills[(int)this.Owner.Define.Class][this.Info.Id];
            this.cd = 0;
        }

        public SkillResult CanCast(Creature target)
        {
            if (this.Define.CastTarget == Common.Battle.TargetType.Target)
            {
                if (target == null || target == this.Owner)
                    return SkillResult.InvalidTarget;

                int distance = this.Owner.Distance(target);
                if (distance > this.Define.CastRange)
                {
                    return SkillResult.OutOfRange;
                }
            }

            if (this.Define.CastTarget == Common.Battle.TargetType.Position && BattleManager.Instance.Position == null)
            {
                return SkillResult.InvalidTarget;
            }

            if (this.Owner.Attributes.MP < this.Define.MPCost)
            {
                return SkillResult.OutOfMP;
            }

            if (this.cd > 0)
            {
                return SkillResult.CoolDown;
            }

            return SkillResult.OK;
        }

        public void BeginCast(NDamageInfo damage) 
        {
            this.IsCasting = true;
            this.castTime = 0;
            this.skillTime = 0;
            this.cd = this.Define.CD;
            this.Damage = damage;
            this.Owner.PlayAnim(this.Define.SkillAnim);

            if(this.Define.CastTime > 0)
            {
                this.Status = SkillStatus.Casting;
            }
            else
            {
                this.Status = SkillStatus.Running;
            }
        }

        public void OnUpdate(float delta)
        {
            UpdateCD(delta);

            if(this.Status == SkillStatus.Casting)
            {
                this.UpdateCasting();
            }

            else if (this.Status == SkillStatus.Running)
            {
                this.UpdateSkill();
            }
        }

        private void UpdateCasting()
        {
            if(this.castTime < this.Define.CastTime)
            {
                this.castTime += Time.deltaTime;
            }
            else
            {
                this.castTime = 0;
                this.Status = SkillStatus.Running;
                Debug.LogFormat("Skill [{0}].UpdateCasting Finish", this.Define.Name);
            }
        }

        private void UpdateSkill()
        {
            this.skillTime += Time.deltaTime;

            if(this.Define.Duration > 0)
            {
                if(this.skillTime > this.Define.Interval * (this.Hit + 1))
                {
                    this.DoHit();
                }

                if(this.skillTime >= this.Define.Duration)
                {
                    this.Status = SkillStatus.None;
                    this.IsCasting = false;
                    Debug.LogFormat("Skill [{0}].UpdateSkill Finish", this.Define.Name);
                }
            }

            else if(this.Define.HitTimes != null && this.Define.HitTimes.Count > 0)
            {
                if(this.skillTime > this.Define.HitTimes[this.Hit])
                {
                    this.DoHit();
                }

                else
                {
                    this.Status = SkillStatus.None;
                    this.IsCasting = false;
                    Debug.LogFormat("Skill [{0}].UpdateSkill Finish", this.Define.Name);
                }
            }

        }
        
        private void DnHit()
        {
            List<NDamageInfo> damages;
            if(this.HitMap.TryGetValue(this.Hit, out damage))
            {
                DoHitDamages(damages);
            }

            this.Hit++;
        }

        private void UpdateCD(float delta)
        {
            if (this.cd > 0)
            {
                this.cd -= delta;
            }

            if (this.cd < 0)
            {
                this.cd = 0;
            }
        }

        internal void DoHit(int hitId, List<NDamageInfo> damages)
        {
            if (hitId <= this.Hit)
                this.HitMap[hitId] = damages;
            else
                DoHitDamages(damages);
        }

        internal void DoHitDamages(List<NDamageInfo> damages)
        {
            foreach(var dmg in damages)
            {
                Creature target = EntityManager.Instance.GetEntity(dmg.entityId) as Creature;
                if (target == null) continue;
                target.DoDamage(dmg);
            }
        }
    }
}