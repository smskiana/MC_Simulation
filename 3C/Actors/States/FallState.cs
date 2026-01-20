using UnityEngine;
namespace _3C.Actors.States
{
    [CreateAssetMenu(fileName = "FallState", menuName = "State/Player/Fall")]
    public class FallState : AirState
    {      
        public override void Enter()
        {
#if UNITY_EDITOR
            Debug.Log("当前状态：下落");
#endif
           base.Enter();
        }
        public override void Exit()
        {

        }
    }
}
