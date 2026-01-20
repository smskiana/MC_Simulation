using Manager;
using UnityEngine;

namespace _3C.Actors.Monsters.States
{
    [CreateAssetMenu(fileName = "FallState", menuName = "State/Monster/Fall")]
    public class FallState : Actors.States.FallState
    {
        public Monster Monster { get => base.Controller as Monster; }
        public override void Update()
        {
            base.Update();
            if (Monster.transform.position.y < -10f)
                Monster.KillSelf();
        }
    }
}
