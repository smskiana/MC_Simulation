using UnityEngine;

namespace _3C.Actors.Monsters.States
{
    [CreateAssetMenu(fileName = "Follow", menuName = "State/Monster/Follow")]
    public class FollowState :Actors.States.MoveState
    {
        [SerializeField] private float loseDistance;
        public Monster Monster { get => base.Controller as Monster; }
        protected override void InitInfo()
        {
            base.id = "Follow";
            base.orgin = "Default";
            base.serializableInterrupt.Add("Attack");
            base.serializableNextState.Add("Attack");
            base.serializableDefaultSwitch.Add("Idle");
        }
        public override void Enter()
        {
            base.Enter();
        }
        public override void Update()
        {          
            Monster.FollowTarget();
            base.Update();
        }
    }
}
