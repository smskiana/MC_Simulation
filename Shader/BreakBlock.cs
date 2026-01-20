
using Sirenix.OdinInspector;
using UnityEngine;

public class BreakBlock : MonoBehaviour
{
    [System.Serializable]
    private struct Pair 
    {
        public float x;
        public float y;
    }
    [SerializeField] private Pair[] pairs;
    public Renderer targetRenderer;
    [Button("ÉèÖÃÆ«ÒÆ")]
    public bool SetTextureOffset(int pos)
    {
        if(pos<0||pos>=pairs.Length) return false;
        var mat = targetRenderer.sharedMaterial;
        var pair= pairs[pos];
        // Æ«ÒÆ£¨Offset£©
        mat.SetTextureOffset("_BaseMap", new Vector2(pair.x, pair.y));
        return true;
    }

}
