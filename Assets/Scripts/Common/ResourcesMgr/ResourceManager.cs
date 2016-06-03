using GameAssets;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// 资源管理类
/// 是对LoadResources类的一个封装，提供资源的加载、缓存和释放，提供易于访问的使用接口
/// </summary>
public static class ResourceManager
{
    //临时使用，后面会改为使用具体的接口
    public static void GetGameObject(string path, Action<GameObject> callBack, bool clearAfterLoaded = false)
    {
        Action<Object> cb = (obj) =>
        {
            if (obj == null)
            {
                Debug.LogError("get no obj, path: " + path);
                return;
            }
            var clone = Object.Instantiate(obj) as GameObject;
            clone.name = obj.name;
            callBack(clone);
        };
        LoadResources.LoadCore(path, item => cb(item.MainAsset), EResType.None, false, clearAfterLoaded);
    }

    /// <summary>
    /// 加载图片资源(从本地或者网上加载)
    /// </summary>
    /// <param name="assetName"></param>
    /// <param name="isLocal"></param>
    /// <param name="callBack"></param>
    public static void LoadImageAssets(string imageUrl, Action<object> onLoaded)
    {
        //LoggerHelper.Error("ima: " + imageUrl);
        LoadResources.LoadCore(imageUrl, item => onLoaded(item.MainAsset), EResType.Texture, false, false, false, false, 0, true);
    }

    public static GameObject GetObject(string path)
    {
        ResourceItem item = LoadResources.LoadCore(path, EResType.None);
        if (item != null)
        {
            return Object.Instantiate(item.MainAsset) as GameObject;
        }
        return null;
    }

    /// <summary>
    /// 加载多个资源
    /// </summary>
    /// <param name="paths"></param>
    /// <param name="eachCallBack"></param>
    /// <param name="completeCallBack"></param>
    public static void LoadMulti(string[] paths, Action<ResourceItem> eachCallBack, Action completeCallBack)
    {
        LoadResources.LoadMulti(paths, eachCallBack, completeCallBack);
    }

    public static void LoadMulti(ResourceItem[] items, Action<ResourceItem> eachCallBack, Action completeCallBack)
    {
        LoadResources.LoadMulti(items, eachCallBack, completeCallBack);
    }

    /// <summary>
    /// 加载字体资源
    /// </summary>
    /// <param name="name">字体名</param>
    /// <param name="callBack">回调函数的参数为实例化后的GameObject对象</param>
    public static void LoadFont(string name, Action<Font> callBack)
    {
        LoadResources.LoadCore(name, item => callBack(Object.Instantiate(item.MainAsset) as Font), EResType.Font, false, false);
    }

    /// <summary>
    /// 加载字体资源
    /// </summary>
    /// <param name="name">字体名</param>
    public static Font LoadFont(string name = "Normal")
    {
        return LoadResources.LoadCore(name, EResType.Font).MainAsset as Font;
    }

    public static void LoadShader(string shaderName, Action<Shader> callBack, bool clearAfterLoaded = false)
    {
        //string shaderFileName = shaderName.Substring(shaderName.IndexOf("/") + 1);
        LoadResources.LoadCore(shaderName, item => callBack(item.MainAsset as Shader), EResType.Shader, false, clearAfterLoaded);
    }

    public static Shader LoadShader(string shaderName)
    {
        //string shaderFileName = shaderName.Substring(shaderName.IndexOf("/") + 1);
        return LoadResources.LoadCore(shaderName, EResType.Shader).MainAsset as Shader;
    }

    public static void LoadMaterial(string name, Action<Material> callBack)
    {
        LoadAsset(name, res =>
        {
            if (LoadResources.m_loadMode == ELoadMode.Resources)
                callBack(Object.Instantiate(res) as Material);
            else
            {
                Material mat = res as Material;
                callBack(mat);
            }
        });
    }

    public static Material LoadMaterial(string name)
    {
        Material retMat = LoadResources.LoadCore(name, EResType.Material).MainAsset as Material;

        if (LoadResources.m_loadMode == ELoadMode.Resources)
            retMat = Object.Instantiate(retMat) as Material;    // 避免本地文件被修改

        //if (Application.isEditor)
        //{
        //    if (retMat.shader != null)
        //    {
        //        retMat.shader = Shader.Find(retMat.shader.name);
        //        //LoggerHelper.Error("-----------------" + mat.shader.name);
        //    }
        //}
        return retMat;
    }

    public static void LoadTexture(string name, Action<Texture2D> callBack, EResType restype = EResType.Texture)
    {
        LoadResources.LoadCore(name, res => callBack(res.MainAsset as Texture2D), restype);
    }

    public static void LoadText(string assetName, Action<string> callBack)
    {
        if (string.IsNullOrEmpty(assetName) || null == callBack)
        {
            return;
        }
        LoadResources.LoadText(assetName, callBack);
    }   

    /// <summary>
    /// 加载声音资源，由AudioManager自己管理缓存
    /// </summary>
    /// <param name="path">相对于Resources路径下的资源路径</param>
    /// <param name="callBack">回调函数的参数为实例化后的AudioClip对象</param>
    public static void LoadAudio(string path, Action<AudioClip> callBack)
    {
        LoadResources.LoadCore(path, item => callBack(item.MainAsset as AudioClip), EResType.Audio, false, false, false);
    }

    /// <summary>
    /// 加载特效Prefab并实例化
    /// </summary>
    /// <param name="path">相对于Resources路径下的资源路径</param>
    /// <param name="callBack">回调函数的参数为实例化后的GameObject对象</param>
    /// <param name="clearAfterLoaded">加载完成后清除自己的缓存</param>

    public static void LoadEffect(string path, Action<GameObject> callBack, bool clearAfterLoaded = false)
    {
        LoadResources.LoadCore(path, item => LoadResources.InstanceAsync(item.MainAsset, go =>
        {
            //var arr = go.transform.GetComponentsInChildren<Renderer>();
            //for (int i = 0; i < arr.Length; i++)
            //{
            //    if (arr[i].sharedMaterial != null)
            //        arr[i].material.shader = Shader.Find(arr[i].sharedMaterial.shader.name);
            //}
            callBack(go);
        }), EResType.Effect, false, clearAfterLoaded);
    }

    /// <summary>
    /// 加载特效Prefab
    /// </summary>
    /// <param name="path">相对于Resources路径下的资源路径</param>
    /// <param name="callBack">回调函数的参数为GameObject对象</param>
    /// <param name="clearAfterLoaded">加载完成后清除自己的缓存</param>
    public static void LoadUIEffect(string path, Action<GameObject> callBack, bool clearAfterLoaded = false)
    {
        LoadResources.LoadCore(path, item => callBack(item.MainAsset as GameObject), EResType.UIEffect, false, clearAfterLoaded);
    }

    public static void LoadAnimatorController(string path, Action<RuntimeAnimatorController> callBack, bool clearAfterLoaded = false)
    {
        LoadResources.LoadCore(path, item => callBack(item.MainAsset as RuntimeAnimatorController), EResType.AnimatorController, false, clearAfterLoaded);
    }

    /// <summary>
    /// 加载AnimationClip动画资源
    /// </summary>
    /// <param name="path">相对于Resources路径下的资源路径</param>
    /// <param name="callBack">回调函数的参数为实例化后的GameObject对象</param>
    /// <param name="clearAfterLoaded">加载完成后清除自己的缓存</param>
    public static void LoadAnimation(string path, Action<AnimationClip> callBack, bool clearAfterLoaded = false)
    {
        //LoadResources.LoadCore(path, item =>
        //{
        //    if (item.MainAsset is AnimationClip)
        //        callBack(item.MainAsset as AnimationClip);
        //    else
        //    {
        //        var temp = item.MainAsset as GameObject;
        //        if (temp && temp.animation)
        //            callBack(temp.animation.clip);
        //        else
        //            callBack(null);
        //    }
        //}, EResType.Animation, false, clearAfterLoaded);
    }

    /// <summary>
    /// 加载并切换场景
    /// </summary>
    /// <param name="path">相对于Resources路径下的资源路径</param>
    /// <param name="callBack"></param>
    //public static void LoadScene(string path, Action callBack)
    //{
    //    var arr = path.Split('/');
    //    if (m_loadMode == ELoadMode.Resources)
    //    {
    //        Driver.singleton.ExecuteAsync(Application.LoadLevelAsync(arr[arr.Length - 1]), callBack);
    //    }
    //    else if (m_loadMode == ELoadMode.AssetBundle)
    //    {
    //        LoadCore("Scenes/" + path, item =>
    //        {
    //            Driver.singleton.ExecuteAsync(Application.LoadLevelAsync(arr[arr.Length - 1]), callBack);
    //        }, EResType.Scene, true);
    //    }
    //}

    /// <summary>
    /// 加载指定图集上的sprte
    /// </summary>
    /// <param name="atlas">图集名</param>
    /// <param name="sprite">sprite名</param>
    /// <param name="callBack">回调函数</param>
    public static void LoadSprite(string atlas, string sprite, Action<Sprite> callBack)
    {
        LoadResources.LoadCore(atlas, item => callBack(LoadSprite(atlas, sprite)), EResType.Atlas, true);
    }

    /// <summary>
    /// 直接从缓存中读取，如果没有预加载，则返回空对象并报错
    /// </summary>
    /// <param name="atlas"></param>
    /// <param name="sprite"></param>
    /// <returns></returns>
    public static Sprite LoadSprite(string atlas, string sprite)
    {
        var path = atlas.ToLower();
        var item = LoadResources.GetResourceItem(path);
        if (item != null)
        {
            item.SetType(EResType.Atlas);
            //if (LoadResources.m_loadMode == ELoadMode.Resources)
            //{
            //    if (item.Assets == null)
            //        item.SetAssets((item.MainAsset as GameObject).GetComponent<SpritesHolder>().Sprites);
            //    return item.Load(sprite, typeof(Sprite)) as Sprite;
            //}
            //else if (LoadResources.m_loadMode == ELoadMode.AssetBundle)
                return item.Load(sprite, typeof(Sprite)) as Sprite;
        }
        Debug.LogError("【Atlas】缓存中不存在该资源：\n" + "atlas:" + path + " sprite:" + sprite);
        return null;
    }

    /// <summary>
    /// 加载UI的prefab并实例化，未完成依赖加载部分，但可以先用
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callBack"></param>
    /// <param name="clearAfterLoaded"></param>
    public static void LoadUI(string path, Action<GameObject> callBack, bool clearAfterLoaded = true)
    {
        LoadResources.LoadCore(path, item =>
        {
            //LoggerHelper.Error("loadui, path: " + path + ", mainasset: " + item.MainAsset);
            var go = Object.Instantiate(item.MainAsset) as GameObject;
            go.name = item.MainAsset.name;
            callBack(go);
        }, EResType.UI, false, clearAfterLoaded);
    }

    /// <summary>
    /// 直接从缓存中读取并实例化，如果没有预加载，则返回空对象并报错
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static GameObject LoadUI(string path)
    {
        var item = LoadResources.LoadCore(path, EResType.UI);
        if (item != null)
        {
            var go = Object.Instantiate(item.MainAsset) as GameObject;
            go.name = item.MainAsset.name;
            return go;
        }
        Debug.LogError("【UI】缓存中不存在该资源：\n" + "path:" + path);
        return null;
    }

    /// <summary>
    /// 直接从缓存中读取资源引用，不实例化，如果没有预加载，则返回空对象并报错
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static GameObject LoadUIRef(string path)
    {
        var item = LoadResources.LoadCore(path, EResType.UI);
        if (item != null)
            return item.MainAsset as GameObject;
        Debug.LogError("【UI】缓存中不存在该资源：\n" + "path:" + path);
        return null;
    }

    /// <summary>
    /// 直接从缓存中读取资源引用，不实例化，如果没有预加载，则返回空对象并报错
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static RenderTexture LoadRenderTexture(string path)
    {
        var item = LoadResources.LoadCore(path, EResType.RenderTexture);
        if (item != null)
            return item.MainAsset as RenderTexture;
        Debug.LogError("【UI】缓存中不存在该资源：\n" + "path:" + path);
        return null;
    }

    /// <summary>
    /// 加载配置文件
    /// </summary>
    /// <param name="path">配置资源路径</param>
    /// <param name="callBack">加载完毕后的回调函数</param>
    public static void LoadConfigs(string path, Action<Object[]> callBack)
    {
        LoadResources.LoadCore(path, item => callBack(item.Assets), EResType.Config, false, true);
    }
  
    /// <summary>
    /// 在没有对应加载方式的情况下使用的，用来加载任意资源
    /// </summary>
    /// <param name="path">相对于Resources路径下的资源路径</param>
    /// <param name="callBack">回调函数的参数为实例化后的GameObject对象</param>
    /// <param name="clearAfterLoaded">加载完成后清除自己的缓存</param>
    public static void LoadAsset(string path, Action<Object> callBack, bool clearAfterLoaded = false)
    {
        LoadResources.LoadCore(path, item => callBack(item.MainAsset), EResType.None, false, clearAfterLoaded);
    }

    public static void Clear(int tags, bool opposite = false, bool unloadAllLoadedObjects = false)
    {
        LoadResources.Clear(tags, opposite, unloadAllLoadedObjects);
    }

    public static void Clear(EResType resType, bool clearDepend = false)
    {
        LoadResources.Clear(resType, clearDepend);
    }

    public static void GarbageCollect()
    {
        //LoggerHelper.Error("GarbageCollect");
        LoadResources.GarbageCollect();
    }

    public static void UnloadAsset(Object obj)
    {
        Resources.UnloadAsset(obj);
    }
}
