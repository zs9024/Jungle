using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class AssetBundleController : EditorWindow
{
    public static AssetBundleController window;
    public static UnityEditor.BuildTarget buildTarget = BuildTarget.StandaloneWindows;

    [MenuItem("CustomEditor/AssetBundle/AssetBundle For Windows", false, 1)]
    public static void ExecuteWindows()
    {
        if (window == null)
        {
            window = (AssetBundleController)GetWindow(typeof(AssetBundleController));
        }
        buildTarget = UnityEditor.BuildTarget.StandaloneWindows;
        window.Show();
    }

    [MenuItem("CustomEditor/AssetBundle/AssetBundle For Android", false, 2)]
    public static void ExecuteAndroid()
    {
        if (window == null)
        {
            window = (AssetBundleController)GetWindow(typeof(AssetBundleController));
        }
        buildTarget = UnityEditor.BuildTarget.Android;
        window.Show();
    }

    [MenuItem("CustomEditor/AssetBundle/AssetBundle For IPhone", false, 3)]
    public static void ExecuteIPhone()
    {
        if (window == null)
        {
            window = (AssetBundleController)GetWindow(typeof(AssetBundleController));
        }
        buildTarget = UnityEditor.BuildTarget.iPhone;
        window.Show();
    }

    [MenuItem("CustomEditor/AssetBundle/AssetBundle For Mac", false, 4)]
    public static void ExecuteMac()
    {
        if (window == null)
        {
            window = (AssetBundleController)GetWindow(typeof(AssetBundleController));
        }
        buildTarget = UnityEditor.BuildTarget.StandaloneOSXUniversal;
        window.Show();
    }


    //public static ShraedAssetBundle sharedWindow;
    void OnGUI()
    {
        if (GUI.Button(new Rect(10f, 10f, 200f, 50f), "CreateAssetBundle"))
        {
            CreateAssetBundle.Execute(buildTarget);
            EditorUtility.DisplayDialog("", "CreateAssetBundle Completed", "OK");
        }

        if (GUI.Button(new Rect(10f, 80f, 200f, 50f), "Create Shared AssetBundle"))
        {
            ShraedAssetBundle sharedWindow = (ShraedAssetBundle)GetWindow(typeof(ShraedAssetBundle));

            if (sharedWindow != null)
            {
                sharedWindow.target = buildTarget;
                sharedWindow.Show();
            }

        }

        if (GUI.Button(new Rect(10f, 150f, 200f, 50f), "Generate MD5"))
        {
            CreateMD5List.Execute(buildTarget);
            EditorUtility.DisplayDialog("", "Generate MD5 Completed", "OK");
        }

        if (GUI.Button(new Rect(10f, 220f, 200f, 50f), "Build MD5 File"))
        {
            CreateAssetBundleForXmlVersion.BuildMd5File(buildTarget);
            EditorUtility.DisplayDialog("", "Build MD5 File Completed", "OK");
        }

        if (GUI.Button(new Rect(10f, 290f, 200f, 50f), "Generate Version"))
        {
//             VersionEditor versionWindow = (VersionEditor)GetWindow(typeof(VersionEditor));
//             if (versionWindow != null)
//             {
//                 versionWindow.target = buildTarget;
//                 versionWindow.Show();
//             }
        }
// 
//         if (GUI.Button(new Rect(10f, 220f, 200f, 50f), "(4)Build VersionNum.xml"))
//         {
//             CreateAssetBundleForXmlVersion.Execute(buildTarget);
//             EditorUtility.DisplayDialog("", "Step (4) Completed", "OK");
//         }
    }

    /// <summary>
    /// 不同平台AB资源放到不同目录
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static string GetPlatformPath(UnityEditor.BuildTarget target)
    {
        string SavePath = "";
        switch (target)
        {
            case BuildTarget.StandaloneWindows:
                SavePath = "Assets/AssetBundle/Windows/";
                break;
            case BuildTarget.StandaloneWindows64:
                SavePath = "Assets/AssetBundle/Windows64/";
                break;
            case BuildTarget.iPhone:
                SavePath = "Assets/AssetBundle/IOS/";
                break;
            case BuildTarget.StandaloneOSXUniversal:
                SavePath = "Assets/AssetBundle/Mac/";
                break;
            case BuildTarget.Android:
                SavePath = "Assets/AssetBundle/Android/";
                break;
            default:
                SavePath = "Assets/AssetBundle/";
                break;
        }

        if (Directory.Exists(SavePath) == false)
            Directory.CreateDirectory(SavePath);

        return SavePath;
    }

    public static string GetPlatformName(UnityEditor.BuildTarget target)
    {
        string platform = "Windows";
        switch (target)
        {
            case BuildTarget.StandaloneWindows:
                platform = "Windows";
                break;
            case BuildTarget.StandaloneWindows64:
                platform = "Windows64";
                break;
            case BuildTarget.iPhone:
                platform = "IOS";
                break;
            case BuildTarget.StandaloneOSXUniversal:
                platform = "Mac";
                break;
            case BuildTarget.Android:
                platform = "Android";
                break;
            default:
                break;
        }
        return platform;
    }

}