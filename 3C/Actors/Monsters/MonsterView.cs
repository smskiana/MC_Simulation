using UnityEngine;
namespace _3C.Actors.Monsters
{
    public class MonsterView:View
    {
        public void SetForward(Vector3 dir)
        {
            if (Appearance != null)
            {
                if(dir==Vector3.zero) return;
                Appearance.forward = dir;
            }
        }
    }
}
