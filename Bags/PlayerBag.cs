using Items;
using Manager;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;
using System;
using UnityEngine;
using StatSystems.Stats.Items;

namespace Bags
{
    public class PlayerBag :MonoBehaviour,IPicker
    {
        [SerializeField]private BagSystem bag;
        [SerializeField]private BagSystem hotbar;
        [SerializeField]private int maxhotbarlist=8;
        [ShowInInspector]
        [ReadOnly]
        private int currentChoice;
        public event Action<ItemStat> HotItemChanged;
        public event Action<int> ChosePosChange;

        public void Awake()
        {
            hotbar.Init(maxhotbarlist);
            bag.Init();
        }
        public void Start()
        {
            Blind();
        }
        private void Blind()
        {
            hotbar.RegisterItemRefreshed(ctx =>
            {
                if (ctx == currentChoice)
                {
                    var (item, count) = hotbar.Peek(ctx);
                    if(count>0)
                        HotItemChanged?.Invoke(item);
                    else
                        HotItemChanged?.Invoke(null);
                }
            });
            hotbar.RegisterItemSwapped((a, b) => {
                if(a == currentChoice||b==currentChoice)
                {
                    var (item, count) = hotbar.Peek(currentChoice);
                    if (count > 0)
                        HotItemChanged?.Invoke(item);
                    else
                        HotItemChanged?.Invoke(null);
                }
                
            });
            var playerInput = InputManager.Instance.PlayerInput;
            var Swap = playerInput.FindAction("Swap");
            Swap.performed += OnSwap;
        }

        public int CurrentChoice { get => currentChoice; set {
             if(currentChoice != value)
                {
                    currentChoice = value;
                    var(item,count) = hotbar.Peek(value);
                    ChosePosChange?.Invoke(value);
                    if (count > 0)
                        HotItemChanged?.Invoke(item);
                    else
                        HotItemChanged?.Invoke(null);
                }
            } }
        public BagSystem Bag { get => bag;}
        public BagSystem Hotbar { get => hotbar;}
        public int Pickup(ItemStat info, int count)
        {
            int rest = bag.Add(info, count);
            return rest <= 0 ? 0 : hotbar.Add(info, rest);
        }
        public void OnSwap(InputAction.CallbackContext text)
        {
            float y = text.ReadValue<float>();
            var re = currentChoice;
            re += ((y > 0 ? 1 : -1) + maxhotbarlist);
            re %= maxhotbarlist;
            Debug.Log(re);
            CurrentChoice = re;
        }
        public bool TryTakeItem(out ItemStat item)
        {
            var (i,c) = hotbar.Take(currentChoice, 1);
            item = i;
            if(c >0) 
                return true;
            else
            {
                return false;
            }         
        }
        public bool HaveItem()
        {
            var (i, c) = hotbar.Peek(currentChoice);
            return i!=null && c >0;
        }
        public ItemStat Peek()
        {
            var (item, count) = hotbar.Peek(currentChoice);
            if(count<=0||item==null) return null;
            return item;
        }
    }
}
