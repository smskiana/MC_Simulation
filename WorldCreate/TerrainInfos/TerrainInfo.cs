using UnityEngine;
using WorldCreate.Chucks;
namespace WorldCreate.TerrainInfos
{
    public class TerrainInfo:IGetBlock
    {
        public PlainTerrainGenerator plainGenerator;
        public PlainTerrainGenerator stoneGenerator;
        public WormCaveGenerator WormCaveGenerator;
      

        public int GetBlock(Vector3Int worldPos)
        {
            int x = worldPos.x;
            int z = worldPos.z;
            int y = worldPos.y;
            int h = plainGenerator.GetHeight(x, z, Chuck.HEIGHT/4);
            float h2 = stoneGenerator.GetHeight(x, z, h);
            int h_2 = (int)(h - h2 / 1.5f);
            if (y == 0) return 3;
            if (y == h - 1 &&  y >= 3) return 1;
            if(y >= h && y > 3 ) return 0;
            if(y < h_2) return 4;
            else return 2;
        }

        public TerrainInfo(int seed)
        {
            plainGenerator = new PlainTerrainGenerator(seed:seed);
            var seed_2 = seed / 2;
            stoneGenerator = new PlainTerrainGenerator(seed_2);
            WormCaveGenerator = new();         
        }
        
    }
}
