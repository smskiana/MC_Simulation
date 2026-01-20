using StatSystems.Store;

namespace StatSystems.Stats
{
    [System.Serializable]
    public class PlayerStat : ActorStat
    {
        public float jumpSpeed;
        public PlayerInfo PlayerInfo { get => info as PlayerInfo; }

        public PlayerStat(PlayerInfo info) : base(info)
        {
            jumpSpeed = info.JumpSpeed;
        }

        protected PlayerStat(PlayerStat stat) : base(stat)
        {
            jumpSpeed = stat.jumpSpeed;
        }
        public override RealTimeData CloneNewInstance()
        {
            return new PlayerStat(this);
        }
        public override void Reset()
        {
            base.Reset();
            jumpSpeed = PlayerInfo.JumpSpeed;
        }

        public override void Reset(ActorStat stat)
        {
            base.Reset(stat);
            if(stat is PlayerStat player)
                jumpSpeed = player.jumpSpeed;
        }
    }
}
