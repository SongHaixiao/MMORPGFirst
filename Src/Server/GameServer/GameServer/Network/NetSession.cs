﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameServer;
using GameServer.Entities;
using GameServer.Services;
using SkillBridge.Message;

namespace Network
{
    class NetSession : INetSession
    {
        public TUser User { get; set; }
        public Character Character { get; set; }
        public NEntity Entity { get; set; }
        public IPostResponser PostResponser { get; set; }

        // when network is disconnected, clear data in Session
        // to avoid re-load previous character
        internal void Disconnected()
        {
            this.PostResponser = null;

            // Character is not null after network is disconnected,
            // clear Character data via UserService 
            if (this.Character != null)
                UserService.Instance.CharacterLeave(this.Character);
        }

        NetMessage response;

        public NetMessageResponse Response
        {
            get
            {
                if (response == null)
                {
                    response = new NetMessage();
                }

                if (response.Response == null)
                {
                    response.Response = new NetMessageResponse();
                }

                return response.Response;
            }
        }

        public byte[] GetResponse()
        {
            if (response != null)
            {
                if (PostResponser != null)
                    this.PostResponser.PostProcess(Response);

                byte[] data = PackageHandler.PackMessage(response);
                response = null;
                return data;
            }

            return null;
        }
    }
}
