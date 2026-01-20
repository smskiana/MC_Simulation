using StatSystems.Store.Items;

namespace StatSystems.Stats.Items
{
    public class GunStat : RangedWeaponStat
    {
        public int AnmoCount;
        public GunInfo GunInfo { get => base.info as GunInfo; }
        public GunStat(GunInfo itemInfo) : base(itemInfo)
        {
            AnmoCount = itemInfo.AnmoContain;
        }
    }
}
