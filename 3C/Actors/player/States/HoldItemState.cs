using Items;
using Sirenix.OdinInspector;
using StatSystems.Stats.Items;
using UnityEngine;

namespace _3C.Actors.player.States
{
    [CreateAssetMenu(fileName = "待机", menuName = "State/Player/Hold")]
    public class HoldItemState : PlayerState,IHandUseBottonDown
    {
        [ShowInInspector] protected bool CheckSomething;
        [ShowInInspector] protected Vector3Int pos;
        [SerializeField] bool ShowPos=true;
        protected override void InitInfo()
        {
            base.orgin = "Null";
            base.id = "Hold";
            base.serializableNextState ??= new();
            base.serializableNextState.Add("Use");
            base.serializableNextState.Add("Hold");
        }
        public override void Enter()
        {
            ItemStat stat;
            if ((stat = Player.Laststat) != null)
            {
                var info = stat.ItemInfo;
                var ob = Object.Instantiate(info.Idol);
                if(ob.TryGetComponent<Weapon>(out var component))
                {
                    component.Init(stat as EquipmentStat,Player);
                }
                Player.HoldItem(ob.transform);
            }
            else
            {
                Player.PlayerView.ClearHand();
            }
            if(!ShowPos) Player.PlayerView.HidePlaceCube();
        }
        public override void Update()
        {
            base.Update();
            CheckBlock();
        }
        private bool CheckBlock()
        {
            if (this.Player.CheckBlock(out var pos))
            {
                this.pos = Tool.Align(pos + Player.EyeForward * .1f);
                CheckSomething = true;
            }
            else            
                CheckSomething = false;      
            
            if (ShowPos)
            {
                if(!CheckSomething) 
                    Player.PlayerView.HidePlaceCube();
                else
                    Player.PlayerView.PlacePlaceCube(this.pos);
            }
            return CheckSomething;
        }
        public virtual void HandUseBottonDown() { Machine.NextState = Player.Use; }
        public override void Exit()
        {
            Player.PlayerView.HidePlaceCube();
        }
    }
}
