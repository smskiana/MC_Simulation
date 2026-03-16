using System.Collections.Generic;
using System.IO;
using UnityEngine;
using WorldCreate.Chucks;
namespace WorldCreate.TerrainInfos
{
    public class TerrainInfo:IGetBlockType
    {
        public PlainTerrainGenerator plainGenerator;
        public PlainTerrainGenerator stoneGenerator;
        public WormCaveGenerator WormCaveGenerator;
        string path = Application.persistentDataPath + "/chucks/";
        public Dictionary<Vector2, ChuckJson> chuckDic = new();
        public TerrainInfo(int seed)
        {
            plainGenerator = new PlainTerrainGenerator(seed:seed);
            var seed_2 = seed / 2;
            stoneGenerator = new PlainTerrainGenerator(seed_2);
            WormCaveGenerator = new();
           
        }
        public void LoadFile(Vector2Int id)
        {
            string path = this.path + id.ToString() + ".json";
            if(File.Exists(path))
            {
                string json = File.ReadAllText(path);
                ChuckJson chuckJson = JsonUtility.FromJson<ChuckJson>(json);
                chuckJson.Init();
                chuckDic.Add(id, chuckJson);
            }
        }
        public void UnloadAll()
            =>chuckDic.Clear();
        public int GetBlockType(Vector3Int chuckpos, Vector2Int chuckID)
        {
            if (chuckDic.TryGetValue(chuckID, out var value))
            {
                if (value.TryGetType(chuckpos, out var type))
                    return type;
                else
                    return 0;
            }
            else
            {
                Vector3Int worldPos = Chuck.GetWorldPos(chuckpos, chuckID);
                int x = worldPos.x;
                int z = worldPos.z;
                int y = worldPos.y;
                int h = plainGenerator.GetHeight(x, z, Chuck.HEIGHT / 4);
                float h2 = stoneGenerator.GetHeight(x, z, h);
                if (WormCaveGenerator.IsCave(worldPos)) return 0;
                int h_2 = (int)(h - h2 / 1.5f);
                if (y == 0) return 3;
                if (y == h - 1 && y >= 3) return 1;
                if (y >= h && y > 3) return 0;
                if (y < h_2) return 4;
                else return 2;
            }        
        }
    }
}
