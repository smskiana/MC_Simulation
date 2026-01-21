using StatSystems.Store.Items;
namespace StatSystems.Stats.Items
{
    public class EquipmentStat : ItemStat
    {
        public float durability;
        public EquipmentInfo EquipmentInfo { get => info as EquipmentInfo; }
        public EquipmentStat(EquipmentInfo itemInfo) : base(itemInfo)
        {
            durability = itemInfo.MaxDurability;
        }
        public override RealTimeData CloneNewInstance()
        {
            return new EquipmentStat(EquipmentInfo)
            {
                durability = durability,
            };
        }
        public override void Reset()
        {
            base.Reset();
            durability = EquipmentInfo.MaxDurability;
        }
        public override int CompareTo(ItemStat info)
        {
            int baseValue = base.CompareTo(info);
            if(info is  EquipmentStat stat)
            {
                if (baseValue != 0) return baseValue;
                else return durability.CompareTo(stat.durability);
            } 
            return baseValue;
        }
        public override bool Equals(ItemStat other)
        {        
            if (other is EquipmentStat stat)
            {
                bool value = base.Equals(other);
                if (!value) return false;
                return durability==stat.durability;
            }
            else
            {
                return false;
            }
        }

    }
}
