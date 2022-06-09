using Common;
using Common.Data;
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
    class Character : Creature, IPostResponser
    {
        public TCharacter Data;

        public ItemManager ItemManger;
        public QuestManager QuestManager;
        public StatusManger StatusManger;
        // public FriendManger FriendManger;

        // public Team team;
        // public double TeamUpdateTS;

        // public Guild Guild;
        // public double GuildUpdateTS;

        // public Chat Chat;

        public Character(CharacterType type, TCharacter cha) :
            base(type, cha.TID, cha.Level, new Core.Vector3Int(cha.MapPosX, cha.MapPosY, cha.MapPosZ), new Core.Vector3Int(100, 0, 0))
        {
            // loading character information while loading game

            // initialization character
            this.Id = cha.ID;
            this.Info.Name = cha.Name;
            this.Info.Id = cha.ID;
            this.Info.Exp = cha.Exp;
            this.Info.Class = (CharacterClass)cha.Class;
            this.Info.mapId = cha.MapID;
            this.Info.Gold = cha.Gold;
            this.Info.Ride = 0;

            // initialization item
            this.Data = cha;
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

            // this.Guild = GuildManager.Instance.GetGuild(this.Data.GuildId);

            // this.Chat = new Chat(this);

            this.Info.attrDynamic = new NAttributeDynamic();
            this.Info.attrDynamic.Hp = cha.HP;
            this.Info.attrDynamic.Mp = cha.MP;
        }

        internal void AddExp(int exp)
        {
            this.Exp = exp;
            this.CheckLevelUp();
        }

        void CheckLevelUp()
        {
            long needExp = (long)Math.Pow(this.Level, 3) * 10 + this.Level * 40 + 50;
            if(this.Exp > needExp)
            {
                this.LeaveUp();
            }
        }

        void LeaveUp()
        {
            this.Level += 1;
            Log.InfoFormat("Character[{0} : {1}] LevelUp : {2}", this.Id, this.Info.Name, this.Level);
            CheckLevelUp();
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
                this.Info.Gold = value;
            }
        }

        public long Exp
        {
            get { return this.Data.Exp; }
            private set
            {
                if (this.Data.Exp == value)
                    return;
                this.StatusManger.AddExpChange((int)(value - this.Data.Exp));
                this.Data.Exp = value;
                this.Data.Exp = value;
            }
        }

        public int Level
        {
            get { return this.Data.Level; }
            private set
            {
                if (this.Data.Level == value)
                    return;
                this.StatusManger.AddLevelUp((int)(value - this.Data.Level));
                this.Data.Level = value;
                this.Data.Level = value;
            }
        }

        public int Ride
        {
            get { return this.Info.Ride; }
            set
            {
                if (this.Info.Ride == value)
                    return;
                this.Info.Ride = value;
            }
        }

        public void PostProcess(NetMessageResponse message)
        {
            Log.InfoFormat("PostProcess > Character : characterID : {0} : {1}", this.Id, this.Info.Name);

            // Friend post process
            // Team post process
            // Guild post process

            if (this.StatusManger.HasStatus)
            {
                this.StatusManger.PostProcess(message);
            }
        }

        // time count when character leave
        public void Clear()
        {
            // Friend Manger
        }

        public NCharacterInfo GetBasicInfo()
        {
            return new NCharacterInfo()
            {
                Id = this.Id,
                Name = this.Info.Name,
                Class = this.Info.Class,
                Level = this.Info.Level
            };
        }
    }
}
