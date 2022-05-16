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

        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Unsubscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
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
                if (User.Instance.CurrentCharacter.Id == cha.Id)
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

        private void OnMapCharacterLeave(object sender, MapCharacterLeaveResponse response)
        {
            
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
    }
}
