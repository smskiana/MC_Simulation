using _3C.Actors;
using StatSystems.Stats.Items;
using StatSystems.Store.Items;
using UnityEngine;

namespace Items
{
    public class Equipment : MonoBehaviour
    {
        [SerializeField]protected View view;
        [SerializeField]protected EquipmentStat stat;
        [SerializeField]protected Actor actor;
        public virtual void Init(EquipmentStat stat,Actor actor)
        {
            this.stat = stat;
            this.actor = actor;
        }
        public virtual void Init(EquipmentInfo info, Actor actor) => Init(info.GetNewItemStat() as EquipmentStat,actor);
    }
}
