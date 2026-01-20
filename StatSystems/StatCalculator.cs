using Actors;
using StatSystems.Stats;
using StatSystems.Store;
using System.Collections.Generic;

namespace StatSystems
{
    public class StatCalculator
    {
        public StatSystem StatSystem {  get;private set; }
        protected ActorStat FrameDataStat;
        public StatCalculator(StatSystem statSystem,ActorInfo frameDataStat)
        {
            FrameDataStat = frameDataStat.GetNewActorStat();
            this.StatSystem = statSystem;
        }
        public ActorStat Calculator(IEnumerable<IEffectApply> effects)
        {
            StatSystem.ResetToBasicStat(FrameDataStat);
            foreach (IEffectApply effect in effects)
            {
                effect.Apply(FrameDataStat);
            }
            return FrameDataStat;
        }
    }
}
