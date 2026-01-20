using Sirenix.OdinInspector;
using StatSystems;
using System.Collections.Generic;
using UnityEngine;

namespace _3C.Actors.player.States
{
    [CreateAssetMenu(fileName = "使用", menuName = "State/Player/Use")]
    public class UseItemState : PlayerState,IHandUseBottonUp
    {
        [ShowInInspector]
        protected bool CheckedBlock;
        [ShowInInspector]
        protected Vector3Int pos;
        [LabelText("挖掘范围(相对位置)")]
        [SerializeField]protected List<Vector3Int> digRange = new() { Vector3Int.zero };
        [LabelText("挖掘效率")]
        [SerializeField] protected float DiglevelPersecond = 1f;
        [LabelText("是否展示挖掘提示框")]
        [SerializeField] bool ShowPos = true;
        [ShowInInspector] protected Vector3Int OldPos;
        [SerializeField]private float AttackCheckDistance;
        protected float timer = 0f;
        private int level = 0;
        private IDamageTarget source;
        protected override void InitInfo()
        {
            base.orgin = "Null";
            base.id = "Use";
            base.serializableNextState ??= new();
            base.serializableNextState.Add("Hold");
        }
        protected void Dig()
        {
            timer += Time.deltaTime;
            if (!CheckedBlock || OldPos != pos)
            {
                level = 0;
                timer = 0f;
                Player.PlayerView.SetDigLevel(0);
                return;
            }
            if (timer > DiglevelPersecond)
            {
                level++;
                timer = 0f;
                if (!Player.PlayerView.SetDigLevel(level))
                {
                    Vector3Int forward = Player.BodyForwardInt;
                    Vector3Int up = Vector3Int.up;
                 
                    var m = WorldManager.Instance;
                    if (m != null)
                    {
                        foreach (var localPoint in digRange)
                        {
                            Vector3Int worldPoint = pos + Tool.Rotation(forward, up, localPoint);
                            m.RemoveBlock(worldPoint);
                        }
                    }

                }

            }
        }       
        protected bool CheckBlock()
        {
            if (this.Player.CheckBlock(out var pos))
            {
                this.pos = Tool.Align(pos + Player.EyeForward * .1f);
                CheckedBlock = true;
            }
            else
            {
                CheckedBlock = false;
                this.pos = Vector3Int.zero;
            }
            if (ShowPos)
            {
                if (!CheckedBlock)
                    Player.PlayerView.HidePlaceCube();
                else
                    Player.PlayerView.PlacePlaceCube(this.pos);
            }
            return CheckedBlock;
        }
        protected bool CheckEnemy()
        {
            source = Player.CheckDamageTarget(AttackCheckDistance);
            return source != null;
        }
        protected void Attack()
        {
            var pack = Player.Stat.GetDamage();
            source.SetDamage(pack);
        }
        public override void Update()
        {
            OldPos = pos;
            if(CheckEnemy())
                Attack();
            else if (CheckBlock())           
                Dig();          
        }
        public override void Enter()
        {
            level = 0;
            Player.PlayerView.SetDigLevel(level);
            if (!ShowPos) Player.PlayerView.HidePlaceCube();
        }
        public override void Exit()
        {
            level = 0;
            Player.PlayerView.SetDigLevel(level);
            Player.PlayerView.HidePlaceCube();
        }      
        public void HandUseBottonUp()
        {
            Machine.NextState = Player.Hold;
        }
    }
}
