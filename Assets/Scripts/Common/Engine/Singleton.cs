using System;
using UnityEngine;

/// <summary>
/// 泛型单例类，所有单例可继承此类，并实现IInit接口
/// by zs
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> where T: IInit,new()
{
    protected static T instance;

    static Singleton()
    {
        T local = default(T);
        Singleton<T>.instance = (local == null) ? Activator.CreateInstance<T>() : default(T);
        Singleton<T>.instance.Init ();
    }

    protected Singleton()
    {
    }
    
    public static T Instance
    {
        get
        {
            return Singleton<T>.instance;
        }
    }
}

public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static readonly object _lockObj;

    static SingletonMono()
    {
        _lockObj = new object();
        if (instance == null)
        {
            lock (_lockObj)
            {
                if (instance == null)
                {
                    var go = new GameObject(typeof(T).ToString());
                    instance = go.AddComponent<T>();

                    DontDestroyOnLoad(go);
                }
            }
        }
    }

    public static T Instance
    {
        get
        {
            return SingletonMono<T>.instance;
        }
    }
}

public interface IInit{
    void Init();
}

