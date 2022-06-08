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
    class Character : CharacterBase, IPostResponser
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
            base(new Core.Vector3Int(cha.MapPosX, cha.MapPosY, cha.MapPosZ), new Core.Vector3Int(100, 0, 0))
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
            this.Info.Ride = 0;
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

            // this.Guild = GuildManager.Instance.GetGuild(this.Data.GuildId);

            // this.Chat = new Chat(this);
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
