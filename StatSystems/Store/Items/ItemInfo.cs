using _3C;
using _3C.Actors.player;
using _3C.Actors.player.States;
using Sirenix.OdinInspector;
using StatSystems.Stats.Items;
using UnityEngine;

namespace StatSystems.Store.Items
{
    public enum ItemType
    {
        Other,
        Environment,
        Building,
        Furniture,
        Defence,
        Weapon,
        Food,
        Drug,
    }

    [CreateAssetMenu(fileName = "NewBlock", menuName = "Game/Info/DefaultItem")]
    public class ItemInfo : Info
    {
        [SerializeField] private ItemType itemType;
        [SerializeField] private string text;
        [SerializeField] private float cost = 10;
        [SerializeField] private float value = 5;
        [SerializeField] private Sprite image;
        [SerializeField] private int maxStackCount = 99;
        [SerializeField] private GameObject idol;
        [AssetsOnly]
        [LabelText("持有他的行为")]
        [SerializeField] private HoldItemState holdState;
        [AssetsOnly]
        [LabelText("使用他的行为")]
        [SerializeField] private UseItemState useState;
        public Sprite Image { get => image; }
        public string Text { get => text; }
        public int MaxStackCount { get => maxStackCount; }
        public float Value { get => value; }
        public float Cost { get => cost; }
        public ItemType ItemType { get => itemType; }
        public GameObject Idol { get => idol;}
        public State HoldState { get => holdState;}
        public State UseState { get => useState;}

        private State lasthold;
        private State lastuse;
        public override int CompareTo(Info info)
        {
            if (info is ItemInfo item)
            {
                if (info == null) return 1;
                if (itemType > item.itemType) return 1;
                if (itemType < item.itemType) return -1;
                return base.CompareTo(info);
            }
            return base.CompareTo(info);
        }
        public virtual void HandItemHold(Player player)
        {
            var machine = player.ArmMachine;
            if (holdState != null)
            {
                string id = holdState.ID;
                if (machine.TryGetState(id, out var state))
                {
                    lasthold = state;
                    machine.AddStateClone(holdState);
                }
            }
            if (UseState != null)
            {
                string id = UseState.ID;
                if(machine.TryGetState(id,out var state))
                {
                    lastuse = state;
                    machine.AddStateClone(useState);
                }
            }
        }  
        public virtual void HandItemSwitch(Player player)
        {
            var machine = player.ArmMachine;
            if (lasthold != null)
            {
                machine.AddStateClone(lasthold);
                lasthold = null;
            }
            if(lastuse != null)
            {
                machine.AddStateClone(lastuse);
                lastuse = null;
            }
        }   
        public virtual ItemStat GetNewItemStat() => new(this);
    }
}
