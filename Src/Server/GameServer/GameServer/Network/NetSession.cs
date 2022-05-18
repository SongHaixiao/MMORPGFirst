using System;
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
    class NetSession
    {
        public TUser User { get; set; }
        public Character Character { get; set; }
        public NEntity Entity { get; set; }

        // when network is disconnected, clear data in Session
        // to avoid re-load previous character
        internal void Disconnected()
        {
           
            // Character is not null after network is disconnected,
            // clear Character data via UserService 
            if (this.Character != null)
                UserService.Instance.CharacterLeave(this.Character);
        }
    }
}
