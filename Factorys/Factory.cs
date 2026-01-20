using Effects;
using StatSystems.Store;
using System.Collections.Generic;
using UnityEngine;

public class Factory
{
    public InfoStorer InfoStorer { get =>InfoStorer.Instance;}
    private readonly Queue<EffectStat> EffectStatsPool = new();
    private readonly Dictionary<string, Queue<GameObject>> appearancePool;
    public readonly int MaxGameObjectCount;
    public readonly int MaxEffectStatsCount;
    private static Factory instance;
    
   
    
    protected Factory(int a,int b)
    {
         this.MaxEffectStatsCount = b;
         this.MaxGameObjectCount = a;
    }
    public static Factory GetFactory(int MaxGameObjectCount = 64, int MaxEffectStatsCount= 256)
    {
        instance ??= new Factory(MaxGameObjectCount,MaxEffectStatsCount);
        return instance;
    }
    private static Effect CreateEffect(EffectStat effectStats)
    {
       var buf = effectStats.Type switch
        {
            EffectType.Once =>  new Effect(),
            EffectType.Persistent =>  new PersistentEffect(),
            EffectType.Timed => new TimedEffect(),
           
            _ => null

        };

        if(buf == null)
        {
            Debug.LogWarning($"未知 EffectType: {effectStats.Type}, 使用默认 Effect");
            return new Effect();
        }
        return buf;
         
    }
    public  Effect GetEffect(EffectStat effectStats)
    {
        if (effectStats == null)
        {
            return null;
        }
        if (EffectStatsPool.Count > 0)
        {
            var buf = EffectStatsPool.Dequeue();
            buf.ResetInfo(effectStats);
            return CreateEffect(buf);
        }
        else
        {
            return CreateEffect(effectStats);       
        }       
    }
    public EffectStat GetEffectStat()
    {
        if (EffectStatsPool.Count > 0)
        {
            var buf = EffectStatsPool.Dequeue();
            buf.ResetInfo();
            return buf;
        }
        else
        {
            return new EffectStat();
        }
    }
    public Effect TurnToEffect(EffectStat effects)
    {
        return CreateEffect(effects);
    }
    public void RecycleEffectStat(EffectStat effect)
    {
        if(effect == null) return;
        EffectStatsPool.Enqueue(effect);
    }
    public void RecycleEffect(Effect effect)
    {
        if(effect == null) return;
       
        effect.Clear(); 
    }
} 
