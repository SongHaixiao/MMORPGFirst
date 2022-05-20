using Common.Data;
using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Managers
{
    public class ItemManger : Singleton<ItemManger>
    {
       // item manger to control local memory
       public Dictionary<int, Item> Items = new Dictionary<int, Item>();


       // init for Items
       internal void Init(List<NItemInfo> items)
        {
            // clear item manger
            this.Items.Clear();
            
            // add items datd from network
            // into item manger
            // Note : Client just for network protoc
            //        Server just for db
            foreach(var info in items)
            {
                Item item = new Item(info);
                this.Items.Add(item.Id, item);

                Debug.LogFormat("ItemManger : Init [{0}]", item);
            }
        }

        // methods will be completed in the future

        // client doesn't have privilege to do
        // the operation of adding and removing items
        // so method to operate items in client is passived
        // afeter item updating sended from server
        public ItemDefine GetItem(int itemId)
        {
            return null;
        }

        public bool UseItem(int itemId)
        {
            return false;
        }

        public bool UseItem(ItemDefine item)
        {
            return false;
        }
    }
}
