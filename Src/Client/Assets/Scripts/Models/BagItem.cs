using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    // summary : 1:18:16

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct BagItem
    {
        public ushort ItemId { get; internal set; }
        public ushort Count { get; internal set; }
        public static BagItem zero = new BagItem { ItemId = 0, Count = 0 };
        public BagItem(int itemId, int count)
        {
            this.ItemId = (ushort)itemId;
            this.Count = (ushort)count;
        }

        public static bool operator == (BagItem lhs, BagItem rhs)
        {
            return lhs.ItemId == rhs.ItemId && lhs.Count == rhs.Count;
        }

        public static bool operator != (BagItem lhs, BagItem rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary
        ///  Returns true if the objects are equal.
        /// </summary>
        public override bool Equals(object other)
        {
            if(other is BagItem)
            {
                return Equals((BagItem)other);
            }

            return false;
        }

        public bool Equals(BagItem other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return ItemId.GetHashCode() ^ ( Count.GetHashCode() << 2);
        }
    }
}
