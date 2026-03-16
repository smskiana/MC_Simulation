using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace WorldCreate.Chucks
{
    [Serializable]
    public struct BlockJson
    {
        public Vector3 chuckpos;
        public int id;

        public BlockJson(Vector3 chuckpos, int id)
        {
            this.chuckpos = chuckpos;
            this.id = id;
        }
    }
    [Serializable]
    public class ChuckJson
    {
        public Vector2Int id;
        public List<BlockJson> blocks;
        private Dictionary<Vector3, int> dic;
        public void Init()
        {
            dic = new Dictionary<Vector3, int>();
            if (blocks != null)
                foreach (var block in blocks)
                    dic.Add(block.chuckpos, block.id);
            blocks.Clear();
        }
        public bool TryGetType(Vector3 chuckpos, out int id) => dic.TryGetValue(chuckpos, out id);
    }
}
