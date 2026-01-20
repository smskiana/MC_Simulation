using _3C.Actors.player;
using Bags;
using Bags.UI;
using Manager;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
namespace MyGUI
{
    public class UIManager:SingletonMono<UIManager>
    {
        [FoldoutGroup("玩家UI")]
        [LabelText("玩家快捷栏")]
        [SerializeField] private BagSystemUI HotBarUI;
        [FoldoutGroup("玩家UI")]
        [LabelText("背包")]
        [SerializeField] private BagSystemUI PlayerBagUI;
        [FoldoutGroup("玩家UI")]
        [LabelText("选择提示")]
        [SerializeField] private Chose chose;
        [LabelText("玩家血条")]
        [SerializeField] private Slider slider;
        
        public void BlindPlayer(Player player)
        {
            PlayerBag bag;
            if(bag = player.PlayerBag)
            {
                if(HotBarUI)
                    HotBarUI.ResetBagSystem(bag.Hotbar);
                if(PlayerBagUI)
                    PlayerBagUI.ResetBagSystem(bag.Bag);
                if (chose)
                    chose.BlindPlay(bag);
            }
            var playerInput = InputManager.Instance.PlayerInput;
            slider.maxValue = player.Stat.MaxHp;
            slider.value = player.Stat.Hp;

            player.Stat.CurHpChange += (_,_, value) => {
                slider.value = value;          
            };
            player.Stat.MaxHpChange += (_, _, maxValue) =>
            {
                slider.maxValue = maxValue;
            };


            playerInput.FindAction("Bag").performed += ctx =>
            {
                if (!PlayerBagUI.gameObject.activeSelf)
                {
                    PlayerBagUI.gameObject.SetActive(true);
                    InputManager.Instance.LockPlayerInput();
                }                  
            };

        }
        public bool AnyUIActive => PlayerBagUI.gameObject.activeSelf;
        public void CloseAll() => PlayerBagUI.gameObject.SetActive(false);
        protected override void SingletonAwake()
        {
            
        }
    }
}
