using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace _3C
{
    [CreateAssetMenu(fileName = "MoveState", menuName = "State/Default")]
    public class State :ScriptableObject
    {
        [SerializeField] protected string orgin;
        [SerializeField] protected string id;
        [SerializeField][ReadOnly] protected bool NewState = true;
        public IControl Controller {  get;private set; }
        public StateMachine Machine {  get; private set; }
        [SerializeField]
        protected List<string> serializableNextState;
        [SerializeField]
        protected List<string> serializableInterrupt;
        [SerializeField]
        protected List<string> serializableDefaultSwitch;
        [ShowInInspector]
        [ReadOnly]
        private HashSet<string> nextState ;
        [ShowInInspector]
        [ReadOnly]
        private  HashSet<string> interrupt;
        [ShowInInspector]
        [ReadOnly]
        private  HashSet<string> defaultSwitch;
#if UNITY_EDITOR
        public IEnumerable<string> SerializableNextState { get => serializableNextState;  }
        public IEnumerable<string> SerializableInterrupt { get => serializableInterrupt; }
        public IEnumerable<string> SerializableDefaultSwitch { get => serializableDefaultSwitch; }
        public string ID { get => id;}
        public string Orgin { get => orgin;}
#endif
        public event Action<State> OnEnter;
        public event Action<State> OnExit;
        public event Action<State> OnPause;
        public event Action<State> OnResume;

        public void TransferActionTo(State state)
        {
            state.OnEnter += OnEnter;
            state.OnExit += OnExit;
            state.OnPause += OnPause;
            state.OnResume += OnResume;
            OnEnter=null;
            OnExit=null;
            OnPause=null;
            OnResume=null;
        }
        public virtual void Awake()
        {
            if (!NewState) return;
            InitInfo();
            NewState = false;
        }
        protected virtual void InitInfo() { }
        private void InitStates()
        {
            defaultSwitch = new HashSet<string>(serializableDefaultSwitch);
            interrupt = new HashSet<string>(serializableInterrupt);  
            nextState = new HashSet<string>(serializableNextState);
            serializableDefaultSwitch = null;
            serializableInterrupt = null;
            serializableNextState = null;         
        }
        public bool CanBeNextstate(string name)=> name != null && nextState.Contains(name);
        public bool CanBeInterrupt(string name) => name != null && interrupt.Contains(name);
        public void Init(IControl control, StateMachine machine)
        {
            if (this.Machine != null && this.Machine != machine)
            {
                Debug.LogError($"{name} 被多个 StateMachine 使用！");
                return;
            }
            this.Controller = control;
            this.Machine = machine;
            InitStates();
        }
        public virtual void Update() { }
        public virtual void Enter() => OnEnter?.Invoke(this);
        public virtual void Exit() => OnExit?.Invoke(this);
        public virtual void Pause() => OnPause?.Invoke(this);
        public virtual void Resume() => OnResume?.Invoke(this);
        public virtual void OnRecycle() { }
        public virtual void OnReuse() { }
        public virtual void TryUpdateState()
        {
            string nextState;
            bool isSwitch = true;
            bool isNeedpop = Machine.NeedPop;

            if(Machine.NextState != null)
            {
                nextState = Machine.NextState;
            }
            else
            {
                nextState = Machine.PushState;
                isSwitch = false;   
            }

            if (Machine.SwitchLocked && (!CanBeInterrupt(nextState)&&!isNeedpop)) return;

            if(nextState != null)
            {
                if(isSwitch) 
                    Machine.StateSwitch(nextState);
                else
                    Machine.StatePush(nextState);      
            }
            else if (isNeedpop)
            {
                Machine.StatePop();
            }
            else
            {
                foreach (string state in defaultSwitch)
                {
                    if (Controller.IsActive(state))
                    {
                        Machine.StateSwitch(state);
                        return;
                    }
                }
            }


            Machine.NextState = null;
            Machine.PushState = null;
            Machine.NeedPop = false;
        }        
    }
}

