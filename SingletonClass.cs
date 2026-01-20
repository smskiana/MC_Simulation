using UnityEngine;

public abstract class SingletonMono<T>:MonoBehaviour where T : SingletonMono<T>
{
    public static T Instance {  get; private set; }

    private void Awake()
    {
        if (Instance&&Instance!=this)
        {
            Object.Destroy(this.gameObject);
            return;
        }
        Instance = this as T;
        SingletonAwake();
    }
    protected abstract void SingletonAwake();
}

public abstract class SustainSingletonMono<T>:MonoBehaviour where T : SustainSingletonMono<T>
{
    public static T Instance {  get; private set; }

    private void Awake()
    {
        if (Instance&&Instance!=this)
        {
            Object.Destroy(this.gameObject);
            return;
        }
        Object.DontDestroyOnLoad(this.gameObject);
        Instance = (T)this;
        SingletonAwake();
    }
    private void OnDestroy()
    {
        if(Instance ==this) Instance = null;
        SingleOnDestroy();
    }
    protected virtual void SingletonAwake() { }
    protected virtual void SingleOnDestroy() { }
}

