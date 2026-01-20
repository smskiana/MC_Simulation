using Sirenix.OdinInspector;
using System;
using UnityEngine;
public enum BaseType
{
    None,
    Player,
    Enemy,
    Item,
}
[System.Serializable]
public class Info : ScriptableObject,IComparable<Info>, IEquatable<Info>
{
    [SerializeField]
    [ReadOnly] private bool NewOne;
    [SerializeField]
    private int id;
    [ShowInInspector]  
    public int ID { get {
         if(id ==0) return this.GetHashCode();
         return id;
        }}
    public void Awake()
    {
        if (NewOne)
        {
            InitInfo();
            NewOne = false;
        }
           
    }
    protected virtual void InitInfo()
    {
       
    }

    [SerializeField] private BaseType type;
    public BaseType Type { get => type;}
    public virtual int CompareTo(Info other)
    {
        if (other == null) return 1;
        int typeCompare = type.CompareTo(other.type);
        if (typeCompare != 0) return typeCompare;    
        return name.CompareTo(other.name);
    }
    public bool Equals(Info other)
    {
        if (other == null) return false;
        return GetInstanceID() == other.GetInstanceID();
    }
    public override bool Equals(object obj) => obj is Info other && Equals(other);
    public override int GetHashCode() => GetInstanceID();
}
