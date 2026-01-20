using UnityEngine;
namespace _3C.Actors.States
{
    public abstract class AirState : ActorState
    {
        [Range(0f,1f)]
        [SerializeField]private  float rate = 0.1f;
        [SerializeField]protected Vector3 g = Physics.gravity;
        protected Vector3 gSpeed;
        protected Vector3 moveSpeed;
        protected Vector3 speed; 
        public override void Enter()
        {
            gSpeed = Vector3.zero;
            moveSpeed = base.Actor.LastSpeed;
        }
        public override void Update()
        {
            gSpeed += g * Time.deltaTime;
            moveSpeed -= rate * Time.deltaTime * moveSpeed;
            speed = moveSpeed + gSpeed;
            Actor.Controller.Move(speed*Time.deltaTime);

        }
        public override void Exit()
        {
           Actor.LastSpeed = speed;
        } 
    }
}
