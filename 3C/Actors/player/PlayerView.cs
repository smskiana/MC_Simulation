
using Items;
using UnityEngine;

namespace _3C.Actors.player
{
    public class PlayerView : View
    {
        [SerializeField] private GameObject PlaceCube;
        [SerializeField] private GameObject destroyCube;
        [SerializeField] private BreakBlock breakBlock;
        [SerializeField] private Transform RightHand;
        [SerializeField] private Weapon weapon;
        public void Hold(Transform Item)
        {
            if(!Item) return;
            if(RightHand != null)
            {
                for(int i=0;i<RightHand.childCount;i++)
                {
                    Destroy(RightHand.GetChild(i).gameObject);
                }
                Item.SetParent(RightHand);
                Item.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                
            }
               
        }
        public void ClearHand()
        {
            if (RightHand != null)
            {
                for (int i = 0; i < RightHand.childCount; i++)
                {
                    Destroy(RightHand.GetChild(i).gameObject);
                }
            }
        }
        public void PlacePlaceCube(Vector3 postion)
        {
            PlaceCube.transform.position = postion;
            PlaceCube.SetActive(true);
        }
        public void HidePlaceCube()=>PlaceCube.SetActive(false);
        public bool SetDigLevel(int level) => breakBlock.SetTextureOffset(level);
    }
}
