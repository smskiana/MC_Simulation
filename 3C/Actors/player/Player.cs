using Bags;
using Manager;
using StatSystems;
using UnityEngine;
using Sirenix.OdinInspector;
using StatSystems.Stats.Items;
using UnityEngine.InputSystem;
using Items;
namespace _3C.Actors.player
{
    public interface IHandUseBottonDown
    {
        public abstract void HandUseBottonDown();
    }
    public interface IHandUseBottonUp 
    {
        public abstract void HandUseBottonUp();
    }
    public interface IHandAimBottonUp 
    { 
        public abstract void HandAimBottonUp();
    }
    public interface IHandAimBottonDown 
    {
        public abstract void HandAimBottonDown();
    }

    public class Player : Actor
    {
        public float Range;
        [SerializeField] private CameraController eye;
        [SerializeField] private float HandLenght = 2;
        [SerializeField] private Transform defaultIdolPos;
        [SerializeField] private StateMachine armMachine;
        [SerializeField] private PlayerBag playerBag;
        [SerializeField] private LayerMask MonsterMask;
        [FoldoutGroup("切换状态ID")]
        [LabelText("跳跃")]
        [SerializeField]
        private string jumpStateName;
        [FoldoutGroup("切换状态ID")]
        [LabelText("持有")]
        [SerializeField]
        private string hold = "Hold";
        [FoldoutGroup("切换状态ID")]
        [LabelText("使用")]
        [SerializeField]
        private string use = "Use";
        [FoldoutGroup("切换状态ID")]
        [LabelText("使用")]
        [SerializeField]
        private string aim = "Aim";
        private ItemStat laststat = null;
        [SerializeField] private Weapon weapon = null;
        [ShowInInspector] public PlayerView PlayerView { get => base.View as PlayerView; }
        [ShowInInspector] public Vector3Int BodyForwardInt => Tool.GetForWorldInt(DefaultIdolPos);
        public PlayerStatSystem  PlayerStatSystem { get => Stat as PlayerStatSystem; }
        public CameraController Eye { get => eye; set => eye = value; }
        public Transform DefaultIdolPos { get => defaultIdolPos; set => defaultIdolPos = value; }
        public Vector3 EyeForward => eye.transform.forward;
        public override Vector3 MoveDir
        {
            get
            {
                if (DefaultIdolPos == null) return base.MoveDir;
                else return DefaultIdolPos.localToWorldMatrix * base.MoveDir;
            }
            set => base.MoveDir = value;
        }
        public PlayerBag PlayerBag { get => playerBag; }
        public StateMachine ArmMachine { get => armMachine;}
        public ItemStat Laststat { get => laststat;}
        public string Aim { get => aim; }
        public string Hold { get => hold;}
        public string Use { get => use;}
        public Weapon Weapon { get => weapon;}

        public override void InitEvent()
        {
            base.InitEvent();
            if (WorldManager.Instance != null)
            {
                var actions = InputManager.Instance.InputActions.FindActionMap("Player");
                actions["Move"].performed += Onmove;
                actions["Move"].canceled += Onmove;
                actions["Attack"].performed += ctx =>
                {
                    if (armMachine.Current is IHandUseBottonDown state) state.HandUseBottonDown();
                };
                actions["Attack"].canceled += ctx =>
                {
                    if (armMachine.Current is IHandUseBottonUp state) state.HandUseBottonUp();
                };
                actions["Jump"].performed += ctx => DefaultMachine.NextState = jumpStateName;
                actions["Aim"].performed += ctx => { if (ArmMachine.Current is IHandAimBottonDown hand) hand.HandAimBottonDown(); };
                actions["Aim"].canceled += ctx => { if (ArmMachine.Current is IHandAimBottonUp hand) hand.HandAimBottonUp(); };
                if (playerBag != null)
                {
                    playerBag.HotItemChanged += (sender) =>
                    {
                        laststat?.ItemInfo.HandItemSwitch(this);
                        sender?.ItemInfo.HandItemHold(this);
                        laststat = sender;
                        armMachine.NextState = hold;
                    };
                }            
            }
        }
        private void Onmove(InputAction.CallbackContext ctx)
        {
            var buf = ctx.ReadValue<Vector2>();
            MoveDir = new(buf.x, 0, buf.y);
        }
        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            Gizmos.color = Color.red;
            Gizmos.DrawLine(eye.transform.position, eye.transform.position + HandLenght * eye.transform.forward);
        }
        public bool CheckBlock(out Vector3 point)
        {          
            Ray ray = new(eye.transform.position, eye.transform.forward);
            if (Physics.Raycast(ray, out var info, HandLenght, base.layerMask))
            {
                point = info.point;
                return true;
            }
            point = new Vector3Int();
            return false;
        }
        public void SetCamera(Transform camera)
        {
            camera.SetParent(eye.transform);
            camera.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
        public IDamageTarget CheckDamageTarget(float distance)
        {
            Ray ray = new(eye.transform.position, eye.transform.forward);
            if(Physics.Raycast(ray,out var hitInfo, distance, MonsterMask))
            {
                return hitInfo.collider.GetComponent<IDamageTarget>();
            }
            return null;
        }


        public void HoldItem(Transform transform)
        {
            transform.TryGetComponent<Weapon>(out weapon);
            PlayerView.Hold(transform);
        }
    }

}


