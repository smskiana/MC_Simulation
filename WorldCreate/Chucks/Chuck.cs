using System.Collections.Generic;
using UnityEngine;
using WorldCreatation;
using WorldCreate.TerrainInfos;

namespace WorldCreate.Chucks
{
    public enum ChuckState 
    {
        New,
        HaveAddBlocks,
        HaveInitBlocks,
        HaveInitMesh,
        HaveCreateMesh,
    }
    public interface IChunkCommunicate
    {
        public bool IsBlockExit(Vector3Int worldPos);
        public bool DeleteFace(Vector3Int worldPos,Face face);
        public bool AddFace(Vector3Int worldPos,Face face);
    }
    public interface IGetBlock
    {
        public int GetBlock(Vector3Int worldPos);
    }
    public class Chuck
    {
        public Vector2Int pos;
        public  Vector3Int Position { get => new(pos.x * WIDTH, 0, pos.y*LENGTH); }
        public IChunkCommunicate world; 
        //==================《区块基本信息》====================
        public const int WIDTH = 32;           
        public const int LENGTH = 32;      
        public ChuckState State {get;private set;} = ChuckState.New;
        public static int HEIGHT { get => 128; }
        //=================《动态记录信息》======================
        private readonly List<Block> removeBlockMark = new();
        private readonly List<(Block block, Face face)> pendingRemove = new();
        private readonly List<(Block block, Face face)> pendingAdd = new();
        private readonly BlocksContainer blocksContainer;
        private readonly MeshContainer meshContainer;
        private readonly ChunkView view;
        public Chuck(Vector2Int pos,ChunkView viewObject)
        {
            this.pos = pos;
            blocksContainer = new(this);
            meshContainer = new MeshContainer();
            removeBlockMark = new();
            pendingAdd = new();
            pendingRemove = new();
            this.view = viewObject;
            this.view.transform.localPosition = this.Position;
        }  
        public void AddAllBlocks(IGetBlock info)
        {
            if(State!=ChuckState.New)
            {
                throw new System.Exception("无法加载区块");
            }
            for (int x = 0; x < WIDTH; x++)
            {
                for(int y = 0; y < HEIGHT; y++)
                {
                    for (int z = 0; z < LENGTH; z++)
                    {
                        
                        Vector3Int posInChuck = new(x, y, z);
                        int id;
                        if((id=info.GetBlock(ToWorldPos(posInChuck))) != 0)
                        {
                            var block = new Block(posInChuck)
                            {
                                ID = id
                            };
                            blocksContainer.AddBlock(block);
                        }                    
                       
                    }
                }
            }
            State = ChuckState.HaveAddBlocks;
        }
        public void Initblock()
        {
            if(State != ChuckState.HaveAddBlocks)
            {
                throw new System.Exception("无法初始化方块");
            }
            blocksContainer.Initblocks();
            State = ChuckState.HaveInitBlocks;
        }
        public void InitMeshInfo()
        {
            if (State != ChuckState.HaveInitBlocks)
            {
                throw new System.Exception("无法初始化网格信息");
            }
            foreach (var block in blocksContainer.Blocks)
            {
                meshContainer.AddBlock(block);
            }
            State = ChuckState.HaveInitMesh;
        }
        public void CreateMesh()
        {
            if (State != ChuckState.HaveInitMesh)
            {
                throw new System.Exception("无法生成网格信息");
            }
            meshContainer.SetMesh();
            view.SetMesh(meshContainer.mesh);
            view.SetMaterials(meshContainer.GetMaterials());   
            State = ChuckState.HaveCreateMesh;
        }
        public Vector3Int ToChuckPos(Vector3Int worldpos) => worldpos - Position;
        public Vector3Int ToWorldPos(Vector3Int chuckpos) => chuckpos + Position;
        public bool IsotherChuckExistBlock(Vector3Int chuckpos) =>world!=null&&world.IsBlockExit(ToWorldPos(chuckpos));
        public bool IsBlockExit(Vector3Int worldpos) => blocksContainer.Contain(ToChuckPos(worldpos));
        public bool IsInChuck(Vector3Int chuckpos)
        {
            int x = chuckpos.x; int z = chuckpos.z;
            bool xInArea = x >= 0 && x < WIDTH;
            bool zInArea = z >= 0 && z < LENGTH;
            return xInArea && zInArea;
        }
        public void IsInChunkWorldPos(Vector3Int worldpos)=>IsInChuck(ToChuckPos(worldpos));
        public bool RemoveBlock(Vector3Int chuckPos)
        {
            if (State != ChuckState.HaveCreateMesh)
            {
                Debug.LogError("无法移除：未初始化区块");
                return false;
            }
            if (blocksContainer.TryGetBlock(chuckPos, out Block block))
            {
                removeBlockMark.Add(block);
                blocksContainer.RemoveBlockRealTime(block);
                return true;
            }
            return false ;
        }
        public bool RemoveFaceNotMe(Vector3Int chuckpos, Face face) => world != null && world.DeleteFace(ToWorldPos(chuckpos),face);
        public bool AddFaceNotMe(Vector3Int chuckpos,Face face)
            => world != null && world.AddFace(ToWorldPos(chuckpos),face);
        public bool AddBlock(Block block)
        {
            if (State != ChuckState.HaveCreateMesh)
            {
                Debug.LogError("无法添加：未初始化区块");
                return false;
            }
            if (!blocksContainer.TryGetBlock(block.posInChuck,out var _))
            {
                blocksContainer.AddBlockRealTime(block);
                return true;
            }
            return false;
        }
        public bool AddFace(Vector3Int chuckpos,Face face)
        {
            if(blocksContainer.TryGetBlock(chuckpos,out var block))
            {
                if (!block.IsFaceDraw(face))
                {
                    pendingAdd.Add((block, face));
                    block.MarkFaceDraw(face, true);
                }
                return true;
            }
            return false;
        }
        public void AddFace(Block block, Face face)
        {
            if (!block.IsFaceDraw(face))
            {
                pendingAdd.Add((block, face));
                block.MarkFaceDraw(face, true);
            }
        }
        public bool RemoveFace(Vector3Int chuckpos,Face face)
        {
            if(blocksContainer.TryGetBlock(chuckpos,out Block block))
            {
                if (block.IsFaceDraw(face))
                {
                    pendingRemove.Add((block, face));
                    block.MarkFaceDraw(face, false);
                    return true;
                }         
            }
            return false;
        }
        public void RemoveFace(Block block,Face face)
        {
            if (block.IsFaceDraw(face))
            {
                pendingRemove.Add((block, face));
                block.MarkFaceDraw(face, false);
            }
        }
        public bool Excute()
        {
            if(pendingRemove.Count == 0 && pendingAdd.Count == 0) return false;

            int AddPoint = 0;  
            for (int i = 0; i < pendingRemove.Count; i++)
            {
                if (AddPoint < pendingAdd.Count)
                {
                    meshContainer.Replace(
                        pendingRemove[i].block, pendingRemove[i].face,
                        pendingAdd[AddPoint].block, pendingAdd[AddPoint].face
                        );
                    AddPoint++;
                    continue;
                }
                meshContainer.RemoveFace(pendingRemove[i].block, pendingRemove[i].face);
            }
            for (int i = AddPoint; i < pendingAdd.Count; i++)
            {
                meshContainer.AddFace(pendingAdd[i].block, pendingAdd[i].face);
            }
           
            pendingAdd.Clear();
            pendingRemove.Clear();
            return true;
        }
    }
}
