using System;
using System.Collections.Generic;
using UnityEngine;

namespace _3C.Actors
{
    public class View : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] protected Transform Appearance;
        protected readonly Dictionary<string,Action<string>> animationCallbacks= new Dictionary<string,Action<string>>();
        public Animator Animator { get => animator; }
        public void PlayerAniMation(string name, Action<string> callback = null)
        {
            if (animator != null)
                animator.SetBool(name, true);

            animationCallbacks.TryGetValue(name, out var oldcallback);

            // 去重：已经存在就不再添加
            if (oldcallback != null)
            {
                foreach (var cb in oldcallback.GetInvocationList())
                {
                    if (cb == (Delegate)callback)
                        return;
                }
            }

            animationCallbacks[name] = oldcallback + callback;
        }
        public void StopAniMation(string name, Action<string> callback = null)
        {
            if (animator != null)
                animator.SetBool(name, false);
            if (animationCallbacks.TryGetValue(name, out var oldcallback))
            {
                oldcallback -= callback;
                if (oldcallback == null)
                    animationCallbacks.Remove(name);
                else
                    animationCallbacks[name] = oldcallback;
            }
        }
        public void StopAniMation(string name)
        {
            if(animator != null)
                animator.SetBool(name, false);
            animationCallbacks.Remove(name);
        }
        public void TriggerAnimationCallback(string info)  
        {
            string[] strings = info.Split('_');
            string name, infos;
            if (strings.Length >= 2)
            {
                name = strings[0];
                infos = strings[1];
            }
            else if(strings.Length >= 1)
            {
                name=strings[0];
                infos = "";
            }
            else
            {
                return;
            }
            if(animationCallbacks.TryGetValue(name, out var callback))
            {
                if(callback != null) callback(infos);
                else
                    animationCallbacks.Remove(name);
            }
        }
    }
}
