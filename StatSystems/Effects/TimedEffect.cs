using Actors;
using UnityEngine;

namespace Effects
{
    public class TimedEffect : Effect
    {
        protected float timer;
        public float PersistentTime;
        public override void Tick()
        {
            timer += Time.deltaTime;
            if (timer > this.PersistentTime||State==EffectState.Over)
            {
                timer = 0;
               State = EffectState.Over;
            }  
        }
       
    }
}
