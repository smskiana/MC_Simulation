using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using WorldCreatation;
using WorldCreate.TerrainInfos;
using Object = UnityEngine.Object;

namespace WorldCreate.Chucks
{
    public class ChuckTest:MonoBehaviour,IChunkCommunicate
    {
        public List<Chuck> chucks;
        public ChunkView Prefab;
        public int seed =1213132;
        public Material material;
        private TerrainInfo info;
        [Button]
        public async Task Begin()
        {
            info = new TerrainInfo(seed);
            chucks = new List<Chuck>();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    var chuck = new Chuck(new(i,j),
                        Instantiate(this.Prefab.gameObject,this.transform).GetComponent<ChunkView>())
                    {
                       world = this,
                    };
                    chucks.Add(chuck);
                }
            }
            Task task = Task.Run(() => 
            {
                foreach (var chuck in chucks)
                    chuck.AddAllBlocks(info);
                foreach (var chuck in chucks)
                    chuck.Initblock();
                foreach(var chuck in chucks)
                    chuck.InitMeshInfo();
            });

            try
            {
                await task;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return;
            }
            Debug.Log("完成");
            foreach (var chuck in chucks)
            {
                chuck.CreateMesh();
            }
        }

        public bool GetChuck(Vector3Int worldPos,out Chuck chuck)
        {
            chuck = null;
            Vector3 pos = worldPos;
            Vector2Int n = new(Tool.FloorDiv(pos.x,Chuck.WIDTH),Tool.FloorDiv(pos.z,Chuck.LENGTH));          
            chuck = chucks.Find(x =>x.pos == n);
            return chuck != null;
        }

        public bool DeleteFace(Vector3Int worldPos, Face face)
        {
            if(GetChuck(worldPos,out var chuck))
            {
               return chuck.RemoveFace(chuck.ToChuckPos(worldPos),face);
            }
            return false;
        }

        public bool AddFace(Vector3Int worldPos, Face face)
        {
            if (GetChuck(worldPos, out var chuck))
            {
                return chuck.AddFace(chuck.ToChuckPos(worldPos),face);
            }
            return false;
        }

        public bool IsBlockExit(Vector3Int worldPos)
        {
            if (GetChuck(worldPos, out Chuck chuck))
            {
                return chuck.IsBlockExit(worldPos);
            }
            return false;
        }
        [Button]
        public void RemoveBlock(Vector3Int worldPos)
        {
            if(GetChuck(worldPos, out Chuck chuck))
            {
                chuck.RemoveBlock(chuck.ToChuckPos(worldPos));
                foreach (var f_chuck in chucks)
                {
                    if (f_chuck.Excute())
                        f_chuck.CreateMesh();
                }
            }
           
        }
        [Button]
        public void AddBlock(Vector3Int worldPos)
        {
            if(GetChuck(worldPos, out Chuck chuck))
            {
                Block block = new(chuck.ToChuckPos(worldPos))
                {
                    ID = 1
                };
                chuck.AddBlock(block);
                foreach (var f_chuck in chucks)
                {
                    if (f_chuck.Excute())
                        f_chuck.CreateMesh();
                }
            }
           
        }
    }
}
