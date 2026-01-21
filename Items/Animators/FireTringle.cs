using _3C.Actors;
using UnityEngine;
namespace AnimatorControl
{
    public class WeaponAnimator : MonoBehaviour
    {
        public Actor charactor;      
        public void OnEnable()
        {
            charactor = GetComponentInParent<Actor>();
        }
    }

}
