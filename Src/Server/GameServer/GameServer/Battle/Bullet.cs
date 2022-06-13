﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Battle;
using Common;
using GameServer.Entities;
using SkillBridge.Message;

namespace GameServer.Battle
{
    public class Bullet
    {
        private Skill skill;
        NSkillHitInfo hitInfo;
        bool TimeMode = true;
        float duration = 0;
        float flyTime = 0;
        public bool Stoped = false;

        public Bullet(Skill skill, Creature target, NSkillHitInfo hitInfo)
        {
            this.skill = skill;
            this.hitInfo = hitInfo;
            int distance = skill.Owner.Distance(target);
            if(TimeMode)
            {
                duration = distance / this.skill.Define.BulletSpeed;
            }

            Log.InfoFormat("Bullet [{0}].CastBullet [{1}] Target : {2} Distance : {3} Time : {4}", this.skill.Define.Name, this.skill.Define.BulletResource, target.Name, distance, duration);
        }

        public void Update()
        {
            if (Stoped) return;

            if (TimeMode)
            {
                this.UpdateTime();
            }
            else
            {
                this.UpdatePos();
            }

        }

        private void UpdateTime()
        {
            this.flyTime += Time.deltaTime;

            if (this.flyTime > duration)
            {
                this.hitInfo.isBullet = true;
                this.skill.DoHit(this.hitInfo);
                this.Stoped = true;
            }
        }

        private void UpdatePos()
        {
            //int distance = skill.Owner.Distance(target);
            //if(distance > 50)
            //{
            //    UpdatePos += speed * Time.deltaTime;
            //}
            //else
            //{
            //    this.hitInfo.isBullet = true;
            //    this.skill.DoHit(this.hitInfo);
            //    this.stoped = true;
            //}
        }


    }
}

