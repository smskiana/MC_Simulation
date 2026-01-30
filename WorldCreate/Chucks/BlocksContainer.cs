using System.Collections.Generic;
using UnityEngine;


namespace WorldCreate.Chucks
{
    public class BlocksContainer
    {
        private readonly Dictionary<Vector3Int, Block> blocks;
        public Chuck chuck;
     
        public BlocksContainer(Chuck chuck)
        {
            blocks = new Dictionary<Vector3Int, Block>();
            this.chuck = chuck;
        }
        public IEnumerable<Block> Blocks
        {
            get
            {
                foreach (var block in blocks.Values)
                {
                    yield return block;
                }
            }
        }

        /// <summary>
        ///  初始化时使用
        /// </summary>
        public void AddBlock(Block block)=>blocks[block.posInChuck] = block;
        public void AddBlockRealTime(Block block)
        {
            blocks.Add(block.posInChuck, block);
            HandAddRealTime(block);
        }
        public void RemoveBlockRealTime(Block block)
        {
            blocks.Remove(block.posInChuck);
            HandRemoveRealTime(block);
        }
        private void HandAddRealTime(Block block)
        {
            Vector3Int pos = block.posInChuck;
            foreach (var face in Block.Faces)
            {
                Vector3Int next = pos + Block.normal[(int)face];
                if (!DeleteBlockFace(next, Block.Opposite(face)))
                {
                    chuck.AddFace(block,face);
                }
            }
        }
        private void HandRemoveRealTime(Block block)
        {
            var pos = block.posInChuck;
            foreach (var face in Block.Faces)
            {
                if (block.IsFaceDraw(face))
                {                  
                    chuck.RemoveFace(block,face);
                }
                else
                {
                    Vector3Int next = pos + Block.normal[(int)face];
                    Face op = Block.Opposite(face);
                    AddBlockFace(next, op);
                }
            }
        }
        private bool DeleteBlockFace(Vector3Int chuckPos, Face face)
        {
            bool inChuck = chuck.IsInChuck(chuckPos);
            if (inChuck)
            {
                if(blocks.TryGetValue(chuckPos,out var block))
                {
                    chuck.RemoveFace(block,face);
                    return true;
                }
                return false;
            }
            else
            {
                return chuck.RemoveFaceNotMe(chuckPos,face);    
            }
        }
        private bool AddBlockFace(Vector3Int chuckPos, Face face)
        {
            bool inChuck = chuck.IsInChuck(chuckPos);
            if (inChuck)
            {
                if (blocks.TryGetValue(chuckPos, out var block))
                {
                    chuck.AddFace(block, face);
                    return true;
                }
                return false;
            }
            else
            {
                return chuck.AddFaceNotMe(chuckPos, face);
            }
        }
        public bool Contain(Vector3Int index)=>blocks.ContainsKey(index);
        private void HandBlock(Block block)
        {
            Vector3Int pos = block.posInChuck;
            Vector3Int up = pos + Vector3Int.up;
            Vector3Int down = pos + Vector3Int.down;
            Vector3Int right = pos + Vector3Int.right;
            Vector3Int left = pos + Vector3Int.left;
            Vector3Int forward = pos + Vector3Int.forward;
            Vector3Int back = pos + Vector3Int.back;
            block.MarkFaceDraw(Face.Up,      ! blocks.ContainsKey(up));
            block.MarkFaceDraw(Face.Down,    ! blocks.ContainsKey(down));
            block.MarkFaceDraw(Face.Left,    chuck.IsInChuck(left) ? !blocks.ContainsKey(left) : !chuck.IsotherChuckExistBlock(left));
            block.MarkFaceDraw(Face.Right,   chuck.IsInChuck(right) ? !blocks.ContainsKey(right) : !chuck.IsotherChuckExistBlock(right));
            block.MarkFaceDraw(Face.Forward, chuck.IsInChuck(forward) ? !blocks.ContainsKey(forward) : !chuck.IsotherChuckExistBlock(forward));
            block.MarkFaceDraw(Face.Back,    chuck.IsInChuck(back) ? !blocks.ContainsKey(back) : !chuck.IsotherChuckExistBlock(back));
        }
        public void Initblocks()
        {
            foreach(var block in blocks.Values)
            {
                HandBlock(block);
            }
        }
        public bool TryGetBlock(Vector3Int chuckPos, out Block block)=>blocks.TryGetValue(chuckPos, out block);
    }
}
