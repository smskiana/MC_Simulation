using UnityEngine;

namespace _3C.Actors.States
{
    [CreateAssetMenu(fileName = "HurtState", menuName = "State/Player/Hurt")]
    public class HurtState :AirState
    {
        [SerializeField] private Vector2 Impulse;

        protected override void InitInfo()
        {
            base.id = "Hurt";
            base.orgin = "Default";
            serializableDefaultSwitch.Add("Fall");
            serializableDefaultSwitch.Add("Idle");
            serializableDefaultSwitch.Add("Move");
        }

        public override void Enter()
        {
            base.Enter();
            Vector3 dir = Actor.HurtDir;
            dir = new Vector3(dir.x, 0 , dir.z).normalized*Impulse.x;
            Vector3 up = Vector3.up* Impulse.y;
            moveSpeed += dir;
            gSpeed += up;
            Machine.SetState(MachineState.SwitchLocked);
        }

        public override void Update()
        {
            base.Update();
            if (gSpeed.y < 0f) Machine.SetState(MachineState.SwitchFree);
        }
        
    }
}
