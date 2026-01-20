using UnityEngine;
using StatSystems;
using StatSystems.Stats;

namespace Effects
{
    [CreateAssetMenu(fileName = "DefenceMultiplyTimed", menuName = "Effect/Timed/DefenceMultiply")]
    public class DefenceMultiplyTimed : Effect, IEffectApply
    {
        [SerializeField] float Order;
        [SerializeField] float rate;
        public virtual void Apply(ActorStat effect)
        {
            effect.defence *=rate;
        }
        public virtual float GetApplyOrder()=>Order;
    }
}
