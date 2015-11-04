#define WinDebug

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// assetbundle资源路径配置
/// by zs
/// </summary>
public class AssetConfig
{
    //外部资源路径，网络更新的资源会存在这里
    private static string AssetPathExternal
    {
        get {

            switch (Application.platform)
            {
                case RuntimePlatform.IPhonePlayer:
                    return "file://" + DownLoadData.ExternalAssetPath + "/";
                case RuntimePlatform.Android:
                    return "file://" + DownLoadData.ExternalAssetPath + "/";
                case RuntimePlatform.WindowsEditor:
#if WinDebug
                    return "file://" + Application.dataPath.Replace('\\', '/') + "/AssetBundle/Windows" + "/";
#else
                    return "file://" + DownLoadData.ExternalAssetPath.Replace('\\', '/') + "/";
#endif
                default:
                    return "file://" + DownLoadData.ExternalAssetPath.Replace('\\', '/') + "/";
            }
        }
    }

    //本地资源路径
    private static string AssetPathLocal
    {
        get
        {
            return "file://" + Application.streamingAssetsPath + "/";
        }
    }


    #region 共享资源
    public static string UISharedAsset = AssetPathExternal + "sharedAssets/Public_106-Microsoft_black-Crystal-sharedAssets.unity3d";
    #endregion

    public static string CubeTest = AssetPathExternal + "Prefabs/Assets.Plugins.ObjectPool.Demo.Prefabs.Bullet.unity3d";
    
}
