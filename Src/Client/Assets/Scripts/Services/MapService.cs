using System;
using Network;
using UnityEngine;

using Common.Data;
using SkillBridge.Message;
using Models;
using Managers;

namespace Services
{
    class MapService : Singleton<MapService>, IDisposable
    {

        public int CurrentMapId = 0;
        public MapService()
        {
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
            MessageDistributer.Instance.Subscribe<MapEntitySyncResponse>(this.OnMapEntitySync);

        }


        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Unsubscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
            MessageDistributer.Instance.Unsubscribe<MapEntitySyncResponse>(this.OnMapEntitySync);
        }

        public void Init()
        {

        }

        // character enter map method
        private void OnMapCharacterEnter(object sender, MapCharacterEnterResponse response)
        {
            Debug.LogFormat("OnMapCharacterEnter:Map:{0} Count:{1}", response.mapId, response.Characters.Count);

            // travel every character's data info from server
            foreach (var cha in response.Characters)
            {
                // current character change map
                if (User.Instance.CurrentCharacter == null ||User.Instance.CurrentCharacter.Id == cha.Id)
                {
                    User.Instance.CurrentCharacter = cha;
                }

                // not current character, add it int character manager
                CharacterManager.Instance.AddCharacter(cha);
            }

            // if current map is not response map,
            // change it to response map
            if (CurrentMapId != response.mapId)
            {
                this.EnterMap(response.mapId); // enter response map
                this.CurrentMapId = response.mapId; // change current map id
            }
        }

        // character leave map method
        private void OnMapCharacterLeave(object sender, MapCharacterLeaveResponse response)
        {
            Debug.LogFormat("OnMapCahracterLeave : CharID : {0}", response.characterId);

            // leaving chracter is others, remove others chracter
            if (response.characterId != User.Instance.CurrentCharacter.Id)
                CharacterManager.Instance.RemoveCharacter(response.characterId);

            // leaving chracter is player self, clear all characters in the map
            else
                CharacterManager.Instance.Clear();
        }

        // enter map 
        private void EnterMap(int mapId)
        {
            // map resource is existed, load this map
            if (DataManager.Instance.Maps.ContainsKey(mapId))
            {
                // get map data from db
                MapDefine map = DataManager.Instance.Maps[mapId];

                // geive map data to current map data in user,
                // when enter map character
                User.Instance.CurrentMapData = map;

                // load map resource
                SceneManager.Instance.LoadScene(map.Resource);
            }

            // map resource is not exited.
            else
                Debug.LogErrorFormat("EnterMap: Map {0} not existed", mapId);
        }

        // send map entity sync request to server
        internal void SendMapEntitySync(EntityEvent entityEvent, NEntity entity)
        {
            Debug.LogFormat("MapEntityUpdateRequest : ID : {0} Pos : {1} DIR : {2} SPD : {3}", entity.Id, entity.Position.String(), entity.Direction.String(), entity.Speed);

            // create sync map entity request net message
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapEntitySync = new MapEntitySyncRequest();
            message.Request.mapEntitySync.entitySync = new NEntitySync()
            {
                // fill infor for sync map entity request net message
                Id = entity.Id,
                Event = entityEvent,
                Entity = entity
            };

            // send sync map eneity request net message to server
            NetClient.Instance.SendMessage(message);
        }

        // sync map entity evetn
        private void OnMapEntitySync(object sender, MapEntitySyncResponse response)
        {
            //System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //sb.AppendFormat("MapEntityUpdateResponse : Entities : {0}", response.entitySyncs.Count);
            //sb.AppendLine();
            foreach(var entity in response.entitySyncs)
            {
                EnityManager.Instance.OnEntitySync(entity);
                //sb.AppendFormat("   [{0}] event : {1} enttiy : {2}", entity.Id, entity.Event, entity.Entity.String());
                //sb.AppendLine();
            }
            //Debug.Log(sb.ToString());
        }
    }
}
