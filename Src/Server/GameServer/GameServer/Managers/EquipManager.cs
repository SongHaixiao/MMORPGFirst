using Common;
using GameServer.Entities;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class EquipManager : Singleton<EquipManager>
    {
        // waer equipment
        public Result EquipItem(NetConnection<NetSession> sender, int slot, int itemId, bool isEquip)
        {
            // get character
            Character character = sender.Session.Character;

            // check character whether existed in character manager
            // if no, failed to void cheating program
            if (!character.ItemManger.Items.ContainsKey(itemId))
                return Result.Failed;

            // character is exited, then update equip information
            UpdateEquip(character.Data.Equips, slot, itemId, isEquip);

            // save db
            DBService.Instance.Save();

            // return wear success
            return Result.Success;
        }

        // update equipment 
        unsafe void UpdateEquip(byte[] equipData, int slot, int itemId, bool isEquip)
        {
            fixed(byte* pt = equipData)
            {
                int* slotid = (int*)(pt+slot*sizeof(int));
                if (isEquip)
                    *slotid = itemId;
                else
                    *slotid = 0;
            }
        }
    }
}
