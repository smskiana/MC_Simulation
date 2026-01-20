using StatSystems.Store.Items;
using UnityEngine;
namespace StatSystems.Stats.Items
{
    [System.Serializable ]
    [CreateAssetMenu(fileName = "Anmo", menuName = "Game/Info/Anmo")]
    public class AnmoInfo : ItemInfo
    {
        [Header("飞行速度")]
        [SerializeField]
        private float speed;
        [Header("消失时间")]
        [SerializeField]
        private float time;
        [Header("数值")]
        [SerializeField]
        private float damage;
        public float Speed { get => speed;}
        public float Time { get => time;}
        public float Damage {get => damage; }
    }
}
