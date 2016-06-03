using UnityEngine;
using System.Collections;
using System;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.IO;
namespace GameAssets
{

    /// <summary>
    /// 资源加载管理类
    /// 设计目的是对外部隐藏Resources加载和Assetbundle加载的区别，提供资源的加载、缓存和释放，提供易于访问的使用接口
    /// </summary>
    public static class LoadResources
    {
        public static ELoadMode m_loadMode;          //加载模式，在游戏启动后不可再更改
        public static AssetBundleManifest AssetBundleManifestObject;
        private static Dictionary<string, ResourceItem> m_resourceItemDic = new Dictionary<string, ResourceItem>();  //资源缓存字典

        public static void Init(Action callBack)
        {
            //注册update,处理多个资源并行加载的情况
            GlobalDelegate.Instance.View.OnUpdate += OnUpdate;

            m_loadMode = ELoadMode.AssetBundle;
            LoadAssetBundleManifest(callBack);
        }

        private static bool IsInit
        {
            get { return AssetBundleManifestObject != null; }
        }

        private static bool IsManifest(string path)
        {
            var platformName = AssetPathConfig.GetPlatformName();
            return !string.IsNullOrEmpty(platformName) && !string.IsNullOrEmpty(path) && platformName.ToLower() == path.ToLower();
        }

        private static void LoadAssetBundleManifest(Action callBack)
        {
            LoadResources.LoadCore(AssetPathConfig.GetPlatformName(), item =>
            {
                AssetBundleManifestObject = item.MainAsset as AssetBundleManifest;
                //LoggerHelper.Error("LoadAssetBundleManifest init: " + AssetBundleManifestObject);
                if (callBack != null)
                {
                    callBack();
                }
            }, EResType.None, false, true, false);
        }

        public static void LoadText(string path, Action<string> callBack)
        {
            LoadCore(path, item =>
            {
                string arr = null;
                if (item.MainAsset != null)
                    arr = item.MainAsset.ToString();
                else if (item.Assets != null && item.Assets.Length > 0)
                    arr = item.Assets[0].ToString();
                if (arr == null)
                {
                    Debug.LogError("LoadText, path: " + path + ", item: " + item + ", arr: " + arr);
                }
                callBack(arr);
            }, EResType.Text, false, true);
        }

        /// <summary>
        /// 按行加载文本文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callBack"></param>
        public static void LoadTextLine(string path, Action<string[]> callBack)
        {
            LoadCore(path, item =>
            {
                string arr = null;
                if (item.MainAsset != null)
                    arr = item.MainAsset.ToString();
                else if (item.Assets != null && item.Assets.Length > 0)
                    arr = item.Assets[0].ToString();
                if (arr == null)
                {
                    Debug.LogError("LoadTextLine, path: " + path + ", item: " + item + ", arr: " + arr);
                }
                string[] arrs = null;
                if (item.MainAsset != null)
                {
                    if (Application.platform == RuntimePlatform.IPhonePlayer)
                    {
                        arrs = arr.ToString().Split(new[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    }
                    else
                    {
                        arrs = arr.ToString().Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    }
                }
                callBack(arrs);
            }, EResType.Text, false, true);
        }

        /// <summary>
        /// 底层加载函数，异步加载unity压缩过的AssetBundle，或者同步加载Resources类型
        /// </summary>
        /// <param name="newPath">相对于Resources路径下的资源路径</param>
        /// <param name="callBack">回调函数的参数为ResourceItem对象，可以详细拿到AB中的所有Asset</param>
        /// <param name="type">分类标签</param>
        /// <param name="saveAssetBundle">是否保存AssetBundle对象不卸载，可以保留依赖关系</param>
        /// <param name="clearAfterLoaded">加载完成后清除自己的缓存</param>
        public static void LoadCore(string path, Action<ResourceItem> callBack, EResType type = EResType.None, bool saveAssetBundle = false, bool clearAfterLoaded = false, bool dependLoad = true, bool needCRC = false, int tag = 0, bool isImage = false)
        {
            //LoggerHelper.Error("LoadCore: " + path + saveAssetBundle + clearAfterLoaded);
            var isMainifest = IsManifest(path);
            if (!isMainifest && !IsInit)
            {
                Debug.LogError("has not init mainifest, can not load item: " + path);
                return;
            }
            string newPath = path.ToLower();
            string savePath = "";
            # region isImage
            if (isImage)
            {
                string assetName = getImagAssetName(newPath);
                if (string.IsNullOrEmpty(assetName))
                {
                    Debug.LogError("LoadImageAssets, url error: " + newPath);
                    return;
                }

                savePath = getImagSavePath(newPath);

                if (!m_resourceItemDic.ContainsKey(assetName))
                {
                    //先从本地加载图片
                    //Debug.LogError("savePath: " + savePath);
                    if (string.IsNullOrEmpty(savePath))
                    {
                        Debug.LogError("LoadImageAssets, url error 2: " + newPath);
                        return;
                    }

#if UNITY_WEBPLAYER
                ResourceItem itemNew = new ResourceItem(path, saveAssetBundle);
#else
                    FileInfo fi = new FileInfo(savePath);
                    //创建缓存保存目录
                    if (!fi.Directory.Exists)
                        fi.Directory.Create();
                    ResourceItem itemNew;
                    if (!fi.Exists)
                    {
                        //如果本地文件不存在，说明是个网络地址
                        itemNew = new ResourceItem(newPath, saveAssetBundle);
                    }
                    else
                    {
                        itemNew = new ResourceItem("file:///" + savePath, saveAssetBundle);
                    }
#endif
                    itemNew.NeedCRC = needCRC;
                    itemNew.AddTags(tag);
                    m_resourceItemDic.Add(assetName, itemNew);
                }

                newPath = assetName;
            }
            # endregion

            bool hasContains = false;

            if (!m_resourceItemDic.ContainsKey(newPath))
            {
                var itemNew = new ResourceItem(path, saveAssetBundle);
                itemNew.NeedCRC = needCRC;
                itemNew.AddTags(tag);
                m_resourceItemDic.Add(newPath, itemNew);
            }
            else
            {
                hasContains = !isImage;
                if (hasContains)
                {
                    Debug.Log("hascontains: " + newPath);
                }
            }

            var item = m_resourceItemDic[newPath];
            item.SetType(type);
            item.IsClearAfterLoaded = clearAfterLoaded;
            if (!hasContains && !item.IsLoaded)
            {
                if (dependLoad && IsInit)
                {
                    //Debug.LogError("bb: " + item.Path);
                    var dependList = AssetBundleManifestObject.GetAllDependencies(item.Path.ToLower());
                    List<string> distinctList = new List<string>();
                    for (int i = 0; i < dependList.Length; i++)
                    {
                        if (m_resourceItemDic.ContainsKey(dependList[i]))
                        {
                            Debug.Log("contains depend, path: " + item.Path + "------- file: " + dependList[i]);
                        }
                        else
                        {
                            Debug.Log("not contains depend, path: " + item.Path + "------- file: " + dependList[i]);
                            distinctList.Add(dependList[i]);
                        }
                    }
                    //LoadMulti(distinctList.ToArray(), null, () => GlobalDelegate.Instance.View.StartCoroutine(BeginAssetBundleLoadAsync(item)));
                    LoadMulti(distinctList.ToArray(), null, () => beginLoadItem(item));
                }
                else if (isImage)
                {
                    GlobalDelegate.Instance.View.StartCoroutine(BeginImageLoadAsync(item, savePath));
                }
                else
                {
                    //GlobalDelegate.Instance.View.StartCoroutine(BeginAssetBundleLoadAsync(item));
                    beginLoadItem(item);
                }
            }

            item.AddCallBackFunc(callBack);
            if (item.IsCompleted)
            {
                item.LaunchCallBack();
            }
        }

        private static void beginLoadItem(ResourceItem item)
        {
            GlobalDelegate.Instance.View.StartCoroutine(BeginAssetBundleLoadAsync(item));
        }

        public static void LoadCore(ResourceItem item, Action<ResourceItem> callBack, bool dependLoad = true)
        {
            LoadCore(item.Path, callBack, item.Type, item.IsSaveAssetBundle, item.IsClearAfterLoaded, dependLoad, item.NeedCRC, item.Tag);
        }

        public static ResourceItem LoadCore(string path, EResType type = EResType.None)
        {
            if (m_resourceItemDic.ContainsKey(path))
            {
                var item = m_resourceItemDic[path];
                item.SetType(type);
                return item;
            }
            return null;
        }

        /// <summary>
        /// 批量加载多个资源，可以作为预加载等用途。最好一次加载同一类资源，否则设置type为EResType.None
        /// </summary>
        /// <param name="paths">资源地址数组</param>
        /// <param name="eachCallBack">每个资源加载完毕后的回调，可为null</param>
        /// <param name="completeCallBack">全部资源加载完毕的回调，可为null</param>
        public static void LoadMulti(string[] paths, Action<ResourceItem> eachCallBack, Action completeCallBack)
        {
            LoadMultiCore(paths, 0, eachCallBack, completeCallBack);
        }

        public static void LoadMulti(ResourceItem[] items, Action<ResourceItem> eachCallBack, Action completeCallBack)
        {
            LoadMultiCore(items, 0, eachCallBack, completeCallBack);
        }

        private static void LoadMultiCore(string[] paths, int index, Action<ResourceItem> eachCallBack, Action completeCallBack)
        {
            List<ResourceItem> items = new List<ResourceItem>();
            for (int i = 0; i < paths.Length; i++)
            {
                items.Add(new ResourceItem(paths[i], true));
            }
            LoadMultiCore(items.ToArray(), index, eachCallBack, completeCallBack);
        }

        private static void LoadMultiCore(ResourceItem[] items, int index, Action<ResourceItem> eachCallBack, Action completeCallBack)
        {
            var multiLoadItem = new MultiLoadItem();
            for (int i = 0; i < items.Length; i++)
            {
                var dependList = AssetBundleManifestObject.GetAllDependencies(items[i].Path.ToLower());
                if (dependList != null && dependList.Length > 0)
                {
                    for (int j = 0; j < dependList.Length; j++)
                    {
                        if (m_resourceItemDic.ContainsKey(dependList[j]))
                        {
                            Debug.LogError("-------has got file: " + dependList[j]);
                        }
                        else
                        {
                            var item = new ResourceItem(dependList[j], true);
                            multiLoadItem.items.Enqueue(item);
                        }
                    }
                }
                multiLoadItem.items.Enqueue(items[i]);
            }
            multiLoadItem.eachCallBack = eachCallBack;
            multiLoadItem.completeCallBack = completeCallBack;

            m_multiLoad.Enqueue(multiLoadItem);

            OnUpdate();
        }

        #region 全局的分帧实例化处理，在Update里实例化
        class InstanceItem
        {
            public Object go;
            public Action<GameObject> callBack;
        }

        private static readonly Queue<InstanceItem> m_instanceQueue = new Queue<InstanceItem>();
        public static void InstanceAsync(Object go, Action<GameObject> callBack)
        {
            m_instanceQueue.Enqueue(new InstanceItem { go = go, callBack = callBack });
        }
        #endregion

        private static bool m_isMultiLoading;
        private static Queue<MultiLoadItem> m_multiLoad = new Queue<MultiLoadItem>();
        public static void OnUpdate()
        {
            if (m_instanceQueue.Count > 0)
            {
                var item = m_instanceQueue.Dequeue();
                if (item.callBack != null)
                {
                    item.callBack(Object.Instantiate(item.go) as GameObject);
                }
            }

            if (m_multiLoad.Count == 0 || m_isMultiLoading)
                return;
            var multiLoadItem = m_multiLoad.Peek();
            if (multiLoadItem.items.Count == 0)
            {
                var removeItem = m_multiLoad.Dequeue();
                if (removeItem.completeCallBack != null)
                {
                    removeItem.completeCallBack();
                }
                return;
            }

            var loadItem = multiLoadItem.items.Dequeue();
            m_isMultiLoading = true;
            LoadCore(loadItem, item =>
            {
                if (multiLoadItem.eachCallBack != null)
                {
                    multiLoadItem.eachCallBack(item);
                }

                m_isMultiLoading = false;
                OnUpdate();
            }, false);
        }

        public class MultiLoadItem
        {
            public Queue<ResourceItem> items = new Queue<ResourceItem>();
            public Action<ResourceItem> eachCallBack;
            public Action completeCallBack;
        }

        private static void GetAllDependResources(string path, Queue<string> dependList)
        {
            if (!IsInit) return;
            var paths = AssetBundleManifestObject.GetAllDependencies(path.ToLower());
            for (int i = 0; i < paths.Length; i++)
            {
                if (!dependList.Contains(paths[i]))
                {
                    dependList.Enqueue(paths[i]);
                }
            }
        }

        /// <summary>
        /// WWW加载图片
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        private static IEnumerator BeginImageLoadAsync(ResourceItem item, string savePath)
        {
            item.SetLoadState(EResItemLoadState.Loading);
            string path = item.Path;
            var www = new WWW(path);
            yield return www;

            //Debug.LogError("LoadImage save good: " + savePath);

            if (string.IsNullOrEmpty(www.error))
            {
                var texture2d = www.texture;
                texture2d.Compress(true);
                item.SetLoadState(EResItemLoadState.Completed);
                item.SetMainAsset(texture2d);
                item.LaunchCallBack();

                try
                {
                    File.WriteAllBytes(savePath, www.bytes);
                }
                catch (Exception e)
                {
                    Debug.LogError("LoadImage save error: " + e.ToString());
                }

                www.Dispose();
            }
            else
            {
                //Debug.LogError("pp: " + path);
                item.SetLoadState(EResItemLoadState.Error);
                Debug.LogError(string.Format("加载资源失败：{0}\n{1}", www.url, www.error));
                item.LaunchCallBack();
            }

            //按需清除ResourceItem缓存
            if (item.IsClearAfterLoaded)
            {
                Clear(item.Path);
                GarbageCollect();
            }
        }

        //开始异步加载Resources类型的资源，用来模拟AssetBundle异步加载流程
        private static IEnumerator BeginResourceLoadAsync(ResourceItem item)
        {
            var request = Resources.LoadAsync(item.Path);
            yield return request;

            var asset = request.asset;
            if (asset == null)
            {
                var assets = Resources.LoadAll(item.Path);
                if (assets == null || assets.Length == 0)
                {
                    Debug.LogError("资源不存在：\n路径：" + item.Path);
                    item.LaunchCallBack();
                    yield break;
                }
                else
                    item.SetAssets(assets);
            }
            else
                item.SetMainAsset(asset);
            //Debug.LogError("load: "+asset);
            //if (item.NeedCRC)
            //{
            //    item.CRC = FirstUtil.CRC32String(File.ReadAllBytes(string.Concat( Application.dataPath, "/Resources/", item.Path, ".prefab")));
            //}

            item.SetLoadState(EResItemLoadState.Completed);

            //按需清除ResourceItem缓存
            if (item.IsClearAfterLoaded && m_resourceItemDic.ContainsKey(item.Path))
                m_resourceItemDic.Remove(item.Path);

            item.LaunchCallBack();
        }

        //开始异步加载AssetBundle类型的资源
        //远程下载文件到内存中成为AssetBundle镜像，再开辟内存从AssetBundle镜像中创建出指定Asset，最后卸载掉AB镜像内存，只保留Asset对象
        private static IEnumerator BeginAssetBundleLoadAsync(ResourceItem item)
        {
            var isMainifest = IsManifest(item.Path);
            var path = AssetPathConfig.GetAssetPathWWW(!isMainifest ? item.Path.ToLower() : item.Path);
            item.SetLoadState(EResItemLoadState.Loading);
            //if (item.Path.Contains("atlas")) LoggerHelper.Error("begin atlas path: " + item.Path);
            var www = new WWW(path);
            yield return www;

            if (string.IsNullOrEmpty(www.error))
            {
                item.SetLoadState(EResItemLoadState.Completed);
                //Debug.LogError("load success, path: " + www.url);

                if (!item.IsSaveAssetBundle || !IsSaveBundle(item.Path))
                {
                    Object mainAsset = null;
                    var bundle = www.assetBundle;
                    if (isMainifest)
                    {
                        //LoggerHelper.Error("is mainifest: " + item.Path);
                        mainAsset = bundle.LoadAsset("AssetBundleManifest");
                        item.SetMainAsset(mainAsset);
                    }
                    else
                    {
                        mainAsset = bundle.mainAsset;
                        if (mainAsset == null)
                        {
                            var assets = bundle.LoadAllAssets();
                            if (assets == null || assets.Length == 0)
                            {
                                Debug.LogError("load no assets, path: " + item.Path);
                                yield break;
                            }
                            mainAsset = assets[0];

                            item.SetMainAsset(mainAsset);
                            item.SetAssets(assets);
                        }
                    }

                    item.LaunchCallBack();

                    if (bundle != null)
                        bundle.Unload(false);

                    www.Dispose();
                }
                else
                {
                    item.SetAssetBundle(www.assetBundle);
                    //if (item.Path.StartsWith("atlas")) LoggerHelper.Error("after atlas path: " + item.Path);
                    item.LaunchCallBack();
                    www.Dispose();
                }
            }
            else
            {
                item.SetLoadState(EResItemLoadState.Error);
                Debug.LogError(string.Format("加载资源失败：{0}\n{1}", www.url, www.error));
                item.LaunchCallBack();
            }

            //按需清除ResourceItem缓存
            if (item.IsClearAfterLoaded)
            {
                Clear(item.Path);
                GarbageCollect();
            }
        }

        private static bool IsSaveBundle(string path)
        {
            return !path.StartsWith("ui.") && !path.StartsWith("fx.");
        }

        /// <summary>
        /// 清除指定路径的资源缓存
        /// </summary>
        /// <param name="path"></param>
        public static void Clear(string path)
        {
            if (m_resourceItemDic.ContainsKey(path))
            {
                m_resourceItemDic[path].Clear();
                m_resourceItemDic.Remove(path);
            }
        }

        /// <summary>
        /// 清除指定标记的资源缓存
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="opposite"></param>
        /// <param name="unloadAllLoadedObjects"></param>
        public static void Clear(int tags, bool opposite = false, bool unloadAllLoadedObjects = false)
        {
            var delList = new List<string>();
            foreach (var resourceItem in m_resourceItemDic)
            {
                if (!opposite && resourceItem.Value.HasTags(tags) || opposite && !resourceItem.Value.HasTags(tags))
                {
                    delList.Add(resourceItem.Value.Path);
                    resourceItem.Value.Clear(unloadAllLoadedObjects);
                }
            }
            for (int i = 0; i < delList.Count; i++)
            {
                m_resourceItemDic.Remove(delList[i]);
                i--;
            }
            delList.Clear();
        }

        /// <summary>
        /// 清除指定类型的资源缓存
        /// </summary>
        /// <param name="resType"></param>
        /// <param name="clearDepend"></param>
        public static void Clear(EResType resType, bool clearDepend = false)
        {
            var delList = new List<string>();
            var list = new Queue<string>();
            foreach (var resourceItem in m_resourceItemDic)
            {
                if (resourceItem.Value.Type == resType)
                {
                    var path = resourceItem.Value.Path;
                    //Debug.LogError("before: " + path);
                    if (resType == EResType.Texture)
                    {
                        var assetName = getImagAssetName(resourceItem.Value.Path);
                        path = getImagePath(assetName);
                    }
                    //Debug.LogError("after: " + path);
                    delList.Add(path);
                    if (clearDepend)
                    {
                        list.Clear();
                        GetAllDependResources(resourceItem.Value.Path, list);
                        delList.AddRange(list);
                    }
                }
            }
            for (int i = 0; i < delList.Count; i++)
            {
                if (m_resourceItemDic.ContainsKey(delList[i]))
                {
                    m_resourceItemDic[delList[i]].Clear();
                    m_resourceItemDic.Remove(delList[i]);
                    i--;
                }
            }
            delList.Clear();
        }

        /// <summary>
        /// 清除所有资源缓存
        /// </summary>
        public static void ClearAll()
        {
            foreach (var resourceItem in m_resourceItemDic)
                resourceItem.Value.Clear();
            m_resourceItemDic.Clear();

            GarbageCollect();
        }

        /// <summary>
        /// 释放内存
        /// </summary>
        public static void GarbageCollect()
        {
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }

        public static ResourceItem GetResourceItem(string path)
        {
            ResourceItem item = null;
            if (m_resourceItemDic.ContainsKey(path))
                item = m_resourceItemDic[path];
            return item;
        }

        /// <summary>
        /// 将网络地址转换一下 原始的格式类似于"http://pic11.nipic.com/20101219/4507639_124353059151_2.jpg"
        /// </summary>
        /// <param name="imgUrl"></param>
        /// <returns></returns>
        public static string getImagAssetName(string imgUrl)
        {
            if (string.IsNullOrEmpty(imgUrl))
            {
                return string.Empty;
            }

            string assetName = "";
            int httpIdx = imgUrl.IndexOf("http://");
            int httpsIdx = imgUrl.IndexOf("https://");
            if (httpIdx >= 0)
            {
                //http前缀
                //先去掉7个字符"http://"
                assetName = imgUrl.Substring(7);
            }
            else if (httpsIdx >= 0)
            {
                //https前缀
                //先去掉8个字符"https://"
                assetName = imgUrl.Substring(8);
            }
            else
            {
                return imgUrl;
            }

            //将特殊字符替换掉
            assetName = assetName.Replace('/', '-');
            assetName = assetName.Replace('\\', '-');
            assetName = assetName.Replace(':', '-');
            assetName = assetName.Replace('?', '-');
            assetName = assetName.Replace('"', '-');
            assetName = assetName.Replace('*', '-');
            assetName = assetName.Replace('<', '-');
            assetName = assetName.Replace('>', '-');
            assetName = assetName.Replace('|', '-');
            return assetName;
        }
        public static string getImagSavePath(string imgPath)
        {
            string assetName = getImagAssetName(imgPath);
            if (string.IsNullOrEmpty(assetName))
            {
                return string.Empty;
            }

            if (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return Application.persistentDataPath + "/ImageSave/" + assetName;
            }
            else
            {
                return Application.dataPath + "/../ImageSave/" + assetName;
            }
        }

        public static string getImagePath(string savePath)
        {
            if (string.IsNullOrEmpty(savePath))
            {
                return null;
            }
            string prePath = "/ImageSave/";
            if (savePath.Contains(prePath))
            {
                return savePath.Substring(savePath.IndexOf(prePath) + prePath.Length);
            }

            return savePath;
        }
    }


    public enum ELoadMode
    {
        Resources,
        AssetBundle
    }

    
    public enum EResItemLoadState
    {
        Wait,       //等待加载
        Loading,    //加载中
        Completed,  //加载完毕
        Error       //加载失败
    }

    public enum EResType
    {
        None,
        Model,
        Effect,
        UIEffect,
        AnimatorController,
        Animation,
        Audio,
        Atlas,
        UI,
        RenderTexture,
        Texture,
        Text,
        Scene,
        BattlePrefab,
        BattleWeapon,
        Config,
        Font,
        Material,
        Shader,
    }

    public static class ResTag
    {
        public const int Forever = 0x00000001;
        public const int Battle = 0x00000002;
        public const int AssetBundle = 0x00000004;
    }
}