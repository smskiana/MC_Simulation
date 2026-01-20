
using System.Collections.Generic;
using UnityEngine;

namespace WorldCreatation
{
    public class ChunkView:MonoBehaviour
    {
        private MeshFilter meshFilter;
        private MeshRenderer  meshRenderer;
        private MeshCollider meshCollider;
        public static Transform targetObject;
        public float rate = .2f;


        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();
            //TODO
            //targetObject = FindFirstObjectByType<player>().transform;
           // camra = FindFirstObjectByType<Camera>().transform;    
        }
        public void SetMesh(Mesh mesh)
        {
            if (meshFilter.sharedMesh == null)
            {
                meshFilter.sharedMesh = mesh;
            }           
            meshCollider.sharedMesh = meshFilter.sharedMesh;

        }
        public void SetMaterial(List<Material> material)
        {
           meshRenderer.materials = material.ToArray();
        }

      
    }
}
