using _3C.Actors;
using UnityEngine;

namespace Items.Animators
{
    public class AnimatorBridge: MonoBehaviour
    {
        public View weapon;
        public void TriggerAction(string name)=>weapon.TriggerAnimationCallback(name);
    }
}
