using Sirenix.OdinInspector;
using StatSystems;
using UnityEngine;

namespace _3C.Actors.Monsters.States
{
    [CreateAssetMenu(fileName = "AttackState", menuName = "State/Monster/Attack")]
    public class AttackState : State
    {
        public Monster Monster { get => base.Controller as Monster; }
        [SerializeField][Range(0f, 10f)][OnValueChanged(nameof(OnMinWaitChanged))] private float minWaitTime;
        [SerializeField][Range(0f, 10f)][OnValueChanged(nameof(OnMaxWaitChanged))] private float maxWaitTime;
        public void OnMinWaitChanged() => maxWaitTime = maxWaitTime > minWaitTime ? maxWaitTime : minWaitTime;
        public void OnMaxWaitChanged() => minWaitTime = minWaitTime < maxWaitTime ? minWaitTime : maxWaitTime;
        public float GetWaitTime() => Random.Range(minWaitTime, maxWaitTime);

        private float WaitTime;
        protected override void InitInfo()
        {
            base.id = "Attack";
            base.orgin = "Default";
            base.serializableDefaultSwitch.Add("Follow");
            base.serializableDefaultSwitch.Add("Idle");
            base.serializableInterrupt.Add("Hurt");        
        }
        public override void Enter()
        {
            WaitTime = GetWaitTime();
            Machine.Lock();
            Attack();
        }
        public override void Update()
        {
            WaitTime -=Time.deltaTime;
            if(WaitTime < 0f) Machine.Unlock();
        }
        
        public void Attack()
        {
            if(Monster.Target.TryGetComponent<IDamageTarget>(out var target))
            {
                var buf = Monster.Stat.GetDamage();
                target.SetDamage(buf);
            }
        }
    }
}
