using UnityEngine;
namespace Effects
{
    public enum EffectType
    {
        Once,
        Persistent,
        Timed,
        Stateful,
    }
    public class EffectStat
    {
        public EffectType Type;
        public float MaxHp;
        public float Damage;
        public float Hp;
        public float Atk;
        public float Def;
        public float Movespeed;
        public float PersistentTime;
        public float BreakTime;
        public EffectStat(EffectStat other)
        {
            Type = other.Type;
            Def = other.Def;
            Damage = other.Damage;
            Atk = other.Atk;
            Movespeed = other.Movespeed;
            PersistentTime = other.PersistentTime;
            MaxHp = other.MaxHp;
            Hp = other.Hp;
            BreakTime = other.BreakTime;
        }
        public EffectStat(){}
        public EffectStat Copy()
        {
            return new EffectStat(this);
        }
        public  void ResetInfo(EffectStat other)
        {
            this.Damage = other.Damage;
            this.Atk = other.Atk;
            this.Def = other.Def;
            this.Movespeed = other.Movespeed;
            this.PersistentTime = other.PersistentTime;
            this.MaxHp = other.MaxHp;
            this.BreakTime = other.BreakTime;
            this.Hp = other.Hp;
            this.Type = other.Type;
        }
        public void Opposite()
        {
            Damage = -this.Damage;
            Atk = -this.Atk;
            Def = -this.Def;
            Movespeed = -this.Movespeed;
            MaxHp = -this.MaxHp;
            Hp = -this.Hp;
        }
        public void ResetInfo()
        {
            // 数值类型重置为 0
            this.Damage = 0f;
            this.Atk = 0f;
            this.Def = 0f;
            this.Movespeed = 0f;
            this.Hp = 0f;
            this.MaxHp = 0f;
            // 时间/持续类属性重置
            this.PersistentTime = 0f;
            this.BreakTime = 0f;

            // 枚举类型重置为默认值（Once）
            this.Type = EffectType.Once;
        }

    }
}
