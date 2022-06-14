using Battle;
using Common.Battle;
using GameServer.Battle;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.AI
{
    internal class AIBase
    {
        private Monster Owner;
        private Creature Target;
        Skill normalSkill;
        private BattleState State;

        public AIBase(Monster owner)
        {
            this.Owner = owner;
            this.normalSkill = this.Owner.SkillMgr.NormalSkill;
        }

        internal void Update()
        {
            if(this.Owner.BattleState == BattleState.InBattle)
            {
                this.UpdateBattle();
            }
        }

        private void UpdateBattle()
        {
            if (this.Target == null)
            {
                this.Owner.BattleState = BattleState.Idle;
                return;
            }

            if (!TryCastSkill())
            {
                if(!TryCastNormal())
                {
                    FollowTarget();
                }
            }
        }

        private void FollowTarget()
        {
            int distance = this.Owner.Distance(this.Target);
            if(distance > normalSkill.Define.CastRange - 50)
            {
                this.Owner.MoveTo(this.Target.Position);
            }
            else
            {
                this.Owner.StopMove();
            }
        }


        private bool TryCastSkill()
        {

            if (this.Target != null)
            {
                BattleContext context = new BattleContext(this.Owner.Map.Battle)
                {
                    Target = this.Target,
                    Caster = this.Owner,
                };

                Skill skill = this.Owner.FindSkill(context, SkillType.Skill);
                if (skill != null)
                {
                    this.CastSkill(context, skill.Define.ID);
                    return true;
                }
            }

            return false;
        }

        private bool TryCastNormal()
        {
            if (this.Target != null)
            {
                BattleContext context = new BattleContext(this.Owner.Map.Battle)
                {
                    Target = this.Target,
                    Caster = this.Owner,
                };

                var result = normalSkill.CanCast(context);

                if (result == SkillResult.Ok)
                {
                    this.Owner.CastSkill(context, normalSkill.Define.ID);
                    return true;
                }

                if(result == SkillResult.OutOfRange)
                {
                    return false;
                }
            }

            return false;
        }

        internal void OnDamage(NDamageInfo damage, Creature source)
        {
            this.Target = source;
        }

        private Skill FindSkill(BattleContext context)
        {
            throw new NotImplementedException();
        }

        private void CastSkill(BattleContext context, object iD)
        {
            throw new NotImplementedException();
        }
    }
}
