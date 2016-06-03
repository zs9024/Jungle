using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameAssets
{

    public class ResourceItem
    {
        private AssetBundle m_assetBundle;
        private Object m_mainAsset;
        private string m_mainAssetName;
        private Object[] m_assets;
        private string[] m_assetsName;
        private string m_path;
        private EResType m_type;
        private int m_tags;
        private bool m_saveAssetBundle;      //如果为true，则www加载完成后不进行LoadAll操作，也不会卸载AB。一般用在依赖打包的对象上
        private bool m_clearAfterLoaded;  //加载完成后移除对自己这个ResourceItem的引用，交由外部管理（比如配置对象）
        private EResItemLoadState m_loadState = EResItemLoadState.Wait;
        private Queue<Action<ResourceItem>> m_callBackFunc;

        private bool m_bNeedCRC = false;//是否需要计算资源的CRC
        private uint m_CRC;

        public ResourceItem(string path, bool saveAssetBundle = false)
        {
            m_path = path;
            m_saveAssetBundle = saveAssetBundle;

            if (saveAssetBundle)
                AddTags(ResTag.AssetBundle);
        }

        /// <summary>
        /// 用来给预加载的资源方便用
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="tag"></param>
        public ResourceItem(string path, EResType type, int tag = 0)
        {
            m_path = path;
            m_saveAssetBundle = type == EResType.Atlas || type == EResType.Font || type == EResType.Shader;
            SetType(type);
            m_clearAfterLoaded = false;
            m_bNeedCRC = type == EResType.BattlePrefab;
            if (m_saveAssetBundle)
                AddTags(ResTag.AssetBundle);
            if (tag != 0)
                AddTags(tag);
        }

        /// <summary>
        /// 判断标签
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool HasTags(int tags)
        {
            if ((m_tags & tags) == tags)
                return true;
            return false;
        }

        /// <summary>
        /// 增加标签
        /// </summary>
        /// <param name="tags"></param>
        public void AddTags(int tags)
        {
            m_tags |= tags;
        }

        /// <summary>
        /// 设置资源类型
        /// </summary>
        /// <param name="type"></param>
        public void SetType(EResType type)
        {
            //资源一旦被设置过非空类型，则不可再设置回空，除非清除
            if (type != EResType.None)
                m_type = type;
        }

        /// <summary>
        /// 执行回调函数，并清除已保存的回调函数
        /// </summary>
        public void LaunchCallBack()
        {
            //确保回调函数只会执行一次
            while (m_callBackFunc != null && m_callBackFunc.Count > 0)
            {
                var func = m_callBackFunc.Dequeue();
                func(this);
            }

            if (m_callBackFunc != null)
                m_callBackFunc.Clear();
        }

        /// <summary>
        /// 添加回调函数
        /// </summary>
        /// <param name="callBack"></param>
        public void AddCallBackFunc(Action<ResourceItem> callBack)
        {
            if (callBack != null)
            {
                if (m_callBackFunc == null)
                    m_callBackFunc = new Queue<Action<ResourceItem>>();
                m_callBackFunc.Enqueue(callBack);
            }
        }

        /// <summary>
        /// 设置加载状态
        /// </summary>
        /// <param name="state"></param>
        public void SetLoadState(EResItemLoadState state)
        {
            m_loadState = state;
        }

        /// <summary>
        /// 设置主资源对象
        /// </summary>
        /// <param name="mainAsset"></param>
        public void SetMainAsset(Object mainAsset)
        {
            m_mainAsset = mainAsset;
            if (mainAsset != null)
                m_mainAssetName = mainAsset.name;
        }

        /// <summary>
        /// 设置资源数组
        /// </summary>
        /// <param name="assets"></param>
        public void SetAssets(Object[] assets)
        {
            m_assets = assets;
            if (m_assets != null)
            {
                m_assetsName = new string[m_assets.Length];
                for (int i = 0; i < m_assets.Length; i++)
                {
                    if (m_assets[i] != null)
                        m_assetsName[i] = m_assets[i].name;
                    else
                        m_assetsName[i] = null;
                }
            }
        }

        /// <summary>
        /// 设置AssetBundle
        /// </summary>
        /// <param name="ab"></param>
        public void SetAssetBundle(AssetBundle ab)
        {
            m_assetBundle = ab;
        }

        public Object Load(string name, Type type)
        {
            if (m_assetBundle != null)
                return m_assetBundle.LoadAsset(name, type);
            if (m_mainAsset != null && m_mainAssetName == name && m_mainAsset.GetType() == type)
                return m_mainAsset;
            if (m_assets != null)
            {
                for (int i = 0; i < m_assets.Length; i++)
                {
                    if (m_assetsName[i] == name && m_assets[i].GetType() == type)
                        return m_assets[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        public void Clear(bool unloadAllLoadedObjects = false)
        {
            m_mainAsset = null;
            m_mainAssetName = null;
            m_assets = null;
            m_assetsName = null;
            if (m_assetBundle != null)
                m_assetBundle.Unload(false);
            m_assetBundle = null;
            m_path = null;
            if (m_callBackFunc != null)
                m_callBackFunc.Clear();
            m_callBackFunc = null;
            m_type = EResType.None;
            m_tags = 0;
            m_bNeedCRC = false;
            m_CRC = 0;
        }

        /// <summary>
        /// 是否被加载过，即是否正在加载或已经加载完毕
        /// </summary>
        public bool IsLoaded { get { return m_loadState == EResItemLoadState.Loading || m_loadState == EResItemLoadState.Completed; } }
        public bool IsCompleted { get { return m_loadState == EResItemLoadState.Completed; } }
        public bool IsClearAfterLoaded { get { return m_clearAfterLoaded; } set { m_clearAfterLoaded = value; } }
        public bool IsSaveAssetBundle { get { return m_saveAssetBundle; } }
        public string Path { get { return m_path; } }
        public EResType Type { get { return m_type; } }
        public Object MainAsset { get { return m_mainAsset != null ? m_mainAsset : Assets[0]; } }
        public Object[] Assets { get { return m_assetBundle != null ? m_assetBundle.LoadAllAssets() : m_assets; } }
        public bool NeedCRC { get { return m_bNeedCRC; } set { m_bNeedCRC = value; } }
        public uint CRC { get { return m_CRC; } set { m_CRC = value; } }
        public int Tag { get { return m_tags; } }
    }

}
