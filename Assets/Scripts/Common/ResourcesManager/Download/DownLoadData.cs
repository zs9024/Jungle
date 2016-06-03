using UnityEngine;
using System.Collections;
using System.IO;

/// <summary>
/// 下载数据类，主要包括各平台资源路径
/// by zs
/// </summary>
public class DownLoadData {

    public const string MD5FileName = "/VersionMD5.xml";
    public const string VersionFileName = "/VersionConfig.xml";

    //Application.persistentDataPath只能从主线程调用,要到主线程赋值
    public static string PersistentDataPath
    {
        //get { return Application.persistentDataPath; }  
        get;
        set;
    }

    //测试中资源下载路径
    public static string ResourcesUrlTest = "http://192.168.51.197:8080";
    //public static string ResourcesUrlTest = "http://192.168.1.103:8080";

    //正式资源下载路径
    public static string ResourcesUrlNet = "http://192.168.4.219:8080/app_resource";

    private static bool isLocalTest = false;    //

    public static string Md5FilePathExternal
    {
        get { return ExternalAssetPath +  MD5FileName; }
    }

    public static string Md5FilePathNet
    {
        get { return ServerResourcesUrl +  MD5FileName; }
    }

    public static string VersionFilePathExternal
    {
        get { return ExternalAssetPath + VersionFileName; }
    }

    public static string VersionFilePathNet
    {
        get { return ServerResourcesUrl + VersionFileName; }
    }

    /// <summary>
    /// 资源缓存路径，也是资源加载路径
    /// </summary>
    public static string ExternalAssetPath
    {
        get
        {
            switch (Application.platform)
            {
                case RuntimePlatform.IPhonePlayer:
                    return PersistentDataPath + "/DownLoad/ExternalLoad";
                case RuntimePlatform.Android:
                    return PersistentDataPath + "/DownLoad/ExternalLoad";
                case RuntimePlatform.WindowsEditor:
                    return Directory.GetParent(Directory.GetCurrentDirectory()).FullName + Path.DirectorySeparatorChar + "DownLoad" + Path.DirectorySeparatorChar + "ExternalLoad";
                case RuntimePlatform.OSXEditor:
                    return Directory.GetParent(Directory.GetCurrentDirectory()).FullName + Path.DirectorySeparatorChar + "DownLoad" + Path.DirectorySeparatorChar + "ExternalLoad";
                default:
                    return Directory.GetParent(Directory.GetCurrentDirectory()).FullName + Path.DirectorySeparatorChar + "DownLoad" + Path.DirectorySeparatorChar + "ExternalLoad";
            }
        }
    }

    /// <summary>
    /// 服务器资源下载路径，安卓和IOS会各有一套资源
    /// </summary>
    public static string ServerResourcesUrl
    {
        get
        {
            switch (Application.platform)
            {
                case RuntimePlatform.IPhonePlayer:
                    if (isLocalTest)
                        return ResourcesUrlTest + "/ios";
                    return ResourcesUrlNet + "/ios";

                case RuntimePlatform.Android:
                    if (isLocalTest)
                        return ResourcesUrlTest + "/android";
                    return ResourcesUrlNet + "/android";

                case RuntimePlatform.WindowsEditor:
                    if (isLocalTest)
                        return ResourcesUrlTest + "/windows";
                    return ResourcesUrlNet + "/windows";

                case RuntimePlatform.OSXEditor:
                    if (isLocalTest)
                        return ResourcesUrlTest + "/xcode";
                    return ResourcesUrlNet + "/xcode";

                default:
                    return ResourcesUrlTest + "/windows";
            }
        }
    }


    /// <summary>
    /// 获取文件的目录，跟服务器路径一致
    /// </summary>
    /// <param name="assetName">格式：Assets.Resources.prefab.Cylinder.unity3d</param>
    /// <returns></returns>
    public static string GetFileDirectory(string assetName)
    {
        if (string.IsNullOrEmpty(assetName))
            return string.Empty;

        string[] dirs = assetName.Split('.');

        return dirs[dirs.Length - 3];
    }


}

public enum DownloadStatue
{
    None = 0,               //没有状态
    UpdateSuccess = 1,      //更新成功
    UpdateFaild = 2,        //更新失败
    NotUpdate = 3,          //不用更新
    Incomplete = 4,         //更新不完整
    VersonFaild = 5,        //更新版本失败
    VersonSuccess = 6,      //更新版本成功
    FileIncomplete = 7,     //系统文件不完整
    NotSpace = 8,           //没有足够磁盘空间
    Updatable = 9          //有新版本，玩家可以自行选择是否更新
}
