
using UnityEngine;

public abstract class InfoSet : MonoBehaviour
{
    public abstract void SetInfo(Info val);

    public abstract void ReSetState();
}

public abstract class InfoSetNoObject
{
    public abstract void SetInfo(Info val);
    public InfoSetNoObject() { }
}

