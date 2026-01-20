using StatSystems;
using StatSystems.Stats;
using UnityEngine;

namespace Effects
{
    [CreateAssetMenu(fileName = "DefenceAddTimed",menuName = "Effect/Timed/DefenceAdd")]
    public class DefenceAddTimed:Effect,IEffectApply
    {
        [SerializeField] float Defenceadd;
        [SerializeField] float Order;

        public virtual void Apply(ActorStat stat)
        {
            stat.defence += Defenceadd;
        }

        public virtual float GetApplyOrder() => Order;
    }
}
