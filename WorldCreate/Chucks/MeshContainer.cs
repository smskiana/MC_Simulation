using StatSystems.Store;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace WorldCreate.Chucks
{
    public class MeshContainer
    {
        public class SubMesh
        {
            public List<int> triangles = new();
            public Dictionary<Vector3Int,Block> blocks = new();
            public Material material;
        }
        public static Face[] Faces {  get=>Block.Faces; }
        private readonly Dictionary<int, int> IDToSubTriPos = new();
        private readonly Dictionary<int,SubMesh> subMeshsInfo= new();
        private readonly List<Vector2> uvs = new();
        private readonly List<Vector3> verts = new();
        public Mesh mesh;  
        public void SetMesh()
        {
            if (mesh == null)
            {
                mesh = new()
                {
                    vertices = verts.ToArray(),
                    uv = uvs.ToArray(),
                    subMeshCount = subMeshsInfo.Count
                };
                mesh.MarkDynamic();
                int count = 0;
                foreach (var i in subMeshsInfo.Values)
                {
                    mesh.SetTriangles(i.triangles.ToArray(), count);
                    count++;
                }
            }
            else
            {
                mesh.Clear();
                mesh.SetVertices(verts.ToArray());
                mesh.SetUVs(0, uvs.ToArray());
                mesh.subMeshCount = subMeshsInfo.Count;
                int count = 0;
                foreach (var i in subMeshsInfo.Values)
                {
                    mesh.SetTriangles(i.triangles.ToArray(), count);
                    count++;
                }
            }
            mesh.RecalculateNormals();  
        }
        public Material[] GetMaterials()
        {
            Material[]  materials = new Material[subMeshsInfo.Count];
            int count = 0;  
           foreach (var i in subMeshsInfo.Values)
            {
                materials[count++] = i.material;
            }
            return materials;
        }
        public void AddBlock(Block block)
        {
            foreach(var face in Faces)
            {
                if(block.IsNeedDraw(face)) AddFace(block,face);
            }
        }
        public void AddFace(Block block,Face face)
        {
           
            var vert = block.GetFacePoint(face);
            var uv  = block.GetUv(face);
            int vustart = verts.Count;
            if(!subMeshsInfo.TryGetValue(block.ID,out var subMesh))
            {
                subMesh = new();
                if(InfoStorer.Instance.TryFindBlock(block.ID,out var info))
                {
                    subMesh.material = info.Material;
                }
                subMeshsInfo.Add(block.ID, subMesh);
            }

            verts.AddRange(vert);
            uvs.AddRange(uv);

            int[] tri = Block.GetTriIndex();
            int[] newtri = new int[tri.Length];
            for (int i = 0; i < tri.Length; i++)
            {
                newtri[i] = vustart + tri[i];
            }
            int start =subMesh.triangles.Count;
            subMesh.triangles.AddRange(newtri);
            if (!subMesh.blocks.TryGetValue(block.posInChuck,out var buf))
            {
                buf = block;
                subMesh.blocks.Add(block.posInChuck, buf);
            }
            buf.DrawFace(face,start);
           
        }
        public void RemoveFace(Block block,Face face)
        {
            //取数据
            int tirIndex = block.GetTriIndex(face);
            var submesh = subMeshsInfo[block.ID];
            var tris = submesh.triangles;
            int vertAndUv = submesh.triangles[tirIndex];
            //找末尾
            int vertUvEnd = verts.Count - Block.FacePointCount;
            //处理点和uv
            //如果不是末尾
            if (vertAndUv != vertUvEnd)
            {
                //交换
                for (int i = 0; i < Block.FacePointCount; i++)
                {
                    (verts[vertUvEnd + i], verts[vertAndUv + i]) = (verts[vertAndUv + i], verts[vertUvEnd + i]);
                    (uvs[vertUvEnd + i], uvs[vertAndUv + i]) = (uvs[vertAndUv + i], uvs[vertUvEnd + i]);
                }
                verts.RemoveRange(vertUvEnd, Block.FacePointCount);
                uvs.RemoveRange(vertUvEnd, Block.FacePointCount);
                //查找
                bool hasFound = false;
                foreach (var sm in subMeshsInfo.Values)
                {
                    var f_tris = sm.triangles;
                    for (int i = 0; i < f_tris.Count; i++)
                    {
                        if (vertUvEnd == f_tris[i])
                        {
                            //更新索引
                            for (int j = 0; j < Block.TriIndexCount; j++)
                            {
                                f_tris[i + j] = tris[tirIndex + j];
                            }
                            hasFound = true;
                            break;
                        }
                    }
                    if (hasFound) break;
                }
            }
            else
            {
                verts.RemoveRange(vertUvEnd, Block.FacePointCount);
                uvs.RemoveRange(vertUvEnd,Block.FacePointCount);
            }

            //处理三角形索引
            int tirEnd = submesh.triangles.Count - Block.TriIndexCount;

            RemoveTris(block, face, tirIndex, submesh, tris, tirEnd);
        }

        private void RemoveTris(Block block, Face face, int tirIndex, SubMesh submesh, List<int> tris, int tirEnd)
        {
            //如果不是末尾
            if (tirEnd != tirIndex)
            {
                //交换
                for (int i = 0; i < Block.TriIndexCount; i++)
                    (tris[tirEnd + i], tris[tirIndex + i]) = (tris[tirIndex + i], tris[tirEnd + i]);
                tris.RemoveRange(tirEnd, Block.TriIndexCount);
                //空桶测试
                if (tris.Count == 0)
                {
                    subMeshsInfo.Remove(block.ID);
                }
                else
                {
                    bool hasFound = false;
                    foreach (var b in submesh.blocks.Values)
                    {
                        foreach (var f in Block.Faces)
                        {
                            int end = b.GetTriIndex(f);
                            if (end == tirEnd)
                            {
                                b.OnlySetTirIndex(f, tirIndex);
                                hasFound = true;
                                break;
                            }
                        }
                        if (hasFound) break;
                    }

                    block.ClearFace(face);
                    if (block.DotHaveTriInfo())
                        submesh.blocks.Remove(block.posInChuck);
                }
            }
            else
            {
                tris.RemoveRange(tirEnd, Block.TriIndexCount);
                if (tris.Count == 0)
                {
                    subMeshsInfo.Remove(block.ID);
                }
                else
                {
                    block.ClearFace(face);
                    if (block.DotHaveTriInfo())
                        submesh.blocks.Remove(block.posInChuck);
                }
            }
        }
        
        public void Replace(Block oldBlock,Face oldface,Block newBlock,Face newFace)
        {

            //取点
            var newVerts = newBlock.GetFacePoint(newFace);
            var newUvs = newBlock.GetUv(newFace);
            //找位置
            var oldtriIndex = oldBlock.GetTriIndex(oldface);
            var oldid= oldBlock.ID;
            var oldsubmesh = subMeshsInfo[oldid];
            var oldsubtri = oldsubmesh.triangles;
            var oldVertUv = oldsubtri[oldtriIndex];
            //替换点和Uv
            for(int i = 0;i<Block.FacePointCount;i++)
            {
                verts[oldVertUv + i] = newVerts[i];
                uvs[oldVertUv + i] = newUvs[i];
            }
            //开始分类讨论三角形索引

            //如果相同类型 =》相同的子网格划分
            if(oldBlock.ID==newBlock.ID)
            {
                //更新方块信息
                if (!oldsubmesh.blocks.TryGetValue(newBlock.posInChuck, out var block))
                {
                    block = newBlock;
                    oldsubmesh.blocks.Add(block.posInChuck, block);
                }
                block.DrawFace(newFace, oldtriIndex);

                //让旧的放弃引用
                oldBlock.ClearFace(oldface);
                if(oldBlock.DotHaveTriInfo())
                    oldsubmesh.blocks.Remove(oldBlock.posInChuck);
            }
            //否则按删除和添加逻辑处理
            else
            {
                //找/创建新的三角形索引
                if(!subMeshsInfo.TryGetValue(newBlock.ID,out var newSubMesh))
                {
                    newSubMesh = new SubMesh();
                    if(InfoStorer.Instance.TryFindBlock(newBlock.ID,out var info))
                        newSubMesh.material = info.Material;
                    subMeshsInfo.Add(newBlock.ID, newSubMesh);
                }
                int startindex = newSubMesh.triangles.Count;
                var tirs = newSubMesh.triangles;
                var blocks = newSubMesh.blocks;
                //在后面续上就行
                //生成索引
                var newTris = new int[Block.TriIndexCount];
                for (int i = 0; i < Block.TriIndexCount; i++)
                    newTris[i] = oldsubtri[oldtriIndex+i];
                tirs.AddRange(newTris);
                //更新方块信息
                if (!newSubMesh.blocks.TryGetValue(newBlock.posInChuck, out var f_block))
                {
                    f_block = newBlock;
                    newSubMesh.blocks.Add(f_block.posInChuck, f_block);
                }
                f_block.DrawFace(newFace, startindex);

                int oldTrisEnd = oldsubtri.Count - Block.TriIndexCount;
                //进行删除(交换移除)
                RemoveTris(oldBlock, oldface,oldtriIndex,oldsubmesh, oldsubtri, oldTrisEnd);
            }
           
        }

    }
}
