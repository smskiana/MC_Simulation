using StatSystems.Store;
namespace StatSystems.Stats
{
    [System.Serializable]
    public class ActorStat : RealTimeData
    {
        public float moveSpeed = 2f;
        public float maxHp;
        public float defence;
        public float atackForce;
        public float breakForce;
        public float atackspeed;
        public float currentHp;
        public ActorInfo CharacterInfo { get => info as ActorInfo; }
        public ActorStat() { }
        public ActorStat(ActorInfo info) : base(info)
        {
           
        }
        protected ActorStat(ActorStat stat) : base(stat) 
        {
            moveSpeed = stat.moveSpeed;
            maxHp = stat.maxHp;
            defence = stat.defence;
            atackForce = stat.atackForce;
            breakForce = stat.breakForce;
            currentHp = stat.currentHp;
        }
        public override RealTimeData CloneNewInstance()
        {
           return new ActorStat(this);
        }
        public override void Reset()
        {
            base.Reset();
            var buf = CharacterInfo;
            moveSpeed = buf.MoveSpeed;
            maxHp = buf.Hp;
            defence = buf.Defence;
            atackForce = buf.AtackForce;
            breakForce = buf.BreakForce;
            currentHp = maxHp;
            atackspeed = buf.Atackspeed;
        }
        public virtual void Reset(ActorStat stat)
        {       
            moveSpeed = stat.moveSpeed;
            maxHp = stat.maxHp;
            defence = stat.defence;
            atackForce = stat.atackForce;
            breakForce = stat.breakForce;
            currentHp = stat.currentHp;
            atackspeed = stat.atackspeed;
        }
    }
}