﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// 资源管理类，实现资源的加载和创建
/// </summary>
public class ResourcesManager : Singleton<ResourcesManager>, IInit
{
    private MonoBehaviour view = null;

    private Dictionary<string, ResourcesLoad> loaderDic;

    public void Init()
    {
        loaderDic = new Dictionary<string, ResourcesLoad>();

        if (view == null)
        {
            GameObject go = GameObject.Find("GlobalDelegate");
            if (go != null)
            {
                view = go.GetComponent<AbstractView>();
                if (view == null)
                {
                    view = go.AddComponent<AbstractView>();
                }
            }
            else
            {
                go = new GameObject("GlobalDelegate");
                view = go.AddComponent<AbstractView>();
            }
        }

        GameObject.DontDestroyOnLoad(view);
    }


    /// <summary>
    /// 资源本地加载
    /// </summary>
    /// <param name="assetPath"></param>
    /// <param name="isMainAsset"></param>
    /// <param name="onGenerated"></param>
    public void LoadAssetsLocal(string assetPath, Action<GameObject> onGenerated = null)
    {
        LoadAssets(true, assetPath, null, false, true, onGenerated);
    }

    /// <summary>
    /// 资源AB加载
    /// </summary>
    /// <param name="assetPath"></param>
    /// <param name="isMainAsset"></param>
    /// <param name="onGenerated"></param>
    public void LoadAssetsBundle(string assetPath, bool isMainAsset = true, Action<GameObject> onGenerated = null)
    {
        LoadAssets(false, assetPath, null, isMainAsset, true, onGenerated);
    }

    /// <summary>
    /// 游戏体生成,异步
    /// </summary>
    /// <param name="assetPath"></param>
    /// <param name="isMainAsset"></param>
    /// <param name="onGenerated"></param>
    public void LoadAssetsBundleAsyn(string assetPath, bool isMainAsset = true, Action<GameObject> onGenerated = null)
    {
        LoadAssets(false, assetPath, null, isMainAsset, false, onGenerated);
    }

    /// <summary>
    /// 加载资源
    /// </summary>
    /// <param name="isLocal">是否本地resources目录加载，否则为AB加载</param>
    /// <param name="assetPath">资源路径</param>
    /// <param name="assetName">资源名称，打包时设置的名称</param>
    /// <param name="isMainAsset">是否主资源（打包时设置）</param>
    /// <param name="isLoadSyn">是否同步加载，false为异步加载</param>
    /// <param name="onGenerated">加载完的回调，GameObject：资源对象</param>
    public void LoadAssets(bool isLocal, string assetPath, string assetName, bool isMainAsset, bool isLoadSyn, Action<GameObject> onLoaded = null)
    {
        Debug.Log("assetPath::" + assetPath);

        if (string.IsNullOrEmpty(assetName))
        {
            if(isLocal)
                assetName = getPrefabNameFromPath(assetPath);
            else
                assetName = getAssetNameFromPath(assetPath);
        }
            
        if (loaderDic != null)
        {
            if(loaderDic.ContainsKey(assetName))
            {
                ResourcesLoad loader = loaderDic[assetName];

                GameObject go = (GameObject)loader.ObjectLoad;
                if (go != null)
                    go.name = assetName;

                if (onLoaded != null)
                    onLoaded(go);
            }   
            else
            {
                ResourcesLoad loader = new ResourcesLoad(assetPath, assetName, isMainAsset, isLoadSyn);
                loaderDic.Add(assetName, loader);

                view.StartCoroutine(LoadAssetsCor(loader, isLocal, assetPath, assetName, isMainAsset, isLoadSyn, onLoaded));
            }
        }

        
    }


    /// <summary>
    /// 资源加载协程
    /// </summary>
    /// <param name="assetPath">资源路径</param>
    /// <param name="assetName">资源名称，打包时设置的名称</param>
    /// <param name="isMainAsset">是否主资源（打包时设置）</param>
    /// <param name="isLoadSyn">是否同步加载，false为异步加载</param>
    /// <param name="onGenerated">生成完的回调</param>
    /// <returns></returns>
    public IEnumerator LoadAssetsCor(ResourcesLoad load, bool isLocal, string assetPath, string assetName, bool isMainAsset, bool isLoadSyn, Action<GameObject> onLoaded)
    {
        if (isLocal)
            yield return view.StartCoroutine(load.LocalLoad());
        else
            yield return view.StartCoroutine(load.WWWLoad());

        GameObject go = (GameObject)load.ObjectLoad;
        if(go != null)
            go.name = assetName;

        if (onLoaded != null)
            onLoaded(go);
    }

    /// <summary>
    /// 对象实例化
    /// </summary>
    /// <param name="objectLoaded">资源对象</param>
    /// <returns>实例化的游戏体</returns>
    public GameObject InstantiateObject(GameObject objectLoaded)
    {
        if (objectLoaded == null)
            return null;

        string assetName = objectLoaded.name;
        if(loaderDic.ContainsKey(assetName))
        {
            var loader = loaderDic[assetName];
            GameObject ins = loader.InstantiateObject();
            ins.name = assetName;

            return ins;
        }

        return null;
    }

    /// <summary>
    /// 加载共享资源
    /// </summary>
    /// <param name="assetPath">共享资源路径</param>
    /// <param name="onGenerated">加载完成回调</param>
    public void LoadSharedAssets(string assetPath, Action onGenerated = null)
    {
        string assetName = getAssetNameFromPath(assetPath);     //打包以后的共享文件的名字

        if (loaderDic != null)
        {
            if (loaderDic.ContainsKey(assetName))
            {
                ResourcesLoad loader = loaderDic[assetName];
                if(loader.Bundle != null)
                {
                    if (onGenerated != null)
                        onGenerated();
                }               
            }
            else
            {
                ResourcesLoad loader = new ResourcesLoad(assetPath);
                loaderDic.Add(assetName, loader);

                view.StartCoroutine(LoadSharedAssetsCor(loader,assetPath, onGenerated));
            }
        }
        
    }

    /// <summary>
    /// 加载共享资源协程
    /// </summary>
    /// <param name="assetPath">共享资源路径</param>
    /// <param name="onGenerated">加载完成回调</param>
    public IEnumerator LoadSharedAssetsCor(ResourcesLoad load,string assetPath, Action onGenerated)
    {
        yield return view.StartCoroutine(load.LoadSharedAssets());

        if (onGenerated != null)
            onGenerated();
    }

    /// <summary>
    /// 获取资源名称，如果是共享资源，得到是打包时设置的文件名
    /// </summary>
    /// <param name="assetPath"></param>
    /// <returns></returns>
    private string getAssetNameFromPath(string assetPath)
    {
        //assetPath::file://F:/SVNPath/DownLoad/ExternalLoad/StarBaby/Assets.Resources.Prefabs.StarBaby.xingzai_A.unity3d

        if (string.IsNullOrEmpty(assetPath))
            return string.Empty;

        string[] args = assetPath.Split('.');
        if (args.Length < 2)
        {
            Debug.LogWarning("The assetpath has error: " + assetPath);
            return string.Empty;
        }

        return args[args.Length - 2];
    }

    private string getPrefabNameFromPath(string assetPath)
    {
        if (string.IsNullOrEmpty(assetPath))
            return string.Empty;

        return assetPath.Substring(assetPath.LastIndexOf('/') + 1);
    }

    /// <summary>
    /// 释放游戏体占用的所有内存
    /// </summary>
    /// <param name="go"></param>
    public void Release(GameObject go)
    {
        string name = string.Empty;
        if (go != null)
        {
            name = go.name;

            GameObject.Destroy(go);       //释放clone的对象
            go = null;
        }

        bool removeable = false;
        foreach(string key in loaderDic.Keys)
        {
            if(key == name) 
            {
                ResourcesLoad loader = loaderDic[key];
                loader.RefCount--;

                if (loader.RefCount == 0)
                {
                    removeable = true;
                    loader.Release();
                    loader = null;
                }

                break;
            }
        }

        if(removeable)
            loaderDic.Remove(name); 
    }

    /// <summary>
    /// 释放游戏体占用的所有内存
    /// </summary>
    /// <param name="assetName"></param>
    public void Release(string assetName)
    {
        foreach (string key in loaderDic.Keys)
        {
            if (key == assetName)
            {
                ResourcesLoad loader = loaderDic[key];
                loader.Release();

                break;
            }
        }

        loaderDic.Remove(assetName); //如果没有这个元素，返回false
    }

    /// <summary>
    /// 释放所有资源
    /// </summary>
//     public void ReleaseAll()
//     {
//         foreach (string key in loaderDic.Keys)
//         {           
//             ResourcesLoad loader = loaderDic[key];
//             loader.Release();
//         }
// 
//         loaderDic.Clear();
// 
//         GC.Collect();
//    
}
