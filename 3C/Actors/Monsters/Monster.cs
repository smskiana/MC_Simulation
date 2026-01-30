using Manager;
using Sirenix.OdinInspector;
using UnityEngine;
namespace _3C.Actors.Monsters
{
    public class Monster : Actor
    {
        [Header("敌人部分配置数据")]
        [SerializeField] private Transform target;
        [SerializeField] private float activeDistance = 20;
        [SerializeField] private float findPlayerDistance = 5;

        [SerializeField] private float loseDistance;
        [SerializeField] private float StopForAttackDistance;
        [SerializeField] private float findDistance;
        [FoldoutGroup("切换状态ID")]
        [LabelText("跟随状态ID")]
        [SerializeField] private string follow = "Follow";
        [SerializeField] private string attack = "Attack";
        private bool newDistance = true;
        private bool newGroundCheck = true;
        [SerializeField][ReadOnly] private bool onGround = false;
        [SerializeField][ReadOnly] private float distanceToTarget = float.MaxValue;
        public MonsterView MonsterView { get => base.View as MonsterView; }
        public Transform Target { get => target; set => target = value; }
        public override Vector3 MoveDir { get => base.MoveDir;
            set {
                base.MoveDir = value;
                MonsterView.SetForward(value);
            } }
        public MachineState lastState;
        public void Init(Transform target,int ID)
        {

        }
        public void GetMoveDir()
        {
            float chose = Random.Range(-1f, 1f);
            float value = Random.Range(-1f, 1f);
            float x, z;
            if(chose < 0f)
            {
                x = 0f;
                z = value < 0 ? -1 : 1;
            }
            else
            {
                z = 0f;
                x = value < 0 ? -1 : 1;
            }
            MoveDir = new Vector3(x, 0, z).normalized;

        }
        public void LateUpdate()
        {
            if (FarAway())
            {
                if (DefaultMachine.State != MachineState.Pause)
                {
                    lastState = DefaultMachine.State;
                    DefaultMachine.SetState(MachineState.Pause);
                }
            }
            else
            {
                if (DefaultMachine.State == MachineState.Pause)
                {
                    if (lastState != MachineState.None)
                    {
                        DefaultMachine.SetState(lastState);
                        lastState = MachineState.None;
                    }
                    else
                    {
                        DefaultMachine.SetState(MachineState.SwitchFree);
                    }
                }
            }
            ReStart();
        }
        private void ReStart()
        {
            newDistance = true;
            newGroundCheck = true;
        }
        public override void InitEvent()
        {
            AddEvent(Move, () => DistanceToTarget() > findDistance && CheckGround());
            AddEvent(Fall, () => !CheckGround());
            AddEvent(Idle, () => DistanceToTarget() > findDistance && CheckGround());
            AddEvent(follow,() => DistanceToTarget() <= findDistance&& DistanceToTarget()>StopForAttackDistance && CheckGround());
            AddEvent(attack, () => DistanceToTarget() <= StopForAttackDistance);
            base.Stat.CurEqZero += (a) => KillSelf();
        }
        public bool FarAway()
        {
            Transform target = Target;
            if (target == null) return true;
            float distance = (transform.position - Target.position).magnitude;
            if (distance > activeDistance) return true;
            return false;
        }
        public void FollowTarget()
        {
            if (target == null) return;
            var dir = Target.position - this.transform.position;
            dir = new Vector3(dir.x, 0, dir.z).normalized;
            MoveDir = dir;
        }
        public float DistanceToTarget()
        {
            if (newDistance)
            {
                distanceToTarget = CaculateDis();
                newDistance = false;
            }
            return distanceToTarget;
            float CaculateDis()
            {
                if (target == null) return float.MaxValue;
                var buf = (Target.position - this.transform.position);
                newDistance = false;
                return new Vector3(buf.x, 0, buf.z).magnitude;
            }
        }
        public override bool CheckGround()
        {
            if (newGroundCheck)
            {
                onGround = base.CheckGround();
                newGroundCheck = false;
            }
            return onGround;
        }
        [Button("自杀")]
        public void KillSelf() => MonsterManager.Instance.RycycleMonster(this);

    }
}
