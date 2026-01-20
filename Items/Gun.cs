using StatSystems.Stats.Items;
using UnityEngine;

namespace Items
{
    public class Gun:RangedWeapon
    {
        public GunStat GunStat { get => base.stat as GunStat; }
        public override void Use()
        {
            view.PlayerAniMation("Fire", Listen);
            view.Animator.SetFloat("Speed",GunStat.GunInfo.FireSpeed);
        }

        private void Listen(string obj)
        {
            var anmoInfo = GunStat.GunInfo.UseAnmo;
            if (anmoInfo!=null)
            {
                var gameObject =anmoInfo.Idol;
                if(gameObject != null)
                {
                    gameObject = Object.Instantiate(gameObject);
                    if (gameObject.TryGetComponent<Anmo>(out var anmo)) anmo.Init(anmoInfo);
                    gameObject.transform.SetLocalPositionAndRotation(base.shotpos.position, base.shotpos.rotation);
                }           
            }
        }

        public override void StopUse()
        {
            view.StopAniMation("Fire");
        }
    }
}
