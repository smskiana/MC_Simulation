
using _3C.Actors.States;
using UnityEngine;
namespace _3C.Actors.player.States
{
    [CreateAssetMenu(fileName = "JumpState", menuName = "State/Player/Jump")]
    public class Jump : AirState
    {
        public Player Player { get=>base.Controller as Player;}  
        [SerializeField] private float JumpForce;
        public override void Enter()
        {
#if UNITY_EDITOR
            Debug.Log("当前状态：跳跃");
#endif
            base.Enter();
            Machine.SetState(MachineState.SwitchLocked);
            float jump;
            if (Player!=null&&Player.PlayerStatSystem != null&&(jump=Player.PlayerStatSystem.JumpSpeed)>0)
                gSpeed += jump * Vector3.up;
            else
                gSpeed += JumpForce * Vector3.up;
        }
        public override void Update()
        {
            base.Update();
            if (gSpeed.y < 0)
            {
                Machine.SetState(MachineState.SwitchFree);
            }
        }
    }
}
