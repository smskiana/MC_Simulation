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
    }
}
