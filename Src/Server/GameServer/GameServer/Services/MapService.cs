using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class MapService : Singleton<MapService>
    {
        public MapService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapEntitySyncRequest>(this.OnMapEntitySync);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapTeleportRequest>(this.OnMapTeleport);
        }

        public void Init()
        {
            // initialization map manager
            MapManager.Instance.Init();
        }

        // map entity sync
        private void OnMapEntitySync(NetConnection<NetSession> sender, MapEntitySyncRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnMapEntitySync: characterID:{0}:{1} Entity.Id:{2} Evt:{3} Entity:{4}", character.Id, character.Info.Name, request.entitySync.Id, request.entitySync.Event, request.entitySync.Entity.String());

            MapManager.Instance[character.Info.mapId].UpdateEntity(request.entitySync);
        }

        // send entity update response to client
        public void SendEntityUpdate(NetConnection<NetSession> conn, NEntitySync entity)
        {
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();

            message.Response.mapEntitySync = new MapEntitySyncResponse();
            message.Response.mapEntitySync.entitySyncs.Add(entity);

            byte[] data = PackageHandler.PackMessage(message);
            conn.SendData(data, 0, data.Length);
        }

        // map teleporter
        private void OnMapTeleport(NetConnection<NetSession> sender, MapTeleportRequest request)
        {
            // get character object
            Character character = sender.Session.Character;
            
            Log.InfoFormat("OnMapTeleport : characterID : {0} : {1} TeleporterId : {2}", character.Id, character.Data, request.teleporterId);


            // teleporter going to is not exited, error
            if(!DataManager.Instance.Teleporters.ContainsKey(request.teleporterId))
            {
                Log.WarningFormat("Souce TeleporterID : [{0}] not existed !", request.teleporterId);
                return;
            }

            // get the teleporter data from db
            TeleporterDefine source = DataManager.Instance.Teleporters[request.teleporterId];

            // LinkTo is unava or is not exited in db, error
            if(source.LinkTo == 0 || !DataManager.Instance.Teleporters.ContainsKey(source.LinkTo))
            {
                Log.WarningFormat("Source TeleporterID [{0}] LinkTo ID [{1}] not exited !", request.teleporterId, source.LinkTo);
                return;
            }

            // teleporter is available
            // get target teleporter from db
            TeleporterDefine target = DataManager.Instance.Teleporters[source.LinkTo];

            // character leave current map
            MapManager.Instance[source.MapID].CharacterLeave(character);

            // set character staring position and direction before getting in target teleporter
            character.Position = target.Position;
            character.Direction = target.Direction;

            // load target map
            MapManager.Instance[target.MapID].CharacterEnter(sender, character);
        }

    }
}
