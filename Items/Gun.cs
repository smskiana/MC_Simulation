using _3C.Actors;
using _3C.Actors.player;
using Sirenix.OdinInspector;
using StatSystems.Stats.Items;
using StatSystems.Store.Items;
using System;
using Object = UnityEngine.Object;

namespace Items
{
    public enum GunState
    {
        Free,
        Firing,
        Loading,
    }
    public class Gun:RangedWeapon
    {
        public GunState State = GunState.Free;
        public GunStat GunStat { get => base.stat as GunStat; }

        public event Action<int> OnRestAnmoCountChanged;
        [ShowInInspector] public bool NeedReLoad { get => GunStat!=null&& GunStat.AnmoCount <= 0; }
        [ShowInInspector] public bool CanReLoad { get => GunStat!=null&& GunStat.AnmoCount < GunStat.GunInfo.AnmoContain; }
        [ShowInInspector] public int RestAnmoCount {get => GunStat==null ? 0 : GunStat.AnmoCount; 
            private set { if (GunStat != null) GunStat.AnmoCount = value;OnRestAnmoCountChanged?.Invoke(value);  } }
        [ShowInInspector] public int MaxAnmo { get => GunStat == null ? 0 : GunStat.GunInfo.AnmoContain; }

        public bool NeedAnmo = false;
        private void Load()
        {
            if (NeedAnmo && actor is Player player)
            {
                var info = GunStat.GunInfo.UseAnmo;
                var need = GunStat.GunInfo.AnmoContain - RestAnmoCount;
                player.PlayerBag.Bag.TryTakeAny(info,need,out int rest);
                RestAnmoCount = GunStat.GunInfo.AnmoContain - rest;
            }
            else
            {
                RestAnmoCount = GunStat.GunInfo.AnmoContain;
            }
            view.StopAniMation("Load");
            State = GunState.Free;
            
        }
        public bool Reload()
        {
            if(!CanReLoad) return false; 
            if (State == GunState.Firing) StopUse();
            if (NeedAnmo&&actor is Player player)
            {
                var info = GunStat.GunInfo.UseAnmo;
                if(!player.PlayerBag.Bag.Contain(info)) return false;
            }
            view.PlayerAniMation("Load", LoadListen);
            State = GunState.Loading;
            return true;
        }
        public override void Use()
        {
            if(NeedReLoad||State==GunState.Loading) return;
            view.PlayerAniMation("Fire", ShotListen);
            view.Animator.SetFloat("Speed",GunStat.GunInfo.FireSpeed);
            State = GunState.Firing;
        }
        private void ShotListen(string obj) => Shot();
        private void LoadListen(string obj) => Load();
        private bool Shot()
        {
            if (NeedReLoad)
            {
                StopUse();
                return false;
            }
            var anmoInfo = GunStat.GunInfo.UseAnmo;
            if (anmoInfo != null)
            {
                var gameObject = anmoInfo.Idol;
                if (gameObject != null)
                {
                    gameObject = Object.Instantiate(gameObject);
                    if (gameObject.TryGetComponent<Anmo>(out var anmo)) anmo.Init(anmoInfo);
                    gameObject.transform.SetLocalPositionAndRotation(base.shotpos.position, base.shotpos.rotation);
                }
            }
            RestAnmoCount--;
            return true;
        }
        public override void StopUse()
        {
            view.StopAniMation("Fire");
            State = GunState.Free;
        }
        public override void Init(EquipmentInfo info, Actor actor)
        {
            base.Init(info, actor);
            if(info is GunInfo gun)
            {
                NeedAnmo = gun.NeedAnmo;
            }
        }
        public override void Init(EquipmentStat stat, Actor actor)
        {
            base.Init(stat, actor);
            if (stat.ItemInfo is GunInfo gun)
            {
                NeedAnmo = gun.NeedAnmo;
            }
        }
    }
}
