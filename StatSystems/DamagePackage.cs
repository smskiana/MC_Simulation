using StatSystems.Stats;
using UnityEngine;

namespace StatSystems
{
    public enum PackageState
    {
        Over,
        Preparing,
        Sending,
        Recepted,       
    }

    public class DamagePackage
    {
        public int Id;
        private float damage;
        public PackageState state = PackageState.Preparing;
        public DamagePackage(float damage)
        {
            this.damage = damage;
        }
        public IDamageSource Source {  get; set; }
        public IDamageTarget Target {  get; set; }
        
        public virtual void SetTargetStatEffect(ActorStat stat)
        {
            damage -=stat.defence;
            damage = damage==0?1:damage;
        }

        public virtual void Apply(ActorStat stat)
        {
            stat.currentHp = Mathf.Clamp(stat.currentHp-damage,0,stat.maxHp);
        }
    }
}
