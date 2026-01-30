using System.Collections.Generic;
using UnityEngine;

namespace WorldCreate.Chucks
{
    [System.Serializable]
    public class BlockPair
    {
        public Vector3Int index;
        public int id;
    }

    public class ChuckInfo
    {
        public Vector2 index;
        public List<BlockPair> BlocksInfo;
    }
}
