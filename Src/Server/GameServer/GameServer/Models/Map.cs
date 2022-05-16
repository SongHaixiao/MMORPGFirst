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

        Dictionary<int, MapCharacter> MapCharacters = new Dictionary<int, MapCharacter>();


        internal Map(MapDefine define)
        {
            this.Define = define;
        }

        internal void Update()
        {
        }

        /// <summary>
        /// Character Enter Game
        /// </summary>
        /// <param name="character"></param>
        internal void CharacterEnter(NetConnection<NetSession> conn, Character character)
        {   
            // send character enter response message to client

            Log.InfoFormat("CharacterEnter: Map:{0} characterId:{1}", this.Define.ID, character.Id);

            // Step01 : tell server you want to enter game
            character.Info.mapId = this.ID;     // mark the map id

            // create character enter game response net message for client
            // and give them value
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            message.Response.mapCharacterEnter.mapId = this.Define.ID;
            message.Response.mapCharacterEnter.Characters.Add(character.Info);

            // Step02 : tell everyone the information that character enter game

            // send player character enter game information to others online,
            foreach (var kv in this.MapCharacters)
            {
                message.Response.mapCharacterEnter.Characters.Add(kv.Value.character.Info);
                this.SendCharacterEnterMap(kv.Value.connection, character.Info); // tell others you are coming
            }
            
            // add character object into app
            this.MapCharacters[character.Id] = new MapCharacter(conn, character);

            // send character enter game responnse net mesage to clietn
            byte[] data = PackageHandler.PackMessage(message);
            conn.SendData(data, 0, data.Length);
        }


        /// <summary>
        /// Character Leave Game
        /// </summary>
        /// <param name="character"></param>
        internal void CharacterLeave(NCharacterInfo character)
        {
            Log.InfoFormat("CharacterEnter: Map:{0} characterId:{1}", this.Define.ID, character.Id);

            // remove character from dictionary
            this.MapCharacters.Remove(character.Id);

            // send player character leave game information to otehrs online
            foreach(var kv in this.MapCharacters)
            {
                // tell others you are leaving
                this.SendCharacterLeaveMap(kv.Value.connection, character);
            }
        }


        // send character enter map response message to client
        void SendCharacterEnterMap(NetConnection<NetSession> conn, NCharacterInfo character)
        {
            // create character enter map reposne net message
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();

            // fill values into response net message
            message.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            message.Response.mapCharacterEnter.mapId = this.Define.ID;
            message.Response.mapCharacterEnter.Characters.Add(character);

            // send response messaget to client
            byte[] data = PackageHandler.PackMessage(message);
            conn.SendData(data, 0, data.Length);
        }

        // send character leav map response message to client
        void SendCharacterLeaveMap(NetConnection<NetSession> conn, NCharacterInfo character)
        {
            // create character leave map response net message
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();

            // fill values into response net message
            message.Response.mapCharacterLeave = new MapCharacterLeaveResponse();
            message.Response.mapCharacterLeave.characterId = character.Id;

            byte[] data = PackageHandler.PackMessage(message);
            conn.SendData(data, 0, data.Length);
        }
    }
}
