using UnityEngine;

namespace _3C.Actors.player.States
{
    [CreateAssetMenu(fileName = "开枪", menuName = "State/Player/开枪")]
    public class GunUseState : UseItemState, IHandAimBottonDown, IHandAimBottonUp
    {
        public void HandAimBottonUp() => Player.View.StopAniMation(Player.Aim);
        public void HandAimBottonDown() => Player.View.PlayerAniMation(Player.Aim);
        public override void Update()
        {
            //null
        }

        public override void Enter()
        {
            base.Enter();
            if (Player.Weapon != null) Player.Weapon.Use();
        }
        public override void Exit()
        {
            base.Exit();
            if (Player.Weapon != null) Player.Weapon.StopUse();
        }

        public override void OnRecycle()
        {
            Player.View.StopAniMation(Player.Aim);
        }
    }
}