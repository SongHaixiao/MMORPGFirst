﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkillBridge.Message;
using UnityEngine;
using Common.Battle;
using Common.Data;
using Managers;

namespace Entities
{
    public class Character : Entity
    {
        public NCharacterInfo Info;

        public CharacterDefine Define;
        public Attributes Attributes;

        public int Id
        {
            get { return this.Info.Id; }
        }

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

        public Character(NCharacterInfo info) : base(info.Entity)
        {
            this.Info = info;
            this.Define = DataManager.Instance.Characters[info.ConfigId];
            this.Attributes = new Attributes();
            var equips = EquipManager.Instance.GetEquipDefines();
            this.Attributes.Init(this.Define, this.Info.Level, equips, this.Info.attrDynamic);
        }

        public void MoveForward()
        {
            Debug.LogFormat("MoveForward");
            this.speed = this.Define.Speed;
        }

        public void MoveBack()
        {
            Debug.LogFormat("MoveBack");
            this.speed = -this.Define.Speed;
        }

        public void Stop()
        {
            Debug.LogFormat("Stop");
            this.speed = 0;
        }

        public void SetDirection(Vector3Int direction)
        {
            Debug.LogFormat("SetDirection:{0}", direction);
            this.direction = direction;
        }

        public void SetPosition(Vector3Int position)
        {
            Debug.LogFormat("SetPosition:{0}", position);
            this.position = position;
        }
    }
}
