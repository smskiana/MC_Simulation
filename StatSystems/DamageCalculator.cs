using StatSystems.Stats;
using StatSystems.Store;
using System.Collections.Generic;
using UnityEngine;

namespace StatSystems
{
    public class DamageCalculator
    {
        private readonly ActorStat BasicStat;
        private readonly StatSystem system;
        public ActorStat FrameDataStat => BasicStat.CloneNewInstance() as ActorStat;    
        public DamageCalculator(StatSystem system,ActorInfo actorInfo)
        {
            this.system = system;
            BasicStat = actorInfo.GetNewActorStat();
        }
        public ActorStat SetDamage(DamagePackage package,ActorStat ThisFrameStat, IEnumerable<IModifyDamage> effects)
        {
            var buf = ModifyDamage(package,effects);
            buf = ModifyDamage(buf, ThisFrameStat);
            system.ResetToBasicStat(BasicStat);
            package.Apply(BasicStat);
            return BasicStat;
        }
        public static ActorStat PredictDamage(DamagePackage dmg, ActorStat stat, IEnumerable<IModifyDamage> effects)
        {
            var copy = stat.CloneNewInstance() as ActorStat;
            var result = ModifyDamage(dmg, effects);
            result = ModifyDamage(result, copy);
            result.Apply(stat);
            return copy;
        }
        public static DamagePackage ModifyDamage(DamagePackage dmg,IEnumerable<IModifyDamage> effects)
        {
            // 2. 遍历效果加成（比如易伤、减伤）
            foreach (var effect in effects)
            {
               dmg=effect.ModifyDamage(dmg);
            }
           
            return dmg; 
        }
        public static DamagePackage ModifyDamage(DamagePackage dmg, ActorStat stat)
        {
            dmg.SetTargetStatEffect(stat.CloneNewInstance() as ActorStat);
            return dmg;
        }
    }
}
