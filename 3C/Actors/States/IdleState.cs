
using UnityEngine;


namespace _3C.Actors.States
{
    [CreateAssetMenu(fileName = "IdleState", menuName = "State/Player/Idle")]
    public class IdleState : GroundState
    {
        [SerializeField]private float press;
        public override void Enter()
        {
            Machine.SetState(MachineState.SwitchFree);
        }
        public override void Update()
        {          
            Actor.Controller .Move(press*Time.deltaTime*Vector3.down);

        } 
        public override void Exit()
        {
           Actor.LastSpeed = Vector3.zero;
        }
      
    }
}
