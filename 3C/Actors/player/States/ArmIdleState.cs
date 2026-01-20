
using UnityEngine;

namespace _3C.Actors.player.States
{
    [CreateAssetMenu(fileName = "ArmIdleState", menuName = "State/Player/ArmIdle")]
    public class ArmIdleState : PlayerState
    {
      
        public override void Enter()
        {
#if UNITY_EDITOR
            Debug.Log("当前状态：空手待机");
#endif
        }

        public override void Update()
        {
            
        }
    }
}
