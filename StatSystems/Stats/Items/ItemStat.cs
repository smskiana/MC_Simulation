using System;
using StatSystems.Store.Items;
namespace StatSystems.Stats.Items
{  
    [System.Serializable]
    public class ItemStat : RealTimeData, IComparable<ItemStat>, IEquatable<ItemStat>
    {
        public ItemStat(ItemInfo itemInfo) : base(itemInfo)
        {
            
        }
        public ItemInfo ItemInfo { get => info as ItemInfo ;}
        public virtual int CompareTo(ItemStat info)
        {
            if(ItemInfo == null) throw new ArgumentNullException(nameof(ItemInfo));
            return ItemInfo.CompareTo(info.ItemInfo);
        }
        //静态道具数据（只读）直接传引用
        public override RealTimeData CloneNewInstance()
        {
           return this;
        }
        public bool Equals(ItemStat other)
        {
            if (ItemInfo == null) throw new ArgumentNullException(nameof(ItemInfo));
            if (other == null) return false;
            return ItemInfo.Equals(other.ItemInfo);
        }
        public virtual bool Equals(int SourceId)
        {
            return ItemInfo.ID == SourceId;
        }
        public override void Reset()
        {

        }
    }
}