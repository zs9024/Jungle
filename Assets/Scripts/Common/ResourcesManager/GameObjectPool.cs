using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public sealed class GameObjectPool : SingletonMono<GameObjectPool>
{
    public enum StartupPoolMode { Awake, Start, CallManually };

    [System.Serializable]
    public class StartupPool
    {
        public int size;
        public GameObject prefab;
    }

    static List<GameObject> tempList = new List<GameObject>();

    Dictionary<GameObject, List<GameObject>> pooledObjects = new Dictionary<GameObject, List<GameObject>>();
    Dictionary<GameObject, GameObject> spawnedObjects = new Dictionary<GameObject, GameObject>();

    Dictionary<string, GameObject> pooledDic = new Dictionary<string, GameObject>();

    public StartupPoolMode startupPoolMode = StartupPoolMode.CallManually;
    public StartupPool[] startupPools;

    bool startupPoolsCreated;

    void Awake()
    {
        if (startupPoolMode == StartupPoolMode.Awake)
            CreateStartupPools();
    }

    void Start()
    {
        if (startupPoolMode == StartupPoolMode.Start)
            CreateStartupPools();
    }

    public static void CreateStartupPools()
    {
        if (!Instance.startupPoolsCreated)
        {
            Instance.startupPoolsCreated = true;
            var pools = Instance.startupPools;
            if (pools != null && pools.Length > 0)
                for (int i = 0; i < pools.Length; ++i)
                    CreatePool(pools[i].prefab, pools[i].size);
        }
    }

    public static void CreatePool<T>(T prefab, int initialPoolSize) where T : Component
    {
        CreatePool(prefab.gameObject, initialPoolSize);
    }
    public static void CreatePool(GameObject prefab, int initialPoolSize)
    {
        if (prefab != null && !Instance.pooledObjects.ContainsKey(prefab))
        {
            if(!Instance.pooledDic.ContainsKey(prefab.name))
            {
                Instance.pooledDic.Add(prefab.name, prefab);
            }

            var list = new List<GameObject>();
            Instance.pooledObjects.Add(prefab, list);

            if (initialPoolSize > 0)
            {
                bool active = prefab.activeSelf;
                prefab.SetActive(false);
                Transform parent = Instance.transform;
                while (list.Count < initialPoolSize)
                {
                    //var obj = ResourcesManager.Instance.InstantiateObject(prefab);
                    var obj = UnityEngine.Object.Instantiate(prefab) as GameObject;
                    obj.transform.parent = parent;
                    list.Add(obj);
                }
                prefab.SetActive(active);
            }
        }
    }

    public static T Spawn<T>(T prefab, Transform parent, Vector3 position, Quaternion rotation) where T : Component
    {
        return Spawn(prefab.gameObject, parent, position, rotation).GetComponent<T>();
    }
    public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
    {
        return Spawn(prefab.gameObject, null, position, rotation).GetComponent<T>();
    }
    public static T Spawn<T>(T prefab, Transform parent, Vector3 position) where T : Component
    {
        return Spawn(prefab.gameObject, parent, position, Quaternion.identity).GetComponent<T>();
    }
    public static T Spawn<T>(T prefab, Vector3 position) where T : Component
    {
        return Spawn(prefab.gameObject, null, position, Quaternion.identity).GetComponent<T>();
    }
    public static T Spawn<T>(T prefab, Transform parent) where T : Component
    {
        return Spawn(prefab.gameObject, parent, Vector3.zero, Quaternion.identity).GetComponent<T>();
    }
    public static T Spawn<T>(T prefab) where T : Component
    {
        return Spawn(prefab.gameObject, null, Vector3.zero, Quaternion.identity).GetComponent<T>();
    }

    public static GameObject Spawn(string name, Transform parent, Vector3 position, Quaternion rotation)
    {
        GameObject prefab;
        if(Instance.pooledDic.TryGetValue(name,out prefab))
        {
            return Spawn(prefab, parent, position, rotation);
        }

        return null;
    }

    public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
    {
        List<GameObject> list;
        Transform trans;
        GameObject obj;
        if (Instance.pooledObjects.TryGetValue(prefab, out list))
        {
            obj = null;
            if (list.Count > 0)
            {
                while (obj == null && list.Count > 0)
                {
                    obj = list[0];
                    list.RemoveAt(0);
                }
                if (obj != null)
                {
                    trans = obj.transform;
                    trans.parent = parent;
                    trans.localPosition = position;
                    trans.localRotation = rotation;
                    obj.SetActive(true);
                    Instance.spawnedObjects.Add(obj, prefab);
                    return obj;
                }
            }
            obj = ResourcesManager.Instance.InstantiateObject(prefab);
            trans = obj.transform;
            trans.parent = parent;
            trans.localPosition = position;
            trans.localRotation = rotation;
            Instance.spawnedObjects.Add(obj, prefab);
            return obj;
        }
        else
        {
            obj = ResourcesManager.Instance.InstantiateObject(prefab);
            trans = obj.GetComponent<Transform>();
            trans.parent = parent;
            trans.localPosition = position;
            trans.localRotation = rotation;
            return obj;
        }
    }
    public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position)
    {
        return Spawn(prefab, parent, position, Quaternion.identity);
    }
    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        return Spawn(prefab, null, position, rotation);
    }
    public static GameObject Spawn(GameObject prefab, Transform parent)
    {
        return Spawn(prefab, parent, Vector3.zero, Quaternion.identity);
    }
    public static GameObject Spawn(GameObject prefab, Vector3 position)
    {
        return Spawn(prefab, null, position, Quaternion.identity);
    }
    public static GameObject Spawn(GameObject prefab)
    {
        return Spawn(prefab, null, Vector3.zero, Quaternion.identity);
    }

    #region 加载资源并生成实例
    /// <summary>
    /// 本地加载并生成游戏对象
    /// </summary>
    /// <param name="assetPath"></param>
    /// <param name="parent"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="onSpawned">生成后回调</param>
    public static void SpawnFromLocal(string assetPath, Transform parent, Vector3 position, Quaternion rotation, Action<GameObject> onSpawned = null)
    {
        ResourcesManager.Instance.LoadAssetsLocal(assetPath, (GameObject resLoad) =>
        {
            if (resLoad != null)
            {
                GameObject go = Spawn(resLoad, parent, position, rotation);

                if (onSpawned != null)
                    onSpawned(go);
            }
        });
    }

    /// <summary>
    /// AssetBundle加载并生成游戏对象
    /// </summary>
    /// <param name="assetPath"></param>
    /// <param name="parent"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="onSpawned"></param>
    public static void SpawnFromBundle(string assetPath, Transform parent, Vector3 position, Quaternion rotation, Action<GameObject> onSpawned = null)
    {
        ResourcesManager.Instance.LoadAssetsBundle(assetPath, true, (GameObject resLoad) =>
        {
            if (resLoad != null)
            {
                GameObject go = Spawn(resLoad, parent, position, rotation);

                if (onSpawned != null)
                    onSpawned(go);
            }
        });
    }

    #endregion


    public static void Recycle<T>(T obj) where T : Component
    {
        Recycle(obj.gameObject);
    }
    public static void Recycle(GameObject obj)
    {
        GameObject prefab;
        if (Instance.spawnedObjects.TryGetValue(obj, out prefab))
            Recycle(obj, prefab);
        else
            ResourcesManager.Instance.Release(obj);
    }
    static void Recycle(GameObject obj, GameObject prefab)
    {
        Instance.pooledObjects[prefab].Add(obj);
        Instance.spawnedObjects.Remove(obj);
        obj.transform.parent = Instance.transform;
        obj.SetActive(false);
    }

    public static void RecycleAll<T>(T prefab) where T : Component
    {
        RecycleAll(prefab.gameObject);
    }
    public static void RecycleAll(GameObject prefab)
    {
        foreach (var item in Instance.spawnedObjects)
            if (item.Value == prefab)
                tempList.Add(item.Key);
        for (int i = 0; i < tempList.Count; ++i)
            Recycle(tempList[i]);
        tempList.Clear();
    }
    public static void RecycleAll()
    {
        tempList.AddRange(Instance.spawnedObjects.Keys);
        for (int i = 0; i < tempList.Count; ++i)
            Recycle(tempList[i]);
        tempList.Clear();
    }

    public static bool IsSpawned(GameObject obj)
    {
        return Instance.spawnedObjects.ContainsKey(obj);
    }

    public static int CountPooled<T>(T prefab) where T : Component
    {
        return CountPooled(prefab.gameObject);
    }
    public static int CountPooled(GameObject prefab)
    {
        List<GameObject> list;
        if (Instance.pooledObjects.TryGetValue(prefab, out list))
            return list.Count;
        return 0;
    }

    public static int CountSpawned<T>(T prefab) where T : Component
    {
        return CountSpawned(prefab.gameObject);
    }
    public static int CountSpawned(GameObject prefab)
    {
        int count = 0;
        foreach (var instancePrefab in Instance.spawnedObjects.Values)
            if (prefab == instancePrefab)
                ++count;
        return count;
    }

    public static int CountAllPooled()
    {
        int count = 0;
        foreach (var list in Instance.pooledObjects.Values)
            count += list.Count;
        return count;
    }

    public static List<GameObject> GetPooled(GameObject prefab, List<GameObject> list, bool appendList)
    {
        if (list == null)
            list = new List<GameObject>();
        if (!appendList)
            list.Clear();
        List<GameObject> pooled;
        if (Instance.pooledObjects.TryGetValue(prefab, out pooled))
            list.AddRange(pooled);
        return list;
    }
    public static List<T> GetPooled<T>(T prefab, List<T> list, bool appendList) where T : Component
    {
        if (list == null)
            list = new List<T>();
        if (!appendList)
            list.Clear();
        List<GameObject> pooled;
        if (Instance.pooledObjects.TryGetValue(prefab.gameObject, out pooled))
            for (int i = 0; i < pooled.Count; ++i)
                list.Add(pooled[i].GetComponent<T>());
        return list;
    }

    public static List<GameObject> GetSpawned(GameObject prefab, List<GameObject> list, bool appendList)
    {
        if (list == null)
            list = new List<GameObject>();
        if (!appendList)
            list.Clear();
        foreach (var item in Instance.spawnedObjects)
            if (item.Value == prefab)
                list.Add(item.Key);
        return list;
    }
    public static List<T> GetSpawned<T>(T prefab, List<T> list, bool appendList) where T : Component
    {
        if (list == null)
            list = new List<T>();
        if (!appendList)
            list.Clear();
        var prefabObj = prefab.gameObject;
        foreach (var item in Instance.spawnedObjects)
            if (item.Value == prefabObj)
                list.Add(item.Key.GetComponent<T>());
        return list;
    }

    public static void DestroyPooled(GameObject prefab)
    {
        List<GameObject> pooled;
        if (Instance.pooledObjects.TryGetValue(prefab, out pooled))
        {
            for (int i = 0; i < pooled.Count; ++i)
                GameObject.Destroy(pooled[i]);
            pooled.Clear();
        }
    }
    public static void DestroyPooled<T>(T prefab) where T : Component
    {
        DestroyPooled(prefab.gameObject);
    }

    public static void DestroyAll(GameObject prefab)
    {
        RecycleAll(prefab);
        DestroyPooled(prefab);
    }
    public static void DestroyAll<T>(T prefab) where T : Component
    {
        DestroyAll(prefab.gameObject);
    }


}

public static class GameObjectPoolExtensions
{
    public static void CreatePool<T>(this T prefab) where T : Component
    {
        GameObjectPool.CreatePool(prefab, 0);
    }
    public static void CreatePool<T>(this T prefab, int initialPoolSize) where T : Component
    {
        GameObjectPool.CreatePool(prefab, initialPoolSize);
    }
    public static void CreatePool(this GameObject prefab)
    {
        GameObjectPool.CreatePool(prefab, 0);
    }
    public static void CreatePool(this GameObject prefab, int initialPoolSize)
    {
        GameObjectPool.CreatePool(prefab, initialPoolSize);
    }

    public static T Spawn<T>(this T prefab, Transform parent, Vector3 position, Quaternion rotation) where T : Component
    {
        return GameObjectPool.Spawn(prefab, parent, position, rotation);
    }
    public static T Spawn<T>(this T prefab, Vector3 position, Quaternion rotation) where T : Component
    {
        return GameObjectPool.Spawn(prefab, null, position, rotation);
    }
    public static T Spawn<T>(this T prefab, Transform parent, Vector3 position) where T : Component
    {
        return GameObjectPool.Spawn(prefab, parent, position, Quaternion.identity);
    }
    public static T Spawn<T>(this T prefab, Vector3 position) where T : Component
    {
        return GameObjectPool.Spawn(prefab, null, position, Quaternion.identity);
    }
    public static T Spawn<T>(this T prefab, Transform parent) where T : Component
    {
        return GameObjectPool.Spawn(prefab, parent, Vector3.zero, Quaternion.identity);
    }
    public static T Spawn<T>(this T prefab) where T : Component
    {
        return GameObjectPool.Spawn(prefab, null, Vector3.zero, Quaternion.identity);
    }
    public static GameObject Spawn(this GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
    {
        return GameObjectPool.Spawn(prefab, parent, position, rotation);
    }
    public static GameObject Spawn(this GameObject prefab, Vector3 position, Quaternion rotation)
    {
        return GameObjectPool.Spawn(prefab, null, position, rotation);
    }
    public static GameObject Spawn(this GameObject prefab, Transform parent, Vector3 position)
    {
        return GameObjectPool.Spawn(prefab, parent, position, Quaternion.identity);
    }
    public static GameObject Spawn(this GameObject prefab, Vector3 position)
    {
        return GameObjectPool.Spawn(prefab, null, position, Quaternion.identity);
    }
    public static GameObject Spawn(this GameObject prefab, Transform parent)
    {
        return GameObjectPool.Spawn(prefab, parent, Vector3.zero, Quaternion.identity);
    }
    public static GameObject Spawn(this GameObject prefab)
    {
        return GameObjectPool.Spawn(prefab, null, Vector3.zero, Quaternion.identity);
    }

    public static void Recycle<T>(this T obj) where T : Component
    {
        GameObjectPool.Recycle(obj);
    }
    public static void Recycle(this GameObject obj)
    {
        GameObjectPool.Recycle(obj);
    }

    public static void RecycleAll<T>(this T prefab) where T : Component
    {
        GameObjectPool.RecycleAll(prefab);
    }
    public static void RecycleAll(this GameObject prefab)
    {
        GameObjectPool.RecycleAll(prefab);
    }

    public static int CountPooled<T>(this T prefab) where T : Component
    {
        return GameObjectPool.CountPooled(prefab);
    }
    public static int CountPooled(this GameObject prefab)
    {
        return GameObjectPool.CountPooled(prefab);
    }

    public static int CountSpawned<T>(this T prefab) where T : Component
    {
        return GameObjectPool.CountSpawned(prefab);
    }
    public static int CountSpawned(this GameObject prefab)
    {
        return GameObjectPool.CountSpawned(prefab);
    }

    public static List<GameObject> GetSpawned(this GameObject prefab, List<GameObject> list, bool appendList)
    {
        return GameObjectPool.GetSpawned(prefab, list, appendList);
    }
    public static List<GameObject> GetSpawned(this GameObject prefab, List<GameObject> list)
    {
        return GameObjectPool.GetSpawned(prefab, list, false);
    }
    public static List<GameObject> GetSpawned(this GameObject prefab)
    {
        return GameObjectPool.GetSpawned(prefab, null, false);
    }
    public static List<T> GetSpawned<T>(this T prefab, List<T> list, bool appendList) where T : Component
    {
        return GameObjectPool.GetSpawned(prefab, list, appendList);
    }
    public static List<T> GetSpawned<T>(this T prefab, List<T> list) where T : Component
    {
        return GameObjectPool.GetSpawned(prefab, list, false);
    }
    public static List<T> GetSpawned<T>(this T prefab) where T : Component
    {
        return GameObjectPool.GetSpawned(prefab, null, false);
    }

    public static List<GameObject> GetPooled(this GameObject prefab, List<GameObject> list, bool appendList)
    {
        return GameObjectPool.GetPooled(prefab, list, appendList);
    }
    public static List<GameObject> GetPooled(this GameObject prefab, List<GameObject> list)
    {
        return GameObjectPool.GetPooled(prefab, list, false);
    }
    public static List<GameObject> GetPooled(this GameObject prefab)
    {
        return GameObjectPool.GetPooled(prefab, null, false);
    }
    public static List<T> GetPooled<T>(this T prefab, List<T> list, bool appendList) where T : Component
    {
        return GameObjectPool.GetPooled(prefab, list, appendList);
    }
    public static List<T> GetPooled<T>(this T prefab, List<T> list) where T : Component
    {
        return GameObjectPool.GetPooled(prefab, list, false);
    }
    public static List<T> GetPooled<T>(this T prefab) where T : Component
    {
        return GameObjectPool.GetPooled(prefab, null, false);
    }

    public static void DestroyPooled(this GameObject prefab)
    {
        GameObjectPool.DestroyPooled(prefab);
    }
    public static void DestroyPooled<T>(this T prefab) where T : Component
    {
        GameObjectPool.DestroyPooled(prefab.gameObject);
    }

    public static void DestroyAll(this GameObject prefab)
    {
        GameObjectPool.DestroyAll(prefab);
    }
    public static void DestroyAll<T>(this T prefab) where T : Component
    {
        GameObjectPool.DestroyAll(prefab.gameObject);
    }
}