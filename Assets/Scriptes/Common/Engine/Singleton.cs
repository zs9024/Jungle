using System;

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

public interface IInit{
    void Init();
}

