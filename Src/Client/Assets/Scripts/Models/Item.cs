using Common.Data;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    public class Item
    {
        /* Function : tool item object*/

        // dfien item attributes
        public int Id;
        public int Count;
        public ItemDefine Define;
        public EquipDefine EquipInfo;

        public Item(NItemInfo item): this(item.Id, item.Count)
        {
        }
        

        // constructor
        // Note : client use the ItemInfo from network protoc
        //        while server use the TCharacterItem from db
        public Item(int id, int count)
        {
            this.Id = id;
            this.Count = count;
            DataManager.Instance.Items.TryGetValue(this.Id, out this.Define);
            DataManager.Instance.Equips.TryGetValue(this.Id,out this.EquipInfo);
        }

        // print imte information
        public override string ToString()
        {
            return string.Format("Item Id : [{0}] Count : {1}", this.Id, this.Count);
        }
    }

}