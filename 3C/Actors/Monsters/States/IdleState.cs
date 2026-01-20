using Sirenix.OdinInspector;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace _3C.Actors.Monsters.States
{
    [CreateAssetMenu(fileName = "IdleState", menuName = "State/Monster/Idle")]
    public class IdleState : Actors.States.IdleState
    {
        [LabelText("跟随状态ID")]
        [SerializeField] private string FollowState = "Follow";
        public Monster Monster { get => base.Controller as Monster; }
        [SerializeField][Range(0f, 10f)][OnValueChanged(nameof(OnMinWaitChanged))] private float minWaitTime;
        [SerializeField][Range(0f, 10f)][OnValueChanged(nameof(OnMaxWaitChanged))] private float maxWaitTime;
        private float idleTime = 0f;

        protected override void InitInfo()
        {
            base.id = "Idle";
            base.orgin = "default";
            base.serializableInterrupt.Add("Follow");
        }
        public void OnMinWaitChanged() => maxWaitTime = maxWaitTime > minWaitTime ? maxWaitTime : minWaitTime;
        public void OnMaxWaitChanged() => minWaitTime = minWaitTime < maxWaitTime ? minWaitTime : maxWaitTime;
        public float GetWaitTime() => Random.Range(minWaitTime, maxWaitTime);
        public override void Enter()
        {
            base.Enter();
            idleTime = GetWaitTime();
            Monster.MoveDir = Vector3.zero;
        }
        public override void Update()
        {
            base.Update();
            idleTime -= Time.deltaTime;
            if(idleTime <= 0f)
            {
                Machine.NextState = Monster.Move;
            }
        }
    }
}
