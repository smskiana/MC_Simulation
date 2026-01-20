using Actors;
using Sirenix.OdinInspector;
using StatSystems;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace _3C.Actors
{
    public abstract class Actor :MonoBehaviour,IControl
    {
        #region ³£Á¿
        static readonly Vector3[] offsets =
        {
            Vector3.forward + Vector3.right,
            Vector3.forward + Vector3.left,
            Vector3.back    + Vector3.right,
            Vector3.back    + Vector3.left
        };
        #endregion
        private  Dictionary<string, Func<bool>> _event = new();
        public Vector3 LastSpeed;
        [SerializeField]private Vector3 moveDir = Vector3.zero;
        [SerializeField]protected LayerMask layerMask;
        [SerializeField]private float groundDistance = 1.6f;
        [SerializeField]private StatSystem stat;
        [SerializeField]private StateMachine defaultMachine;
        [SerializeField]private View view;
        [SerializeField]private CharacterController controller;
        [SerializeField]private Transform DamageSource;
        [FoldoutGroup("ÇÐ»»×´Ì¬ID")]
        [LabelText("ÒÆ¶¯")]
        [SerializeField] private string move = "Move";
        [FoldoutGroup("ÇÐ»»×´Ì¬ID")]
        [LabelText("´ý»ú")]
        [SerializeField] private string idle = "Idle";
        [FoldoutGroup("ÇÐ»»×´Ì¬ID")]
        [LabelText("µôÂä")]
        [SerializeField] private string fall = "Fall";
        [FoldoutGroup("ÇÐ»»×´Ì¬ID")]
        [LabelText("±»»÷·É")]
        [SerializeField] private string hurt = "Hurt";
        [ReadOnly] public int ID;
        public StatSystem Stat { get => stat; protected set => stat = value; }
        public CharacterController Controller { get => controller; }
        public Vector3Int Postion { get{ return Tool.Align(transform.position); } }
        public virtual Vector3 MoveDir { get => moveDir; set => moveDir = value; }
        public StateMachine  DefaultMachine { get => defaultMachine;}
        public View View { get => view;}
        public Vector3Int PostionInt { get => Tool.Align(this.transform.position); }
        public Vector3 HurtDir { get {
                if (DamageSource == null)
                    return Vector3.zero;
                return (transform.position - DamageSource.position).normalized;
            } }

        public string Move { get => move;}
        public string Idle { get => idle; }
        public string Fall { get => fall;}
        public string Hurt { get => hurt; }

        protected void Awake()
        {
            if (layerMask.value == 0)
                layerMask = LayerMask.GetMask("Ground");
        }
        public virtual void Start()=> InitEvent(); 
        public virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector3 re = transform.position +Vector3.up * .5f + (Vector3.forward + Vector3.right) * .5f;
            Gizmos.DrawLine(re,re+Vector3.down*groundDistance);

            Gizmos.color = Color.blue;
            re = transform.position + Vector3.up * .5f + (Vector3.forward + Vector3.left) * .5f;
            Gizmos.DrawLine(re,re+Vector3.down*groundDistance);

            Gizmos.color = Color.yellow;
            re = transform.position + Vector3.up * .5f + (Vector3.back + Vector3.right) * .5f;
            Gizmos.DrawLine(re, re+Vector3.down * groundDistance);

            Gizmos.color = Color.green;
            re = transform.position + Vector3.up * .5f + (Vector3.back + Vector3.left) * .5f;
            Gizmos.DrawLine(re,re+Vector3.down*groundDistance);

        }
        public virtual void InitEvent()
        {
            AddEvent(move, () =>
            {
                return CheckGround() && moveDir != Vector3.zero;
            });
            AddEvent(idle, () => {
                return CheckGround() && moveDir == Vector3.zero;
            });
            AddEvent(fall, () =>
            {
                return !CheckGround();
            });
        }
        public void AddEvent(string id, Func<bool> func)
        {
            _event ??= new Dictionary<string, Func<bool>>();
            if (!_event.ContainsKey(id))
            {
                _event.Add(id, func);
            }
            else
            {
                _event[id] = func;
            }
        }
        public virtual bool CheckGround()
        {
            bool isGround = false;
            foreach (var o in offsets)
            {
                Vector3 start = transform.position + o * .5f + Vector3.up * .5f;
                if (Physics.Raycast(start, Vector3.down, groundDistance, layerMask))
                {
                    isGround = true;
                    break;
                }
            }
            return isGround;
        }
        public bool IsActive(string name)
        {
            _event.TryGetValue(name, out var part);
            return part == null || part.Invoke();
        }
        public void SetHurt(DamagePackage package) 
        {
            if (package.Source is MonoBehaviour behaviour)
            {
                DamageSource = behaviour.transform;
                DefaultMachine.NextState = hurt;
            }
        }

    }
}

