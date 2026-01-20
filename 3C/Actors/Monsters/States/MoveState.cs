
using Sirenix.OdinInspector;
using UnityEngine;

namespace _3C.Actors.Monsters.States
{
    [CreateAssetMenu(fileName = "MoveState", menuName = "State/Monster/Move")]
    public class MoveState : Actors.States.MoveState
    {
        public Monster Monster { get => base.Controller as Monster; }
        [SerializeField][Range(0f, 10f)][OnValueChanged(nameof(OnMinMoveChanged))] private float minMoveTime;
        [SerializeField][Range(0f, 10f)][OnValueChanged(nameof(OnMaxMoveChanged))] private float maxMoveTime;
        private float MoveTime;
        public void OnMinMoveChanged() => maxMoveTime = maxMoveTime > minMoveTime ? maxMoveTime : minMoveTime;
        public void OnMaxMoveChanged() => minMoveTime = minMoveTime < maxMoveTime ? minMoveTime : maxMoveTime;
        public float GetMoveTime() => Random.Range(minMoveTime, minMoveTime);
        public override void Enter()
        {
            MoveTime = GetMoveTime();
            Monster.GetMoveDir();
            base.Enter();
        }
        public override void Update()
        {        
            base.Update();
            MoveTime -= Time.deltaTime;
            if (MoveTime <= 0f)
            {
                Machine.NextState = Monster.Idle;
            }
        }
    }
}
