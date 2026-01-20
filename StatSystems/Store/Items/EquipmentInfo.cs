using UnityEngine;
namespace StatSystems.Store.Items
{
    public class EquipmentInfo : ItemInfo
    {
        [SerializeField] private float maxDurability;
        [SerializeField] private float canAbrasion;
        public float MaxDurability { get => maxDurability;}
        public float CanAbrasion { get => canAbrasion;}
    }
}
