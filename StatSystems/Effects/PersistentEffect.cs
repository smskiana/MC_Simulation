using UnityEngine;
namespace Effects
{
    public class PersistentEffect : TimedEffect
    {
        [SerializeField]protected float bTimer;
        [SerializeField]protected float BreakTime;
    
        public override void Tick()
        {
            base.Tick();
            bTimer += Time.deltaTime;
            float buf = timer - PersistentTime;
            if (buf > 0) bTimer -= buf;
            while(bTimer >= BreakTime)
            {
                bTimer-= BreakTime;
            }
        }
    }
}
