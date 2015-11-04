using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class CreateAssetBundle
{

    private const string SharedAssetDir = "sharedAssets/";

    /// <summary>
    /// 单独打包，自身依赖
    /// </summary>
    /// <param name="target">平台</param>
    public static void Execute(UnityEditor.BuildTarget target)
    {
        string SavePath = AssetBundleController.GetPlatformPath(target);

        // 当前选中的资源列表
        foreach (Object o in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
        {
            string path = AssetDatabase.GetAssetPath(o);
            Debug.Log("path is ::" + path);             //path is ::Assets/Resources/prefab/Cylinder.prefab

            //获得所在文件夹
            string[] dirArray = path.Split('/');
            string dir = dirArray[dirArray.Length - 2];
            Debug.Log("the file's dir is :" + dir);

            // 过滤掉meta文件和文件夹
            if (path.Contains(".meta") || path.Contains(".") == false)
                continue;

            // 过滤掉UIAtlas目录下的贴图和材质(UI/Common目录下的所有资源都是UIAtlas)
            if (path.Contains("UI/Common"))
            {
                if ((o is Texture) || (o is Material))
                    continue;
            }

            string fileDirectory = SavePath + dir;
            if (!Directory.Exists(fileDirectory))
                Directory.CreateDirectory(fileDirectory);

            path = fileDirectory + "/" + ConvertToAssetBundleName(path);   //文件路径转成文件名
            path = path.Substring(0, path.LastIndexOf('.'));
            path += ".unity3d";

            Debug.Log("Asset path is ::" + path);       //Asset path is ::Assets/AssetBundle/Windows/Assets.Resources.prefab.Cylinder.unity3d

            BuildPipeline.BuildAssetBundle(o, null, path, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.DeterministicAssetBundle, target);
        }

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 构建共享资源的assetbundle
    /// </summary>
    /// <param name="target">平台</param>
    /// <param name="sharedAssets">共享资源</param>
    /// <param name="assets">要打包的资源</param>
    public static void BuildSharedAssets(UnityEditor.BuildTarget target, List<Object> sharedAssets, List<Object> assets)
    {
        if (sharedAssets == null || sharedAssets.Count <= 0 || assets == null || assets.Count <= 0)
        {
            Debug.LogError("The raw resources are error!!!");
            return;
        }

        string savePath = AssetBundleController.GetPlatformPath(target);

        string sharedAssetsName = string.Empty;
        //取共享资源的文件名
        for (int i = 0; i < sharedAssets.Count; i++)
        {
            string path = AssetDatabase.GetAssetPath(sharedAssets[i]);
            int index1 = path.LastIndexOf('/');
            int index2 = path.LastIndexOf('.');

            sharedAssetsName += path.Substring(index1 + 1, index2 - index1 - 1);
            sharedAssetsName += "-";
            Debug.Log("sharedName:" + sharedAssetsName);
        }

        BuildAssetBundleOptions buildOp = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets
            | BuildAssetBundleOptions.DeterministicAssetBundle;

        BuildPipeline.PushAssetDependencies();

        string buildPath = savePath + SharedAssetDir;
        if (!Directory.Exists(buildPath))
            Directory.CreateDirectory(buildPath);

        BuildPipeline.BuildAssetBundle(null, sharedAssets.ToArray(), buildPath + sharedAssetsName + "sharedAssets.unity3d", buildOp, target);

        BuildPipeline.PushAssetDependencies();
        //遍历所有的游戏对象
        foreach (Object obj in assets)
        {
            string path = AssetDatabase.GetAssetPath(obj);

            //获得所在文件夹
            string[] dirArray = path.Split('/');
            string dir = dirArray[dirArray.Length - 2];
            Debug.Log("the file's dir is :" + dir);

            string fileDirectory = savePath + dir;
            if (!Directory.Exists(fileDirectory))
                Directory.CreateDirectory(fileDirectory);

            path = fileDirectory + "/" + ConvertToAssetBundleName(path);   //文件路径转成文件名
            path = path.Substring(0, path.LastIndexOf('.'));    //去掉后缀
            path += ".unity3d";

            BuildPipeline.PushAssetDependencies();
            if (BuildPipeline.BuildAssetBundle(obj, null, path, buildOp, target))
            {
                Debug.Log(obj.name + "资源打包成功");
            }
            else
            {
                Debug.Log(obj.name + "资源打包失败");
            }
            BuildPipeline.PopAssetDependencies();

        }
        BuildPipeline.PopAssetDependencies();

        BuildPipeline.PopAssetDependencies();

        EditorUtility.DisplayDialog("", "Completed", "OK");
        AssetDatabase.Refresh();

    }



    static string ConvertToAssetBundleName(string ResName)
    {
        return ResName.Replace('/', '.');
    }


    [MenuItem("CustomEditor/Build AssetBundle From Selection - Track dependencies")]
    static void ExportResource()
    {

        string path = EditorUtility.SaveFilePanel("Save Resource", "", "New Resource", "unity3d");
        if (path.Length != 0)
        {
            // Build the resource file from the active selection.
            Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path,
                                BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets);
            Selection.objects = selection;
        }
    }

    [MenuItem("CustomEditor/Build AssetBundle From Selection - No dependency tracking")]
    static void ExportResourceNoTrack()
    {
        string path = EditorUtility.SaveFilePanel("Save Resource", "", "New Resource", "unity3d");
        if (path.Length != 0)
        {
            BuildPipeline.BuildAssetBundle(Selection.activeObject, Selection.objects, path, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, BuildTarget.StandaloneWindows);
        }
    }

    [MenuItem("CustomEditor/Make unity3d file to bytes file")]
    static void ExportResource2Bytes()
    {
        string path = EditorUtility.SaveFilePanel("Save Resource", "", "New Resource", "unity3d");
        if (path.Length != 0)
        {

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            byte[] buff = new byte[fs.Length];
            fs.Read(buff, 0, (int)fs.Length);
            string password = "et2t";
            packXor(buff, buff.Length, password);
            Debug.Log("filelength:" + buff.Length);
            fs.Close();
            File.Delete(path);

            string BinPath = path.Substring(0, path.LastIndexOf('.')) + ".bytes";
            FileStream cfs = new FileStream(BinPath, FileMode.Create);
            cfs.Write(buff, 0, buff.Length);
            Debug.Log("filelength:" + buff.Length);
            buff = null;
            cfs.Close();

        }
    }

    static void packXor(byte[] _data, int _len, string _pstr)
    {
        int length = _len;
        int strCount = 0;


        for (int i = 0; i < length; ++i)
        {
            if (strCount >= _pstr.Length)
                strCount = 0;
            _data[i] ^= (byte)_pstr[strCount++];
        }
    }



    [MenuItem("CustomEditor/Create AssetBunldes One")]
    static void CreateAssetBunldesMain()
    {
        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

        foreach (Object obj in SelectedAsset)
        {
            //string sourcePath = AssetDatabase.GetAssetPath(obj);
            //本地测试：建议最后将Assetbundle放在StreamingAssets文件夹下，如果没有就创建一个，因为移动平台下只能读取这个路径
            //StreamingAssets是只读路径，不能写入
            //服务器下载：就不需要放在这里，服务器上客户端用www类进行下载。
            string targetPath = Application.dataPath + "/StreamingAssets/" + obj.name + ".assetbundle";
            if (BuildPipeline.BuildAssetBundle(obj, null, targetPath, BuildAssetBundleOptions.CollectDependencies))
            {
                Debug.Log(obj.name + "资源打包成功");
            }
            else
            {
                Debug.Log(obj.name + "资源打包失败");
            }
        }
        //刷新编辑器
        AssetDatabase.Refresh();

    }

    [MenuItem("CustomEditor/Create AssetBunldes ALL")]
    static void CreateAssetBunldesALL()
    {

        Caching.CleanCache();

        string Path = Application.dataPath + "/StreamingAssets/sharedAsset.assetbundle";

        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

        foreach (Object obj in SelectedAsset)
        {
            Debug.Log("Create AssetBunldes name :" + obj);
        }

        //这里注意第二个参数就行
        if (BuildPipeline.BuildAssetBundle(null, SelectedAsset, Path, BuildAssetBundleOptions.CollectDependencies))
        {
            AssetDatabase.Refresh();
        }
        else
        {

        }
    }

    /// <summary>
    /// 构建共享资源的assetbundle
    /// Push和Pop成对使用，一个Push/Pop对就相当于一个Layer（层），层可以嵌套，内层可以依赖外层的资源。
    /// 也就是说内层某资源在打包时，如果其引用的某个资源已经在外层加载了，那么内层的这个资源包就会包含该资源的引用而不是资源本身。
    /// Push/Pop实际上维持了一个依赖的堆栈。
    /// </summary>
    [MenuItem("CustomEditor/Create Shared Resources AssetBunldes ")]
    static void ExportResourcesShared()
    {
        string SavePath = Application.streamingAssetsPath + "/";//"D:/work/UnityProj/Resources/AssetsStreamingAssets";

        BuildAssetBundleOptions buildOp = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets
            | BuildAssetBundleOptions.DeterministicAssetBundle;

        BuildPipeline.PushAssetDependencies();
        // 共享资源
        Object sharedAsset1 = AssetDatabase.LoadMainAssetAtPath("Assets/Font/Microsoft_black.ttf");
        Object sharedAsset2 = AssetDatabase.LoadMainAssetAtPath("Assets/Atlas/NGUI/PublicAtlas.prefab");

        Object[] assets = new Object[2];
        assets[0] = sharedAsset1;
        assets[1] = sharedAsset2;

        BuildPipeline.BuildAssetBundle(null, assets, SavePath + "sharedAsset.unity3d", buildOp, BuildTarget.StandaloneWindows);

        //获取在Project视图中选择的所有游戏对象
        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

        BuildPipeline.PushAssetDependencies();
        //遍历所有的游戏对象
        foreach (Object obj in SelectedAsset)
        {
            string targetPath = Application.dataPath + "/StreamingAssets/" + obj.name + ".unity3d";

            BuildPipeline.PushAssetDependencies();
            if (BuildPipeline.BuildAssetBundle(obj, null, targetPath, buildOp, BuildTarget.StandaloneWindows))
            {
                Debug.Log(obj.name + "资源打包成功");
            }
            else
            {
                Debug.Log(obj.name + "资源打包失败");
            }
            BuildPipeline.PopAssetDependencies();

        }
        BuildPipeline.PopAssetDependencies();

        BuildPipeline.PopAssetDependencies();

        EditorUtility.DisplayDialog("", "Completed", "OK");
        AssetDatabase.Refresh();
    }




}




