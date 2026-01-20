using StatSystems.Stats.Items;
using UnityEngine;
namespace StatSystems.Store.Items
{
    [CreateAssetMenu(fileName = "Gun", menuName = "Game/Info/Gun")]
    public class GunInfo : RangedWeaponInfo
    {
        [SerializeField] private int anmoContain;
        [SerializeField] private AnmoInfo useAnmo;
        [SerializeField] private float fireSpeed = 4;
        [SerializeField] private bool needAnmo = false;

        public int AnmoContain { get => anmoContain;}
        public AnmoInfo UseAnmo { get => useAnmo;}
        public float FireSpeed { get => fireSpeed;}

        public override ItemStat GetNewItemStat() => new GunStat(this);    
    }
}
