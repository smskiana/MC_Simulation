using MyGUI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Manager
{
    public class InputManager : SingletonMono<InputManager>
    {
        [SerializeField]
        private InputActionAsset defaultInputActions;
        [SerializeField]
        private InputActionAsset inputActions;
        public InputActionAsset InputActions { get => inputActions; }
        public InputActionMap PlayerInput { get => inputActions.FindActionMap("Player"); }
        public void LockPlayerInput() => inputActions.FindActionMap("Player").Disable();
        public bool IsPlayerInputActive()=>inputActions.FindActionMap("Player").enabled;
        public void UnlockPlayerInput() => inputActions.FindActionMap("Player").Enable();
        public void OnDestroy()
        {
            inputActions.Disable();
        }
        protected override void SingletonAwake()
        {
            inputActions.FindActionMap("System").FindAction("Esc").performed += ctx =>
            {
                if(UIManager.Instance != null && UIManager.Instance.AnyUIActive)
                {
                    UIManager.Instance.CloseAll();
                    PlayerInput.Enable();
                } 
            };

        }
    }
}
