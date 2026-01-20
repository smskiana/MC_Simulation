using UnityEngine;


namespace _3C.Actors.player.States
{
    [CreateAssetMenu(fileName = "持有方块", menuName = "State/Player/持有方块")]
    public class ArmPlaceState : HoldItemState
    {
        protected override void InitInfo()
        {
            base.orgin = "Block";
            base.id = "Hold";
            base.serializableNextState ??= new();
            base.serializableNextState.Add("Use");
        }
        public override void Enter()
        {
            base.Enter();     
            Player.PlayerView.SetDigLevel(0);
        }
        public override void Update()
        {
            PlaceCheck();
        }
        private void PlaceCheck()
        {
            if (this.Player.CheckBlock(out var pos))
            {
                this.pos = Tool.Align(pos - Player.EyeForward * .1f);
                Player.PlayerView.PlacePlaceCube(this.pos);
                CheckSomething = true;
            }
            else
            {
                Player.PlayerView.HidePlaceCube();
                CheckSomething = false;
            }
        }
    }
}
