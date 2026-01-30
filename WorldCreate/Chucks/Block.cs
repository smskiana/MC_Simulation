using UnityEngine;
using System.Collections.Generic;
namespace WorldCreate.Chucks
{
    public enum Face
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3,
        Forward = 4,
        Back = 5,
    }

    public class Block
    {
        public static readonly Face[] Faces = new Face[6] { Face.Up, Face.Down, Face.Left, Face.Right, Face.Forward, Face.Back };
        public static readonly Vector3Int[] normal = new Vector3Int[6] { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right, Vector3Int.forward, Vector3Int.back };
        private readonly bool[] faceDraw = new bool[6];
        private readonly int[] faceIndex = new int[6] {-1,-1,-1,-1,-1,-1,};
        public const int FacePointCount = 4;
        public const int TriIndexCount = 6;
        public const int FaceCount = 6;
        public Vector3Int posInChuck;
        public int ID = 1;
        public Block(Vector3Int posInChuck)
        {
            this.posInChuck = posInChuck;
        }
        public void MarkFaceDraw(Face face,bool need)=>faceDraw[(int)face]=need;
        public void MarkFaceDraw(int face,bool need)=>faceDraw[face]=need;
        public void CancelFaceDraw(Face face) => faceDraw[(int)face] = false;
        public void CancelFaceDraw(int face) => faceDraw[face] = false;
        public void DrawFace(Face face, int Tirindex)
        {
            faceDraw[(int)face] = true;
            faceIndex[(int)face] = Tirindex;
        }
        public void OnlySetTirIndex(Face face,int Tirindex)=> faceIndex[(int)face] = Tirindex;
        public void DrawFace(int face, int Tirindex)
        {
            faceDraw[face] = true;
            faceIndex[face] = Tirindex;
        }
        public int GetTriIndex(Face face) => faceIndex[(int)face];
        public int GetTriIndex(int face) => faceIndex[face];
        public bool IsFaceDraw(Face face) => faceDraw[(int)face];
        public bool IsFaceDraw(int face) => faceDraw[face];
        public bool IsNeedDraw(Face face) => faceDraw[(int)face] && faceIndex[(int)face] < 0;
        public bool IsNeedDraw(int face) => faceDraw[face] && faceIndex[face] < 0;
        public Vector3[] GetFacePoint(Face face)
        {
            int index = (int)face;
            Vector3[]faces = new Vector3[FacePointCount];
            for (int i = 0; i < FacePointCount; i++)
            {
                int tir = VoxelData.voxelTris[index,i];
                faces[i] = posInChuck + VoxelData.voxelVerts[tir];
            }
            return faces;
        }
        public static int[] GetTriIndex() => VoxelData.voxelTriIndex;
        public static Face Opposite(Face face)
        {
            return face switch
            {
                Face.Left => Face.Right,
                Face.Up => Face.Down,
                Face.Right => Face.Left,
                Face.Back => Face.Forward,
                Face.Forward => Face.Back,
                Face.Down => Face.Up,
                _ => throw new System.NotImplementedException(),
            };

        }
        public static int Opposite(int face) => face / 2 == 0 ? face+1 : face-1;
        public static IEnumerable<int> Tris 
        { 
            get
            {
                for(int i = 0; i < TriIndexCount; i++)
                {
                    yield return VoxelData.voxelTriIndex[i];
                }
            }
        }
        public Vector2[] GetUv(Face face) => VoxelData.SetUvPos(TurnToIndex(face));
        protected virtual int TurnToIndex(Face face) => (int)face;
        public void ClearFace(Face face) {faceDraw[(int)face] = false; faceIndex[(int)face] = -1; }
        public void ClearAll()
        {
            for (int i = 0;i < TriIndexCount; i++)
            {
                faceDraw[i] = false;
                faceIndex[i] = -1;
            }
        }
        public bool DotHaveTriInfo() => faceIndex[0] < 0 
            && faceIndex[1] < 0
            && faceIndex[2] < 0
            && faceIndex[3] < 0 
            && faceIndex[4] < 0 
            && faceIndex[5] < 0;
    }
    public static class VoxelData
    {

        public static readonly Vector3[] idToVec = new Vector3[6]
        {
            Vector3.up,      // 0
            Vector3.down,    // 1
            Vector3.left,    // 2
            Vector3.right,   // 3
            Vector3.forward, // 4
            Vector3.back     // 5
        };

        public static readonly Vector3[] voxelVerts = new Vector3[8]
        {
            Vector3.zero,                    //0
            Vector3.right,                   //1
            Vector3.up+Vector3.right,     //2
            Vector3.up,                      //3
            Vector3.forward,                 //4
            Vector3.forward+Vector3.right,//5
            Vector3.one,                     //6
            Vector3.up+Vector3.forward    //7
        };

        public static readonly int[,] voxelTris = new int[6, 4]
        {
            {6,2,7,3},//up
            {1,5,0,4},//dowm
            {4,7,0,3},//left
            {1,2,5,6},//right
            {5,6,4,7},//forward
            {0,3,1,2},//back
        };

        public static readonly int[] voxelTriIndex = new int[6] { 0, 1, 2, 2, 1, 3 };

        public static readonly Vector3[] voxelNormals = new Vector3[6]
        {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right,
            Vector3.forward,
            Vector3.back,
        };

        public static Vector2[][] UvPos = new Vector2[6][]
        {
             new Vector2[]{ -Vector2Int.one, -Vector2Int.one, -Vector2Int.one, -Vector2Int.one },
             new Vector2[]{ -Vector2Int.one, -Vector2Int.one, -Vector2Int.one, -Vector2Int.one },
             new Vector2[]{ -Vector2Int.one, -Vector2Int.one, -Vector2Int.one, -Vector2Int.one },
             new Vector2[]{ -Vector2Int.one, -Vector2Int.one, -Vector2Int.one, -Vector2Int.one },
             new Vector2[]{ -Vector2Int.one, -Vector2Int.one, -Vector2Int.one, -Vector2Int.one },
             new Vector2[]{ -Vector2Int.one, -Vector2Int.one, -Vector2Int.one, -Vector2Int.one },
        };
        public static Vector2[] SetUvPos(int pos)
        {
            if (UvPos[pos][0] == -Vector2Int.one)
            {
                Vector2[] vector2s = new Vector2[4];
                int t = pos / 3;
                int i = pos % 3;
                float x = i * (1.0f / 3.0f);
                float y = t * 0.5f;
                vector2s[0] = new Vector2(x, y);
                vector2s[1] = new Vector2(x, y + 0.5f);
                x += 1.0f / 3.0f;
                if (x > 1) x = 1;
                vector2s[2] = new Vector2(x, y);
                vector2s[3] = new Vector2(x, y + 0.5f);
                UvPos[pos] = vector2s;
                return vector2s;
            }
            else
            {
                return UvPos[pos];
            }

        }
    }
}
