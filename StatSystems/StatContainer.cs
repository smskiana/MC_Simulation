using Actors;
using Sirenix.OdinInspector;
using StatSystems.Stats;
using StatSystems.Store;


namespace StatSystems
{
    [System.Serializable]
    public class StatContainer:IDamageSource
    {
        public StatSystem StatSystem {  get;private set; }
        public StatContainer(ActorInfo charactorInfo,StatSystem statSystem)
        {
            ThisFrameStat = charactorInfo.GetNewActorStat();
            Stat =charactorInfo.GetNewActorStat();
            LastFrameStat = charactorInfo.GetNewActorStat();
            StatSystem = statSystem;
        }
        public StatContainer(StatSystem statSystem, ActorStat stat)
        {
            StatSystem = statSystem;
            ThisFrameStat = stat;
            LastFrameStat = stat.CloneNewInstance() as ActorStat;
            Stat = stat.CloneNewInstance() as ActorStat;
        }
        [ShowInInspector]
        public ActorStat ThisFrameStat {  get; internal set; }
        [ShowInInspector]
        public ActorStat LastFrameStat {  get; internal set; }
        [ShowInInspector]
        public ActorStat Stat {  get; internal set; }
        public void Reset()
        {
            ThisFrameStat.Reset();
            Stat.Reset();
        }
        public virtual void SetChange(ActorStat stats)
        {
            LastFrameStat.Reset(ThisFrameStat);
            ThisFrameStat.Reset(stats);
            if( LastFrameStat.maxHp != ThisFrameStat.maxHp)
            {
                StatSystem.TriggerMaxHpChange( LastFrameStat.maxHp, ThisFrameStat.maxHp);
            }
            if( LastFrameStat.currentHp != ThisFrameStat.currentHp)
            {
                StatSystem.TriggerCurHpChange( LastFrameStat.currentHp, ThisFrameStat.currentHp);
            }
          
        }
        public virtual void SetBasicChange(ActorStat stat)
        {
            Stat.Reset(stat);
            if (Stat.currentHp <= 0) StatSystem.TriggerHpEqZero();
        }
        public DamagePackage GetDamage()
        {
            var buf = new DamagePackage(ThisFrameStat.atackForce)
            {
                Source = this,
                state = PackageState.Sending,
            };
            return buf;
        }
    }
}

