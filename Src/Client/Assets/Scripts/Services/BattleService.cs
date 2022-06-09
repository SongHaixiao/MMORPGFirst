using Entities;
using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using UnityEngine;

namespace Services
{
    class BattleService : Singleton<BattleService>, IDisposable
    {
        public void Init()
        {

        }

        public BattleService()
        {
            MessageDistributer.Instance.Subscribe<SkillCastResponse>(this.OnSkillCast);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<SkillCastResponse>(this.OnSkillCast);
        }

        public void SendSkillCast(int skillId, int casterId, int targetId, NVector3 position)
        {
            if (position == null) position = new NVector3();
            Debug.LogFormat("SendSkillCast :: skill : {0} caster : {1} target : {2}  pos : {3}", skillId, casterId, targetId, position.ToString());

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.skillCast.castInfo = new NSkillCastInfo();

            message.Request.skillCast.castInfo.skillId = skillId;
            message.Request.skillCast.castInfo.casterId = casterId;
            message.Request.skillCast.castInfo.targetId = targetId;
            message.Request.skillCast.castInfo.Position = position;

            NetClient.Instance.SendMessage(message);
        }

        public void OnSkillCast(object sender, SkillCastResponse message)
        {
            Debug.LogFormat("OnSkillCast :: skill : {0}, caster : {1}, target : {2}, pos : {3}, result : {4}", message.castInfo.skillId, message.castInfo.casterId, message.casterInfo.targetId, message.casterInfo.position, message.casterInfo.result);
            if (message.Result == Result.Success)
            {
                Creature caster = EntityManager.Instance.GetEntity(message.castInfo.casterId) as Creature;
                if (caster != null)
                {
                    Creature target = EntityManager.Instance.GetEntity(message.castInfo.targetId) as Creature;
                    caster.CastSkill(message.castInfo.skillId, target, message.castInfo.Position);
                }
            }
            else
            {
                ChatManager.Instance.AddSystemMessage(message.Errormsg);
            }
        }
    }
}