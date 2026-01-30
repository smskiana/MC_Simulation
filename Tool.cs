using Sirenix.OdinInspector;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public static class Tool
{
    public static int FloorDiv(float a, float b)
    {
        float q = a / b;
        return Mathf.FloorToInt(q);
    }

   
    public static Vector3Int GetMaxAxisVector(Vector3 v)
    {
        float x = v.x;
        float z = v.z;

        if (x == 0f && z == 0f)
            return Vector3Int.zero;

        float ax = Mathf.Abs(x);
        float az = Mathf.Abs(z);

        int sx = x > 0f ? 1 : (x < 0f ? -1 : 0);
        int sz = z > 0f ? 1 : (z < 0f ? -1 : 0);

        // 控制斜向的判定阈值（可调整）
        const float diagFactor = 0.41421356f; // tan(22.5°)

        if (ax > az)
        {
            if (az / ax > diagFactor)
                return new Vector3Int(sx, 0, sz);   // 斜方向
            return new Vector3Int(sx, 0, 0);       // X方向
        }
        else
        {
            if (ax / az > diagFactor)
                return new Vector3Int(sx, 0, sz);   // 斜方向
            return new Vector3Int(0, 0, sz);       // Z方向
        }
    }
    public static Vector3Int AlignWithOffset(Vector3 pos, Vector3Int axis)
    {
        // 1. 对 x/z 方向加上 axis
        float newX = pos.x + axis.x;
        float newY = pos.y; // y 不处理
        float newZ = pos.z + axis.z;

        // 2. 转为整数
        int xInt = Align(newX);
        int yInt = Align(newY);
        int zInt = Align(newZ);

        // 3. 保证 x/z 方向偏移 >= 1
        if (Mathf.Abs(xInt + 0.5f - pos.x) < .5f&& Mathf.Abs(zInt + 0.5f - pos.z) < .5f)
        {
            if (Mathf.Abs(axis.x) > 0)
            {
                xInt += axis.x;
               
            }
            else if (Mathf.Abs(axis.z) > 0)
            {
                zInt += axis.z;
            }          
        }
        // 4. 返回 Vector3 偏移结果
        return new Vector3Int(
            xInt,
            yInt,
            zInt
        );
    }
    public static Vector3 ToCenter(Vector3Int pos) => pos + (Vector3.one * .5f);
    public static int Align(float n) => Mathf.FloorToInt(n);
    public static Vector3Int Align(Vector3 v)
    {
        var buf = new Vector3Int(
            Mathf.FloorToInt(v.x),
            Mathf.FloorToInt(v.y),
            Mathf.FloorToInt(v.z));
        return buf;
    }
    public static int UpAlign(float n)
    {
        return n > 0 ? ((int)n) + 1 : (int)n;
    }
    public static void SwapRemove<T>(this List<T> list, int index)
    {
        int last = list.Count - 1;
        if (last == index)
        {
            list.RemoveAt(last);
            return;
        }
        (list[index], list[last]) = (list[last], list[index]);
        list.RemoveAt(last);
    }
    public static bool GetGroundY(float x, float z, out float y)
    {
        Vector3 start = new(x, 10000f, z);

        if (Physics.Raycast(start, Vector3.down, out RaycastHit hit, Mathf.Infinity))
        {
            y = hit.point.y;
            return true;
        }

        y = 0f;
        return false;
    }
    public static Dictionary<K,V> CloneNewInstance<K,V>(this Dictionary<K,V> valuePairs)=>new(valuePairs);

    /// <summary>
    /// 旋转（int）
    /// </summary>
    /// <param name="forward"> 应该为（0，0，1）这种标准向量 </param>
    /// <param name="up">应该为（0，1，0）这种标准向量 </param>
    /// <param name="point">任意的点 </param>
    /// <returns></returns>
    public static Vector3Int Rotation(Vector3Int forward, Vector3Int up, Vector3Int point)
    {
        // 防止非法方向
        if (forward == Vector3Int.zero || up == Vector3Int.zero)
            return Vector3Int.zero;
        Vector3Int right = up.Cross(forward);
        // forward 和 up 不能共线
        if (right==Vector3Int.zero)
            return Vector3Int.zero;

        return
            right * point.x +
            up * point.y +
            forward * point.z;
    }
    public static Vector3Int Cross(this Vector3Int a ,Vector3Int b)
    {
        int x =   a.y * b.z - b.y * a.z;
        int y = -(a.x * b.z - b.x * a.z);
        int z =   a.x * b.y - b.x * a.y;
        return new Vector3Int( x, y, z );
    }
    public static Vector3Int GetForWorldInt(Transform transform)
    {
        Vector3 dir = transform.forward;

        float ax = Mathf.Abs(dir.x);
        float az = Mathf.Abs(dir.z);
        if (ax >= az)
            return new Vector3Int(dir.x > 0 ? 1 : -1, 0, 0);
        else
            return new Vector3Int(0, 0, dir.z > 0 ? 1 : -1);
    }

}
[System.Serializable]
public class SerializableDic<Key, Value>
{
    [System.Serializable]
    private struct Pair
    {
        public Key key;
        public Value value;
        public Pair(Key key, Value value)
        {
            this.key = key;
            this.value = value;
        }
    }
    [ReadOnly]
    [SerializeField]
    [ListDrawerSettings]
    private List<Pair> serializableDic = new();
    [ShowInInspector]
    [ReadOnly]
    private Dictionary<Key,Value> dic = null;
    public IEnumerable<Value> Values
    {
        get {
            if (dic==null)
            {
                foreach(Pair pair in serializableDic)
                {
                    yield return pair.value;
                }
            }
            else
            {

                foreach(var value in dic.Values)
                {
                    yield return value;
                }
            }
        }

    }
    public IEnumerable<Key> Keys
    {
        get
        {
            if (dic==null)
            {
                foreach (Pair pair in serializableDic)
                {
                    yield return pair.key;
                }
            }
            else
            {
                foreach (var key in dic.Keys)
                {
                    yield return key;
                }
            }
        }
    }
    public IEnumerable<(Key key,Value value)> Pairs
    {
        get
        {
            if (dic==null)
            {
                foreach (Pair pair in serializableDic)
                {
                    yield return (pair.key, pair.value);
                }
            }
            else
            {
                foreach (var pair in dic)
                {
                    yield return (pair.Key, pair.Value);
                }
            }
        }
    }
    public Dictionary<Key,Value> GetDic()
    {
        if (Application.isPlaying&& this.dic != null)
           return this.dic.CloneNewInstance();

        var dic = new Dictionary<Key,Value>();
        if(serializableDic==null) return dic;
        foreach(var pair in serializableDic)
        {
            dic.Add(pair.key, pair.value);
        }
        return dic; 
    }
    public bool TryGetValue(Key key,out Value value)
    {
        if (!Application.isPlaying)
        {
            value = default;
            int buf = serializableDic.FindIndex(x => x.key.Equals(key));
            if(buf == -1) 
                return false;
            else
            {
                value = serializableDic[buf].value;
                return true;
            }
        }
        else
        {
            dic ??= GetDic();
            return dic.TryGetValue(key, out value);
        }
    }


    
    public bool ContainKey(Key key)
    {
        if (!Application.isPlaying)
        {
            int buf = serializableDic.FindIndex(x => x.key.Equals(key));
            if (buf == -1)
                return false;
            else
                return true;
        }
        else
        {
            dic ??= GetDic();
            return dic.ContainsKey(key);
        }    
    }
    [Button("从真字典保存")]
    public void Save()
    {
        if (dic != null)
        {
            serializableDic.Clear();
            foreach(var pair in dic)
            {
                serializableDic.Add(new(pair.Key, pair.Value));
            }
        }
    }
    [Button("构建/重构字典")]
    public void TurnToDic()
    {
        dic = GetDic();
    }
    [ReadOnly]
    [Button("添加")]
    public void Add(Key key, Value Value)
    {
        if (Application.isPlaying)
        {
            dic??=GetDic();
            dic.Add(key, Value);
        }
        else
        {
            serializableDic ??= new List<Pair>();
            int buf = serializableDic.FindIndex(x => x.key.Equals(key));
            if (buf == -1)
            {
                serializableDic.Add(new Pair()
                {
                    key = key,
                    value = Value
                });
            }
            else
            {
                serializableDic[buf] = new Pair()
                {
                    key = key,
                    value = Value
                };
            }
        }
       
    }
    [Button("移除")]
    public void Remove(Key key)
    {
        if(Application.isPlaying)
        {
            dic ??= GetDic();
            dic.Remove(key);
        }
        else
        {
            if (serializableDic == null) return;
            int pos = serializableDic.FindIndex(x => x.key.Equals(key));
            if (pos == -1) return;
            serializableDic.RemoveAt(pos);
        }
       
    }

    [Button("清空")]
    public void Clear()
    {
        if (Application.isPlaying)
        {
            dic?.Clear();
        }
        else
        {
            dic?.Clear();
            serializableDic?.Clear();
        }
    }
}
public class SeriaclizableSet<T>
{
    [ShowInInspector]
    [ReadOnly]
    private readonly List<T> set= new();
    private HashSet<T> hashset = null;

    public bool Contain(T value)
    {
        if (Application.isPlaying)
        {
            hashset ??= GetHashset();
            return hashset.Contains(value);
        }
        return set.Contains(value);
    }
    public HashSet<T> GetHashset()
    {
        if(hashset != null) return hashset;
        var set = new HashSet<T>(this.set);
        return set;
    } 

    [Button("添加")]
    public void Add(T value)
    {
        if (Application.isPlaying)
        {
            hashset ??= GetHashset();
            hashset.Add(value);
            return;
        }
        if (set.Contains(value)) return;
        set.Add(value);
    }
    [Button("移除")]
    public void Remove(T value)
    {
        if (Application.isPlaying)
        {
            hashset??= GetHashset();
            hashset.Remove(value);
            return;
        }
       set.Remove(value);
    }

}



