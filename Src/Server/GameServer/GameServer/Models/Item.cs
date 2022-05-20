using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    class Item
    {
        // define attributions
        TCharacterItem dbItem;

        public int ItemID;
        public int Count;

        // constructor
        public Item(TCharacterItem item)
        {
            this.dbItem = item;
            this.ItemID = (short)item.ItemID;
            this.Count = (short)item.ItemCount;
        }

        // increase number of item when item is added
        public void Add(int count)
        {
            // update local item count
            this.Count += count;

            // update db item count
            this.dbItem.ItemCount = this.Count;
        }

        // decrease number of item when item is deleted
        public void Remove(int count)
        {
            // update local item count
            this.Count -= count;

            // update db item count
            this.dbItem.ItemCount = this.Count;
        }

        // check the item whther is used
        public bool Use(int count = 1)
        {
            return false;
        }

        // print the Item information
        public override string ToString()
        {
            return string.Format("Item Id : {0}, Count : {1}", this.ItemID, this.Count);
        }
    }
}
