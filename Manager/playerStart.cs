using Sirenix.OdinInspector;
using UnityEngine; 

namespace Manager
{
    [ExecuteAlways]
    public class PlayerStart:MonoBehaviour
    {
        [Button("调整位置")]
        public void UpdatePos()
        {
            float x = transform.position.x;
            float z = transform.position.z;
            Tool.GetGroundY(x, z, out float y);
            transform.position = new(x, y + 1.2f, z);
        }
    }
}
