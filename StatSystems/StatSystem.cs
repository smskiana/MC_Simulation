using _3C.Actors;
using Effects;
using Sirenix.OdinInspector;
using StatSystems.Stats;
using StatSystems.Store;
using System;
using UnityEngine;
using Object = UnityEngine.Object;
namespace StatSystems { 
    public class StatSystem : MonoBehaviour,IDamageSource,IDamageTarget,IEffectTarget 
    { 
        [SerializeField]  protected Actor actor; 
        [SerializeField]  protected ActorInfo characterInfo;
        [ShowInInspector] protected StatContainer container; 
        [ShowInInspector] protected EffectSystem effectSystem;
        protected StatCalculator calculator;
        protected DamageCalculator damageCalculator;
        /// <summary>
        /// 自己，旧，新
        /// </summary>
        public event Action<StatSystem, float, float> MaxHpChange; 
        /// <summary>
        /// 自己，旧，新
        /// </summary>
        public event Action<StatSystem, float, float> CurHpChange;
        public event Action<StatSystem, Effect> EffectExcuteInMe; 
        public event Action<StatSystem> CurEqZero; 

        public float MaxHp { get => container.ThisFrameStat.maxHp; }
        public float Hp {get => container.ThisFrameStat.currentHp; }
        public float MoveSpeed { get=>container.ThisFrameStat.moveSpeed;}
        public void Awake() 
        { 
            container = new(characterInfo, this); 
            effectSystem = new EffectSystem(this); 
            calculator = new StatCalculator(this,characterInfo); 
            damageCalculator = new DamageCalculator(this,characterInfo);
        }
        public ActorStat  Stat { get => container.ThisFrameStat.CloneNewInstance() as ActorStat; }
        public void TriggerMaxHpChange(float oldvalue, float newValue) => MaxHpChange?.Invoke(this, oldvalue, newValue); 
        public void TriggerEffectExcuteInMe(Effect effect) => EffectExcuteInMe?.Invoke(this, effect); 
        public void TriggerCurHpChange(float oldvalue, float newValue) => CurHpChange?.Invoke(this, oldvalue, newValue); 
        public void ResetToBasicStat(ActorStat stat)
        {
            stat?.Reset(container.Stat);
        }
        public void LateUpdate()
        { 
            effectSystem.Tick();
            var buf = calculator.Calculator(effectSystem.Applylist);
            container.SetChange(buf);
        }
        public DamagePackage GetDamage()
        {
            var damage = container.GetDamage();   
            damage.state = PackageState.Preparing;
            damage.Source = this;
            var buf = DamageCalculator.ModifyDamage(damage,effectSystem.ModifyDamageList);
            buf.state = PackageState.Sending;
            return buf;
        }
        public void SetDamage(DamagePackage damage)
        {
            damage.state = PackageState.Recepted;
            var buf = damageCalculator.SetDamage(damage, container.ThisFrameStat, effectSystem.ModifyDamageList);
            damage.state = PackageState.Over;
            container.SetBasicChange(buf);
            actor.SetHurt(damage);
        }
        [Button("普通攻击")]
        public void GetDamage(float value)
        {
            DamagePackage damage = new(value);
            SetDamage(damage);
        }
        public void AddEffect(Effect effect) => effectSystem.Register(effect);
        [Button("移除效果")]
        public void RemoveEffect(Effect effect)=>effectSystem.Unregist(effect.Id);
        [Button("移除效果（ID）")]
        public void RemoveEffect(int effectId)=>effectSystem.Unregist(effectId);

#if UNITY_EDITOR
        /// <summary>
        /// 测试用
        /// </summary>
        [Button("添加效果")]
        public void SetEffectClone(Effect effect)
        {
            var buf = Object.Instantiate(effect);
            effectSystem.Register(buf);
        }
#endif

    } 
}