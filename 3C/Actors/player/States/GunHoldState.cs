using UnityEngine;
namespace _3C.Actors.player.States
{
    [CreateAssetMenu(fileName = "持枪", menuName = "State/Player/持枪")]
    public class GunHoldState : HoldItemState,IHandAimBottonDown,IHandAimBottonUp
    {
        [SerializeField] private string OverInfo;
        public void HandAimBottonUp() => Player.View.StopAniMation(Player.Aim);
        public void HandAimBottonDown() => Player.View.PlayerAniMation(Player.Aim);
        public override void Update()
        {
            // null
        }
        public override void OnRecycle()
        {
            Player.View.StopAniMation(Player.Aim);
        }
    }
}
