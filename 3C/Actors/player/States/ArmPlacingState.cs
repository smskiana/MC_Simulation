using UnityEngine;
using StatSystems.Store.Items;

namespace _3C.Actors.player.States
{
    [CreateAssetMenu(fileName = "放置", menuName = "State/Player/放置")]
    public class ArmPlacingState: UseItemState
    {
        public BlocksInfo BlocksInfo;
        [SerializeField]private float BreakTime;
        
       

        protected override void InitInfo()
        {
            base.orgin = "Block";
            base.id = "Use";
            base.serializableNextState ??= new();
            base.serializableNextState.Add("Hold");
            base.digRange.Clear();
        }
        public override void Enter()
        {
            timer = BreakTime;
        }
        public override void Update()
        {
            if (CheckEnemy())
            {
                Attack();
                return;
            }
            PlaceCheck();
            TryPlace();
        }
        private void TryPlace()
        {
            timer += Time.deltaTime;
            if (timer >= BreakTime)
            {
                if (base.CheckedBlock)
                {
                    var item = Player.Laststat;
                    if (item!=null)
                    {
                        BlocksInfo = item.ItemInfo as BlocksInfo;
                        if(WorldManager.Instance.AddBlock(pos,BlocksInfo))
                            Player.PlayerBag.TryTakeItem(out _);
                    }
                }
                timer -= BreakTime;
            }
        }
        private void PlaceCheck()
        {
            if (this.Player.CheckBlock(out var pos))
            {
                this.pos = Tool.Align(pos - Player.EyeForward * .1f);
                Player.PlayerView.PlacePlaceCube(this.pos);
                CheckedBlock = true;
            }
            else
            {
                Player.PlayerView.HidePlaceCube();
                CheckedBlock = false;
            }
        }
    }
}
