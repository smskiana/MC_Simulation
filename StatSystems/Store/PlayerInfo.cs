
using StatSystems.Stats;
using UnityEngine;

namespace StatSystems.Store
{
    [CreateAssetMenu(fileName = "NewPlayer", menuName = "Game/Info/Player")]
    public class PlayerInfo : ActorInfo
    {
        [Header("跳跃初始速度")]
        [SerializeField] private float jumpSpeed = 5.0f;
        public float JumpSpeed { get => jumpSpeed;}
        public override ActorStat GetNewActorStat() => new PlayerStat(this); 
    }
}
