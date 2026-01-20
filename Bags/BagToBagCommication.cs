using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Bags
{
    public class BagToBagCommication : MonoBehaviour
    {
        [SerializeField] private BagSystem player;
        [SerializeField] private BagSystem other;
        [SerializeField] private BagSystemUI plyerBagUI;
        [SerializeField] private BagSystemUI otherBagUI;
        public void PlayerMoveToOther()
        {
            player.MoveAllTo(other);
        }
        public void OtherMoveToPlayer()
        {
            other.MoveAllTo(player);
        }
        public void RegistPlayer(BagSystem player)
        {
            this.player = player;
            plyerBagUI.ResetBagSystem(player);
        }
        public void Registother(BagSystem other)
        {
            this.other = other;
            otherBagUI.ResetBagSystem(other);
        }
        public IEnumerator Start()
        {
            yield return null;
            if(player != null)
            {
                plyerBagUI.ResetBagSystem(player);
            }
            if(other != null)
            {
                otherBagUI.ResetBagSystem(other);
            }
        }

    }
}
