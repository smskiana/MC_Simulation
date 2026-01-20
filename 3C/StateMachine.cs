using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
namespace _3C
{
    [Flags]
    public enum MachineState
    {
        None = 0,   
        New = 1,
        Pause = 1<<1,
        SwitchLocked = 1<<2,
        SwitchFree = 1<<3,
        Error = 1<<4,
        /// <summary>
        /// 仅用于判断
        /// </summary>
        Running = SwitchLocked|SwitchFree
    }
    public interface IControl
    {
        public abstract bool IsActive(string name);
    }
    public class StateMachine :MonoBehaviour
    {   
        [SerializeField]  private StateMachineInfo info;
        [ShowInInspector] private IControl controller;
        [SerializeField]  private string DefaultStateName = "Idle";
        [ShowInInspector] [ReadOnly] private MachineState state = MachineState.New;
        [ShowInInspector] [ReadOnly] private Dictionary<string, State> Allstate = null;
        [ShowInInspector] public State Current => statesStack.Count > 0 ? statesStack.Peek() : null;
        private List<State> bufferList = null; 
        private readonly Stack<State> statesStack = new();
        private string nextState;
        private string pushState;
        private bool needPop;
        public string NextState { get=>nextState; set {
               
                switch (state)
                {
                    case MachineState.SwitchLocked:
                        if (value == null) nextState = null;
                        else if (Current.CanBeInterrupt(value)) { state = MachineState.SwitchFree;nextState = value; }
                        else if (Current.CanBeNextstate(value)) {  nextState = value; }
                        break;
                    case MachineState.SwitchFree:
                        if(value==null) nextState = null;
                        else if (Current.CanBeNextstate(value)) { nextState = value; }
                        break;
                }
            } }
        public string PushState { get =>pushState; set {
                if (value == null)
                {
                    pushState = null;
                    return;
                }
                if(Current.CanBeNextstate(value))
                    pushState = value;
            } }
        public bool NeedPop {  get =>needPop; set {
                if (statesStack.Count > 1)
                {
                    needPop = value;
                    return;
                }
                else needPop = false;
            } }
        public MachineState State { get => state;}
        public bool SwitchLocked { get => (state & MachineState.SwitchLocked) != MachineState.None; }
        public bool IsRunning { get => (State & MachineState.Running)!=MachineState.None; }
        public void Lock() => state = MachineState.SwitchLocked;
        public void Unlock() => state = MachineState.SwitchFree;
        private void Awake()
        {
            controller ??= GetComponent<IControl>();   
          
        }
        private void Start()
        {
            Init();
        }
        private void Update()
        {
            if(Current == null)
            {
                Debug.LogError($"{this}:状态栈为空为null");
                return;
            }
            if (!MachineState.Running.HasFlag(state))return; 
            Current.TryUpdateState();
            Current.Update();
        }
        public void SetState(MachineState state)
        {
            var switchLock_Free = MachineState.SwitchFree | MachineState.SwitchLocked;
            // 禁止同时 Free + Locked
            if ((state & switchLock_Free) == switchLock_Free)
                return;
            bool switchToRun = (MachineState.Running & state) != MachineState.None;
            bool switchToPause = (MachineState.Pause & state) != MachineState.None;
            bool switchFromRun = (MachineState.Running & this.state) != MachineState.None;
            bool switchFromPause = (MachineState.Pause & this.state) != MachineState.None;
            if (switchToPause && switchFromRun) Current.Pause();
            if (switchToRun && switchFromPause) Current.Resume();
            this.state = state;
        }
        public void Init()
        {
            Allstate = new Dictionary<string, State>();
            foreach (var (key, value) in info.SerializableDic.Pairs)
            {
                var buffer = Object.Instantiate(value);
                buffer.Init(controller, this);
                Allstate.Add(key, buffer);
            }         
            if (string.IsNullOrEmpty(DefaultStateName)||!Allstate.ContainsKey(DefaultStateName))
            {
                Debug.LogError($"{this}:异常的{nameof(Init)}，{nameof(DefaultStateName)} 为null");
                return;
            }
            Exit();
            statesStack.Push(Allstate[DefaultStateName]);
            Current.Enter();
            state = MachineState.SwitchFree;
        }
        private void Exit()
        {
            foreach (var state in statesStack)
            {
                state.Exit();
            }
            statesStack.Clear();
        }
        internal void StateSwitch(string name)
        {
            if(!Allstate.TryGetValue(name,out var state)) return;
            statesStack.Pop().Exit();
            statesStack.Push(state);
            state.Enter();          
        }
        internal void StatePush(string name)
        {
            if(!Allstate.TryGetValue(name, out var state)) return;
            Current.Pause();
            statesStack.Push(state);
            state.Enter();
        }
        internal void StatePop()
        {       
            if (statesStack.Count > 1)
            {
                statesStack.Pop().Exit();
                Current.Resume();
            }
        }
        public bool TryGetState(string key, out State value)
        {
            return Allstate.TryGetValue(key,out value);
        }
        //运行时
        public void AddStateClone(State state)
        {
            if(state == null) return;
            bufferList ??= new();
            if(Allstate.TryGetValue(state.ID,out var Oldstate))
            {
                if(Oldstate.Orgin == state.Orgin)
                {
                    if (Oldstate.ID == Current.ID)
                        StateSwitch(state.ID);
                    return;
                }     
                var index = bufferList.FindIndex(x=>x.Orgin == state.Orgin);
                State buf = null;
                if (index < 0)
                {
                    buf = Object.Instantiate(state);
                    buf.Init(controller, this);
                }
                else
                {
                    buf = bufferList[index];
                    buf.OnReuse();
                    bufferList.RemoveAt(index);
                }
                Allstate[state.ID] = buf;
                Oldstate.TransferActionTo(state);
                if (Oldstate.ID == Current.ID)
                    StateSwitch(state.ID);
                Oldstate.OnRecycle();
                bufferList.Add(Oldstate);
            }
            else
            {
                var buf = Object.Instantiate(state);
                buf.Init(controller, this);
                Allstate.Add(state.ID, buf);
            }      
        }
    }
}