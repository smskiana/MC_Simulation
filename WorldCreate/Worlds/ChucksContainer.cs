using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorldCreate.Chucks;
using WorldCreate.TerrainInfos;
using System.Collections;
using Chuck = WorldCreate.Chucks.Chuck;
using Object = UnityEngine.Object;
using Block = WorldCreate.Chucks.Block;

namespace WorldCreate.Worlds
{
    [System.Serializable]
    public class ChucksContainer:IChunksCommunicate
    {
        public WorldCreartor creartor;
        public GameObject viewPrefab;
        public ChunkView Prefab;
        public int seed = 1213132;
        [SerializeField] private int BreakCount;
        private readonly List<Chuck> chucks = new();

        public ChucksContainer(int seed)
        {
            this.seed = seed;
            info = new TerrainInfo(seed);
        }

        private TerrainInfo info;
        public bool GetChuck(Vector3Int worldPos, out Chuck chuck)
        {
            Vector2Int n = GetIndex(worldPos);
            chuck = chucks.Find(x => x.pos == n);
            return chuck != null;
        }
        private static Vector2Int GetIndex(Vector3Int worldPos)
        {
            Vector3 pos = worldPos;
            Vector2Int n = new(Tool.FloorDiv(pos.x, Chuck.WIDTH), Tool.FloorDiv(pos.z, Chuck.LENGTH));
            return n;
        }
        public bool DeleteFace(Vector3Int worldPos, Face face)
        {
            if (GetChuck(worldPos, out var chuck))
            {
                return chuck.RemoveFace(chuck.ToChuckPos(worldPos), face);
            }
            return false;
        }
        public bool AddFace(Vector3Int worldPos, Face face)
        {
            if (GetChuck(worldPos, out var chuck))
            {
                return chuck.AddFace(chuck.ToChuckPos(worldPos), face);
            }
            return false;
        }
        public bool IsBlockExist(Vector3Int worldPos)
        {
            if (GetChuck(worldPos, out Chuck chuck))
            {
                return chuck.IsBlockExit(worldPos);
            }
            return false;
        }     
        public IEnumerator LoadChucks(Vector3Int worldCenter,int range)
        {
            var centerIndex = GetIndex(worldCenter);
            for (int left = centerIndex.x - range; left < centerIndex.x + range; left++)
                for (int back = centerIndex.y - range; back < centerIndex.y + range; left++)
                {
                    var index = new Vector2Int(left, back);
                    var buf = new Chuck(index,Object.Instantiate(viewPrefab).GetComponent<ChunkView>());
                }

            Task task = Task.Run(() =>
            {
                foreach (var chuck in chucks)
                    chuck.AddAllBlocks(info);
                foreach (var chuck in chucks)
                    chuck.Initblock();
                foreach (var chuck in chucks)
                    chuck.InitMeshInfo();
            });

            while (!task.IsCompleted)
                yield return null;
            if (task.Exception != null)
            {
                Debug.LogException(task.Exception);
            }
            else
            {
                int counter = 0;
                foreach (var chuck in chucks)
                {
                    chuck.CreateMesh();
                    counter++;
                    if (counter >= BreakCount)
                    {
                        counter = 0;
                        yield return null;
                    }
                }

            }

        }

        [Button]
        public void RemoveBlock(Vector3Int worldPos)
        {
            if (GetChuck(worldPos, out Chuck chuck))
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
            if (GetChuck(worldPos, out Chuck chuck))
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
