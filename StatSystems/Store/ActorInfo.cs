using StatSystems.Stats;
using UnityEngine;
namespace StatSystems.Store
{
    public abstract class ActorInfo : Info
    {
        [Header("移动速度")]
        [SerializeField] private float moveSpeed = 2f;
        [Header("基础生命值")]
        [SerializeField] private float hp;
        [Header("基础防御力")]
        [SerializeField] private float defence;
        [Header("攻击力")]
        [SerializeField] private float atackForce;
        [Header("破坏力")]
        [SerializeField] private float breakForce;
        [Header("攻击速度")]
        [SerializeField] private float atackspeed;
        [Header("立绘ID")]
        [SerializeField] private string spriteID;
        [Header("角色预制体")]
        [SerializeField] private GameObject prefab;
        public float MoveSpeed { get => moveSpeed; }
        public float Hp { get => hp;}
        public float Defence { get => defence;}
        public float AtackForce { get => atackForce; }
        public float Atackspeed { get => atackspeed;}
        public float BreakForce { get => breakForce;}
        public GameObject Prefab { get => prefab;}
        public virtual ActorStat GetNewActorStat()=>new(this);
    }
}
