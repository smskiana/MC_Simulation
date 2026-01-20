using Sirenix.OdinInspector;
using StatSystems;
using StatSystems.Stats;

namespace _3C.Actors.player
{
    public class PlayerStatSystem :StatSystem
    {
        [ShowInInspector]
        public float JumpSpeed
        {
            get
            {
                if(container != null)
                {
                    if(container.ThisFrameStat is PlayerStat stat)
                    {
                        return stat.jumpSpeed;
                    }
                }
                return 0f;
            }
        }
  
    }
}
