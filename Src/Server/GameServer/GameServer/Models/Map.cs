using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Message;

using Common;
using Common.Data;

using Network;
using GameServer.Managers;
using GameServer.Entities;
using GameServer.Services;

namespace GameServer.Models
{
    class Map
    {
        internal class MapCharacter
        {
            public NetConnection<NetSession> connection;
            public Character character;

            public MapCharacter(NetConnection<NetSession> conn, Character cha)
            {
                this.connection = conn;
                this.character = cha;
            }
        }

        public int ID
        {
            get { return this.Define.ID; }
        }
        internal MapDefine Define;

        // character in map, CharacterID as Key
        Dictionary<int, MapCharacter> MapCharacters = new Dictionary<int, MapCharacter>();


        // spawn manager
        SpawnManager SpawnManager = new SpawnManager();

        // monster manager
        public MonsterManager MonsterManager = new MonsterManager();

        internal Map(MapDefine define)
        {
            this.Define = define;
            this.SpawnManager.Init(this);
            this.MonsterManager.Init(this);
        }

        internal void Update()
        {
            SpawnManager.Update();
        }

        /// <summary>
        /// Character Enter Game
        /// </summary>
        /// <param name="character"></param>
        internal void CharacterEnter(NetConnection<NetSession> conn, Character character)
        {   
            // send character enter response message to client

            Log.InfoFormat("CharacterEnter: Map : {0} characterId : {1}", this.Define.ID, character.Id);

            // Step01 : tell server you want to enter game
            character.Info.mapId = this.ID;     // mark the map id
            this.MapCharacters[character.Id] = new MapCharacter(conn, character);

            // create character enter game response net message for client
            // and give them value
            conn.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            conn.Session.Response.mapCharacterEnter.mapId = this.Define.ID;


            // Step02 : tell everyone the information that character enter game

            // send player character enter game information to others online,
            // add character object into app
            foreach (var kv in this.MapCharacters)
            {
                conn.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.character.Info);
                if(kv.Value.character != character)
                {
                    this.AddCharacterEnterMap(kv.Value.connection, character.Info); // tell others you are coming
                }
               
            }

            // Step03 : add monster in map
            foreach(var kv in this.MonsterManager.Monsters)
            {
                conn.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.Info);
            }

           
            // send character / monster enter game responnse net mesage to clietn
            conn.SendResponse();
        }

        /// <summary>
        /// Character Leave Game
        /// </summary>
        /// <param name="character"></param>
        internal void CharacterLeave(Character character)
        {
            Log.InfoFormat("CharacterEnter: Map:{0} characterId:{1}", this.Define.ID, character.Id);

            // send player character leave game information to otehrs online
            foreach(var kv in this.MapCharacters)
            {
                // tell others you are leaving
                this.SendCharacterLeaveMap(kv.Value.connection, character);
            }

            // remove character from dictionary
            this.MapCharacters.Remove(character.Id);
        }


        // send character enter map response message to client
        void AddCharacterEnterMap(NetConnection<NetSession> conn, NCharacterInfo character)
        {
            if(conn.Session.Response.mapCharacterEnter == null)
            {
                // create character enter map response net message,
                // fill values into response net message
                conn.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
                conn.Session.Response.mapCharacterEnter.mapId = this.Define.ID;
            }

            conn.Session.Response.mapCharacterEnter.Characters.Add(character);
            conn.SendResponse();
        }

        // send character leave map response message to client
        void SendCharacterLeaveMap(NetConnection<NetSession> conn, Character character)
        {
            conn.Session.Response.mapCharacterLeave = new MapCharacterLeaveResponse();
            conn.Session.Response.mapCharacterLeave.entityId = character.entityId;

            conn.SendResponse();
        }

        // update entity
        internal void UpdateEntity(NEntitySync entity)
        {
            foreach(var kv in this.MapCharacters)
            {
                if(kv.Value.character.entityId == entity.Id)
                {
                    kv.Value.character.Position = entity.Entity.Position;
                    kv.Value.character.Direction = entity.Entity.Direction;
                    kv.Value.character.Speed = entity.Entity.Speed;
                }
                else
                {
                    MapService.Instance.SendEntityUpdate(kv.Value.connection, entity);
                }
            }
        }

        // monster enter map
        internal void MonsterEnter(Monster monster)
        {
            Log.InfoFormat("MonsterEnter : Map : {0} monsterId : {1}", this.Define.ID, monster.Id);
            foreach(var kv in this.MapCharacters)
            {
                this.AddCharacterEnterMap(kv.Value.connection, monster.Info);
            }
        }
    }
}
