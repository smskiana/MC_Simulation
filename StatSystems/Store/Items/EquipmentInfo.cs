using UnityEngine;
namespace StatSystems.Store.Items
{
    public class EquipmentInfo : ItemInfo
    {
        [SerializeField] private float maxDurability;
        [SerializeField] private bool canAbrasion;
        public float MaxDurability { get => maxDurability;}
        public bool CanAbrasion { get => canAbrasion;}
    }
}
