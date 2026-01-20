using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Manager
{
    public class InputInfo: MonoBehaviour
    {
        [Header("Input Action Asset")]
        public InputActionAsset actions;
        public InputInfo instance;
        public void Awake()
        {
            instance = this;
        }

        [Header("UI")]
        public Button jumpRebindButton;
        public TMP_Text jumpBindingText;

        public Button buildRebindButton;
        public TMP_Text buildBindingText;

        public Button usingWeaponRebindButton;
        public TMP_Text usingWeaponBindingText;

        public Button shottingRebindButton;
        public TMP_Text shottingBindingText;

        private InputAction jumpAction;
        private InputAction buildAction;
        private InputAction usingWeaponAction;
        private InputAction shottingAction;
        void Start()
        {
            jumpAction = actions.FindAction("Player/Jump");
            buildAction = actions.FindAction("Player/Build");
            usingWeaponAction = actions.FindAction("Player/Shot");
            shottingAction = actions.FindAction("Player/Shotting");

            LoadSavedRebinds();
            RefreshUI(jumpAction,jumpBindingText);
            RefreshUI(buildAction,buildBindingText);
            RefreshUI(usingWeaponAction,usingWeaponBindingText);
            RefreshUI(shottingAction,shottingBindingText);

            jumpRebindButton.onClick.AddListener(() => StartRebind(jumpAction, jumpBindingText));
            buildRebindButton.onClick.AddListener(() => StartRebind(buildAction, buildBindingText));
            usingWeaponRebindButton.onClick.AddListener(() => StartRebind(usingWeaponAction, usingWeaponBindingText));
            shottingRebindButton.onClick.AddListener(() => StartRebind(shottingAction, shottingBindingText));

        }
        public void StartRebind(InputAction action, TMP_Text bindingText)
        {
            bindingText.text = "Press a key...";
            action.Disable();
            action.PerformInteractiveRebinding()
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(operation =>
                {
                    operation.Dispose();
                    action.Enable();
                    SaveRebinds();
                    RefreshUI(action,bindingText);
                })
                .Start();
        }
        private void RefreshUI(InputAction action, TMP_Text bindingText)
        {
           bindingText.text = InputControlPath.ToHumanReadableString(
                action.bindings[0].effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice
            );
          
        }
        private void SaveRebinds()
        {
            string json = actions.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString("rebinds", json);
        }
        private void LoadSavedRebinds()
        {
            if (PlayerPrefs.HasKey("rebinds"))
            {
                actions.LoadBindingOverridesFromJson(PlayerPrefs.GetString("rebinds"));
            }
        }
    }

}

