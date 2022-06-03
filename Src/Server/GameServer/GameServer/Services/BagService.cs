using Common;
using GameServer.Entities;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class BagService : Singleton<BagService>
    {
        /*Function : accept Bag Saving Request from Client.*/

        public BagService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<BagSaveRequest>(this.OnBagSave);
        }

        public void Init()
        {

        }

        // method for bag saving request
        void OnBagSave(NetConnection<NetSession> sender, BagSaveRequest request)
        {
            // get character data from session
            Character character = sender.Session.Character;

            // print log
            Log.InfoFormat("BagSaveRequest : character : {0} , Unlocked : {1}", character.Id, request.BagInfo.Unlocked);

            // bag save request's BagInfo is available
            if(request.BagInfo != null)
            {
                // set items data in bag from network into db
                character.Data.Bag.Items = request.BagInfo.Items;

                // save db
                DBService.Instance.Save();
            }
        }
    }
}
