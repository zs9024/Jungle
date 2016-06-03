using UnityEngine;
using System.Collections;

/// <summary>
/// 资源加载类
/// author zs
/// </summary>
public class ResourcesLoad
{

    private string assetPath;               //资源路径
    private string assetName;               //资源名称
    //private bool isLocal;                 //是否本地资源
    private bool isMainAsset = true;        //是否主资源
    //private bool isEncrypted;             //是否已加密
    private bool isLoadSyn = true;          //是否同步加载

    private Object objectLoad;              //加载出来的object对象
    public Object ObjectLoad { get { return objectLoad; } }

    private GameObject gObj;                //实例化的游戏体
    public GameObject GO { get { return gObj; } }

    private AssetBundle assetBundle;
    public AssetBundle Bundle { get { return assetBundle; } }

    private int refCount = 0;               //实例化对象的引用计数
    public int RefCount { 
        get { return refCount; }
        set { refCount = value; }
    }

    private bool reused = false;            //是否需要回收
    public bool Reused {
        get { return reused; }
        set { reused = value; }
    }

    public ResourcesLoad(string assetPath)
    {
        this.assetPath = assetPath;
    }

    public ResourcesLoad(string assetPath, bool isLoadSyn)
    {
        this.assetPath = assetPath;
        this.isLoadSyn = isLoadSyn;
    }

    public ResourcesLoad(string assetPath, string assetName, bool isMainAsset, bool isLoadSyn)
    {
        this.assetPath = assetPath;
        this.assetName = assetName;
        this.isMainAsset = isMainAsset;
        this.isLoadSyn = isLoadSyn;
    }

    /// <summary>
    /// www加载AB资源
    /// </summary>
    /// <returns></returns>
    public IEnumerator WWWLoad()
    {
        Debug.Log("WWWLoad 11");

        WWW www = new WWW(assetPath);
        yield return www;

        Debug.Log("WWWLoad 22");

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogWarning("WWWLoad is error:" + www.error + "!AssetPath is " + assetPath);
            yield break;
        }

        assetBundle = www.assetBundle;
        if (assetBundle != null)
        {
            if (isMainAsset)
            {
                objectLoad = assetBundle.mainAsset;
            }
            else
            {
                if (assetBundle.Contains(assetName))
                {
                    if (isLoadSyn)
                        objectLoad = null;//assetBundle.Load(assetName, typeof(Object));
                    else
                    {
                        Debug.Log("WWWLoad 33");
                        AssetBundleRequest abReq = null; //assetBundle.LoadAsync(assetName, typeof(Object));
                        yield return abReq;

                        Debug.Log("WWWLoad 44");
                        if (abReq.isDone)
                            objectLoad = abReq.asset;
                    }
                }
            }
        }

        assetBundle.Unload(false);
        www.Dispose();
    }

    /// <summary>
    /// 本地prefab资源加载
    /// </summary>
    /// <returns></returns>
    public IEnumerator LocalLoad()
    {
        if(isLoadSyn)
        {
            objectLoad = Resources.Load(assetPath);
            yield return objectLoad;
        }
        else
        {
            ResourceRequest resReq = Resources.LoadAsync(assetPath);
            yield return resReq;

            objectLoad = resReq.asset;
        }
        
    }

    /// <summary>
    /// 实例化对象
    /// </summary>
    /// <returns></returns>
    public GameObject InstantiateObject()
    {
        if (objectLoad == null)
            return null;

        gObj = GameObject.Instantiate(objectLoad) as GameObject;
        refCount++;

        return gObj;
    }

    /// <summary>
    /// 加载共享的资源
    /// </summary>
    /// <returns></returns>
    public IEnumerator LoadSharedAssets()
    {
        WWW www = new WWW(assetPath);
        yield return www;

        assetBundle = www.assetBundle;
        //共享资源不能提前unload

        www.Dispose();
    }


    public void Release()
    {
        if(assetBundle != null)
        {
            assetBundle.Unload(false);  //释放AB镜像     
            assetBundle = null;
        }

        //Resources.UnloadAsset(objectLoad); //注意:UnloadAsset只能释放individual assets，如：mesh / texture / material / shader
        Resources.UnloadUnusedAssets();     //释放AB load出来的asset
    }

}
