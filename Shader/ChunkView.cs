
using System.Collections.Generic;
using UnityEngine;

namespace WorldCreatation
{
    [RequireComponent(typeof(MeshCollider),typeof(MeshFilter),typeof(MeshRenderer))]
    public class ChunkView:MonoBehaviour
    {
        public MeshFilter meshFilter;
        public MeshRenderer  meshRenderer;
        public MeshCollider meshCollider;
      

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();
        
        }
        public void SetMesh(Mesh mesh)
        {
            if (meshFilter.sharedMesh == null)
            {
                meshFilter.sharedMesh = mesh;
            }           
            meshCollider.sharedMesh = meshFilter.sharedMesh;

        }
        public void SetMaterials(Material[] material)
        {
           meshRenderer.materials = material;
        }
        public void SetMaterial(Material material)
        {
           meshRenderer.material = material;
        }

      
    }
}
