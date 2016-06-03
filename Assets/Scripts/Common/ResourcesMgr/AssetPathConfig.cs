using System.IO;
using UnityEngine;

namespace GameAssets
{
    public static class AssetPathConfig
    {
        public static string RESOURCES_PATH_EDITOR = "/../../../RuntimeResource/";
        public static string ASSETS_UPDATE_PATH = "/Download/Assets/";//热更资源目录
        public static string ASSETS_RESOURCES_SUB_PATH = "/Resources/";//资源子路径

        public static string GetPlatformName()
        {
            return GetPlatformForAssetBundles(Application.platform);
        }

        private static string GetPlatformForAssetBundles(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                case RuntimePlatform.OSXWebPlayer:
                case RuntimePlatform.WindowsWebPlayer:
                    return "WebPlayer";
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    return "Windows";
                case RuntimePlatform.OSXPlayer:
                    return "OSX";
                // Add more build targets for your own.
                // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
                default:
                    return null;
            }
        }

        /// <summary>
        /// 各个平台对应的PlatformStreaming文件夹目录
        /// </summary>
        public static string PlatformStreamingAssetsPathWWW
        {
            get
            {
                string pre = "file://";
                string path = PlatformStreamingAssetsPath.Replace("\\", "/");
                switch (Application.platform)
                {
                    case RuntimePlatform.Android:
                        pre = "";
                        break;
                    default:
                        break;
                }
                return string.Concat(pre, path);
            }
        }

        /// <summary>
        /// 各个平台对应的PlatformStreaming文件夹目录
        /// </summary>
        public static string PlatformStreamingAssetsPath
        {
            get
            {
                if (Application.platform == RuntimePlatform.OSXEditor)
                    return Application.streamingAssetsPath;

                else if (Application.platform == RuntimePlatform.WindowsWebPlayer)
                    return Application.streamingAssetsPath;

                else if (Application.platform == RuntimePlatform.OSXPlayer)
                    return Application.streamingAssetsPath;

                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                    return Application.dataPath + "/Raw";

                else if (Application.platform == RuntimePlatform.Android)
                    return Application.streamingAssetsPath + "/Resources";

                else if (Application.platform == RuntimePlatform.WindowsEditor  )
                    //return Application.dataPath + RESOURCES_PATH_EDITOR + GetPlatformName();
                    return Application.streamingAssetsPath + "/Resources";

                else if (Application.platform == RuntimePlatform.WindowsPlayer)
                    return Application.dataPath + "/StreamingAssets";

                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// 各个平台对应的PlatformPersistentDataPath文件夹目录
        /// </summary>
        public static string PlatformPersistentDataPath
        {
            get
            {
                return Application.persistentDataPath;
            }
        }

        public static string GetAssetPathWWW(string fileName)
        {
            string url = string.Concat(PlatformPersistentDataPath, ASSETS_UPDATE_PATH, fileName);
            if (File.Exists(url))
            {
                url = string.Concat(Application.isEditor ? "file:///" : "file://", url);
            }
            else
            {
                url = string.Concat(PlatformStreamingAssetsPathWWW, "/", fileName);
            }
            return url;
        }
    }
}
