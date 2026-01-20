using Items;
using Sirenix.OdinInspector;
using StatSystems.Store.Items;
using UnityEngine;

namespace Manager
{
    public class DropperManager:SingletonMono<DropperManager>
    {
        public GameObject dropPrefab;
        public GameObject Idol;
        [Button("生成掉落物")]
        public Drop CreateDropprer(Vector3 pos,ItemInfo info,int count = 1)
        {
            var drop = Object.Instantiate(dropPrefab);
            GameObject Idol;
            if(info.Idol == null) 
                Idol = Object.Instantiate(this.Idol);
            else
                Idol = Object.Instantiate(info.Idol);
            var d = drop.GetComponent<Drop>();
            d.Init(Idol, info, count);
            drop.transform.position = pos;
            return d;
        }
        [Button("生成掉落物(tranform)")]
        public Drop CreateDropprer(Transform pos,ItemInfo info,int count = 1)
        {
            var drop = Object.Instantiate(dropPrefab);
            if (info.Idol == null)
                Idol = Object.Instantiate(this.Idol);
            else
                Idol = Object.Instantiate(info.Idol);
            var d = drop.GetComponent<Drop>();
            d.Init(Idol, info, count);
            drop.transform.position = pos.position;
            return d;
        }
        protected override void SingletonAwake()
        {
            
        }
    }
}
