﻿using Common.Data;
using GameServer.Core;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entities
{
    /// <summary>
    /// Player Character Class
    /// </summary>
    class Character : CharacterBase, IPostResponser
    {
        public TCharacter Data;

        public ItemManager ItemManger;
        public QuestManager QuestManager;
        public StatusManger StatusManger;

        public Character(CharacterType type,TCharacter cha):
            base(new Core.Vector3Int(cha.MapPosX, cha.MapPosY, cha.MapPosZ),new Core.Vector3Int(100,0,0))
        {
            // loading character information while loading game

            // initialization character
            this.Data = cha;
            this.Id = cha.ID;
            this.Info = new NCharacterInfo();
            this.Info.Type = type;
            this.Info.Id = cha.ID;
            this.Info.EntityId = this.entityId;
            this.Info.Name = cha.Name;
            this.Info.Level = 10;//cha.Level;
            this.Info.ConfigId = cha.TID;
            this.Info.Class = (CharacterClass)cha.Class;
            this.Info.mapId = cha.MapID;
            this.Info.Gold = cha.Gold;
            this.Info.Entity = this.EntityData;
            this.Define = DataManager.Instance.Characters[this.Info.ConfigId];

            // initialization item
            this.ItemManger = new ItemManager(this);
            this.ItemManger.GetItemInfos(this.Info.Items);

            // initialization bag and add items to bag
            this.Info.Bag = new NBagInfo();
            this.Info.Bag.Unlocked = this.Data.Bag.Unlocked;
            this.Info.Bag.Items = this.Data.Bag.Items;

            // initialization equipments
            this.Info.Equips = this.Data.Equips;

            this.QuestManager = new QuestManager(this);
            this.QuestManager.GetQuestInfos(this.Info.Quests);

            this.StatusManger = new StatusManger(this);

            // this.FriendManager = new FriendManager(this);
            // this.FriendManager.GetFriendInfos(this.Info.Friends);
        }

        public long Gold
        {
            get { return this.Data.Gold; }
            set
            {
                if (this.Data.Gold == value)
                    return;

                this.StatusManger.AddGoldChange((int)(value - this.Data.Gold));
                this.Data.Gold = value;
            }
        }

        public void PostProcess(NetMessageResponse message)
        {
            if(this.StatusManger.HasStatus)
            {
                this.StatusManger.PostProcess(message);
            }
        }

        // time count when character leave
        public void Clear()
        {
            
        }
    }
}
