using Common;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class ItemManager
    {
        /* Function : Control the items behaviour.*/

        // define attributes
        Character Owner;
        public Dictionary<int, Item> Items = new Dictionary<int, Item>();

        // constructor
        public ItemManager(Character owner)
        {
            this.Owner = owner;

            foreach(var item in owner.Data.Items)
            {
                this.Items.Add(item.ItemID, new Item(item));
            }
        }

        // item using, default number is 1
        public bool UseItem(int itemId, int count = 1)
        {
            Log.InfoFormat("Character [{0}] UserItem [{1} : {2}]", this.Owner.Data.ID, itemId, count);

            Item item = null;

            // check the itemId wheter existed in Items Manager,
            // and copy to item if exited at the same time
            if(this.Items.TryGetValue(itemId, out item))
            {
                // item numsber is not enough retunr
                if (item.Count < count)
                    return false;

                // TO DO : Using logic will be added in the furture
                // .....

                // remove item after using
                item.Remove(count);

                // Note : when item acount is 0, we can don't delete form db
                // to decrease db operation cost

                return true;
            }
            return false;
        }

        // check item whether is in Items
        public bool HasItem(int itemId)
        {
            Item item = null;

            // item is existed, return tures
            if(this.Items.TryGetValue(itemId, out item))
                    return item.Count > 0;

            // item is not existed, return false
            return false;
        }

        // get the item
        public Item GetItem(int itemId)
        {
            Item item = null;
            this.Items.TryGetValue(itemId, out item);
            Log.InfoFormat("Character [{0}] Get Item [{1} : {2}]", this.Owner.Data.ID, itemId, item);
            return item;
        }

        // add item
        public bool AddItem(int itemId, int count)
        {
            Item item = null;

            // if item is existed before, add its count
            if(this.Items.TryGetValue(itemId, out item))
            {
                item.Add(count);
            }
            
            // if item is not existed before,
            // create a new item data table
            // to insert it into DB and Items manager
            else
            {
                // create a new item data
                TCharacterItem dbItem = new TCharacterItem();
                dbItem.CharacterID = Owner.Data.ID;
                dbItem.Owner = Owner.Data;
                dbItem.ItemID = itemId;
                dbItem.ItemCount = count;

                // insert it into db
                Owner.Data.Items.Add(dbItem);

                // insert it into Items mangers
                item = new Item(dbItem);
                this.Items.Add(itemId, item);
            }

            // set item status
            this.Owner.StatusManger.AddItemChange(itemId, count, StatusAction.Add);

            Log.InfoFormat("Character [{0}] Add Item : [{1}] addCount : [{2}]", this.Owner.Data.ID, item, count);
            
            return true;
        }

        // remove item
        public bool RemoveItem(int itemId, int count)
        {
            // item is not existed in Itmes manager
            if(!this.Items.ContainsKey(itemId))
            {
                return false;
            }

            // get the item form item manger
            Item item = this.Items[itemId];

            // item db count less than local,
            // error, return false
            if(item.Count < count)
                return false;

            // rremove item
            item.Remove(count);

            // set item status
            this.Owner.StatusManger.AddItemChange(itemId, count, StatusAction.Delete);

            Log.InfoFormat("Character [{0}] Remove Item : [{1}] removeCount : [{2}]", this.Owner.Data.ID, item, count);
            
            return true;
        }

        // change item memory data into network data
        public void GetItemInfos(List<NItemInfo> list)
        {
            // travels item in Items manager
            foreach(var item in this.Items)
            {
                // add each item infor to network Item info message
                list.Add(new NItemInfo() { Id = item.Value.ItemID, Count = item.Value.Count });
            }
        }
    } 
}
