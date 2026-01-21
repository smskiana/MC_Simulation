using UnityEngine;

namespace Items
{
    public class Weapon :Equipment
    {
        [SerializeField]private Transform idol;
        public Transform Idol { get { return idol; } private set=>idol=value; }
        public virtual void Use()
        {

        }
        public virtual void StopUse()
        {

        }
        public virtual void Excute() { }
        public void LookAt(Vector3 pos)
        {
            transform.LookAt(pos);
        }
    }
}
