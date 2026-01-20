using UnityEngine;

namespace Items
{
    public class RangedWeapon : Weapon
    {
        [SerializeField] protected Transform shotpos;
        [SerializeField] private bool CanStop;
       
    }
}
