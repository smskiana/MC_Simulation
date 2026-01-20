
using UnityEngine;

namespace _3C.Actors.States
{
    [CreateAssetMenu(fileName = "MoveState",menuName ="State/Player/Move")]
    public class MoveState : GroundState
    {
        [SerializeField] private float Defaultspeed;
        [SerializeField] private float press;
        protected Vector3 speed;     
        public override void Update()
        {         
            Vector3 dir = Actor.MoveDir;
            if (Actor != null && Actor.Stat != null)
                speed = Actor.Stat.MoveSpeed * dir;
            else
                speed = Defaultspeed*dir;
            Actor.Controller.Move(4 * Time.deltaTime * speed +press*Time.deltaTime*Vector3.down);
        }       
        public override void Enter()
        {
            Machine.SetState(MachineState.SwitchFree);
        }
        public override void Exit()
        {
            Actor.LastSpeed = speed;
            
        }
    }
}
