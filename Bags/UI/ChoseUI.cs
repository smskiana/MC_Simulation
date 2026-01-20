using Sirenix.OdinInspector;
using UnityEngine;

namespace Bags.UI
{
    public class Chose: MonoBehaviour
    {
        public BagSystemUI blindUI;
        [Button("绑定")]
        public void BlindPlay(PlayerBag bag)
        {
            bag.ChosePosChange += OnChoseChanged;
            int pos = bag.CurrentChoice;
            if(blindUI!=null)
            {
                var buf = blindUI.CellBackUIs[pos];
                this.transform.position = buf.transform.position;
            }
        }

        public void OnChoseChanged(int pos)
        {
            if (blindUI != null)
            {
                var buf = blindUI.CellBackUIs[pos];
                this.transform.position = buf.transform.position;
            }
        }

    }
}
