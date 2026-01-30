using System.Collections.Generic;
using UnityEngine;
namespace WorldCreate.TerrainInfos
{
    public interface ICaveGenerator
    {
        bool IsCave(Vector3Int pos);
    }
    public class WormCaveGenerator : ICaveGenerator
    {
        public bool IsCave(Vector3Int pos)
        {
            float t = Mathf.Sin(pos.x * 0.1f)
                    + Mathf.Sin(pos.z * 0.1f)
                    + Mathf.Sin(pos.y * 0.15f);

            return t > 2.2f && pos.y > 6 && pos.y < 120;
        }
    }

    public static class Perlin3D
    {
        public static float Noise(float x, float y, float z)
        {
            float xy = Mathf.PerlinNoise(x, y);
            float yz = Mathf.PerlinNoise(y, z);
            float xz = Mathf.PerlinNoise(x, z);
            float yx = Mathf.PerlinNoise(y, x);
            float zy = Mathf.PerlinNoise(z, y);
            float zx = Mathf.PerlinNoise(z, x);
            return (xy + yz + xz + yx + zy + zx) / 6f * 2f - 1f;
        }
    }
   
}

