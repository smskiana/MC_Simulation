using StatSystems.Stats.Items;
using StatSystems.Store.Items;
using UnityEngine;

namespace Items
{
    public interface IPicker
    {
        public abstract int Pickup(ItemStat info,int count);
    }

    public class Drop : MonoBehaviour
    {
        public ItemStat stat;
        public int Count;
        public Transform Idoloffset;
        public bool IsEmpty()
        {
            return Count <= 0 || stat == null;
        }
        public void Init(GameObject idol,ItemInfo stat,int count)
        {
            if (Idoloffset == null) Idoloffset = this.transform;
            idol.transform.SetParent(Idoloffset.transform);
            idol.transform.localPosition = Vector3.zero;
            if (count <= 0)
            {
                Destroy(this.gameObject);
                return;
            }
            this.stat = stat.GetNewItemStat();
            this.Count = count;
        }
        public void Init(GameObject idol,ItemStat stat,int count)
        {
            if(Idoloffset == null) Idoloffset = this.transform;
            idol.transform.SetParent(Idoloffset.transform);
            idol.transform.position = Vector3.zero;
            if (count <= 0)
            {
                Destroy(gameObject);
                return;
            }
            this.stat = stat;
            this.Count = count;
        }
        public void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IPicker>(out IPicker pick))
            {
                int rest = pick.Pickup(stat, Count);
                if (rest <= 0) Destroy(this.gameObject);
            }
        }
    }
}
