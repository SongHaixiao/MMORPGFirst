using Common.Data;
using Models;
using Services;
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

            StatusService.Instance.RegisterStatusNofity(StatusType.Item, OnItemNotify);
        }

        bool OnItemNotify(NStatus status)
        {
            if(status.Action == StatusAction.Add)
            {
                this.AddItem(status.Id, status.Value);
            }

            if(status.Action == StatusAction.Delete)
            {
                this.RemoveItem(status.Id, status.Value);
            }

            return true;
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

        void AddItem(int itemId, int count)
        {
            Item item = null;
            if(this.Items.TryGetValue(itemId, out item))
            {
                item.Count += count;
            }

            else
            {
                item = new Item(itemId, count);
                this.Items.Add(itemId, item);
            }

            BagManager.Instance.AddItem(itemId, count);
        }

        void RemoveItem(int itemId, int count)
        {
            if(!this.Items.ContainsKey(itemId))
                return;

            Item item = this.Items[itemId];
            
            if(item.Count < count)
                return;

            item.Count -= count;

            BagManager.Instance.RemoveItem(itemId, count);
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
