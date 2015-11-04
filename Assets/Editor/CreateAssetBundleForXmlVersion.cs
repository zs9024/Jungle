using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class CreateAssetBundleForXmlVersion
{
    public static void Execute(UnityEditor.BuildTarget target)
    {
        string SavePath = AssetBundleController.GetPlatformPath(target);
        Object obj = AssetDatabase.LoadAssetAtPath(SavePath + "VersionNum/VersionNum.xml", typeof(Object));
        BuildPipeline.BuildAssetBundle(obj, null, SavePath + "VersionNum/VersionNum.assetbundle", BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.DeterministicAssetBundle, target);

        AssetDatabase.Refresh();
    }

    public static void BuildMd5File(UnityEditor.BuildTarget target)
    {
        string SavePath = AssetBundleController.GetPlatformPath(target);
        Object obj = AssetDatabase.LoadAssetAtPath(SavePath + "VersionNum/VersionMD5.xml", typeof(Object));
        BuildPipeline.BuildAssetBundle(obj, null, SavePath + "VersionNum/VersionMD5.unity3d", BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.DeterministicAssetBundle, target);

        AssetDatabase.Refresh();
    }

    static string ConvertToAssetBundleName(string ResName)
    {
        return ResName.Replace('/', '.');
    }

}