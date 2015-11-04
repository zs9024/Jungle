using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// 共享资源打包类
/// by zs
/// </summary>
public class ShraedAssetBundle : EditorWindow
{
    List<string> sharedAssetPath = new List<string>();
    List<string> assetPath = new List<string>();

    List<Object> sharedAssets = new List<Object>();
    List<Object> assets = new List<Object>();

    public UnityEditor.BuildTarget target = BuildTarget.StandaloneWindows;

    void OnGUI()
    {
        if (GUILayout.Button("添加共享资源", GUILayout.Width(200)))
        {
            addResources(sharedAssetPath, sharedAssets);
        }

        EditorGUILayout.LabelField("共享资源如下：");
        drawResPath(sharedAssetPath);
        
        if (GUILayout.Button("添加打包资源", GUILayout.Width(200)))
        {
            addResources(assetPath,assets);
        }

        EditorGUILayout.LabelField("打包资源如下：");
        drawResPath(assetPath);

        if (GUILayout.Button("打包", GUILayout.Width(200)))
        {
            CreateAssetBundle.BuildSharedAssets(target, sharedAssets, assets);
        }

    }

    void addResources(List<string> pathList,List<Object> objList)
    {
        //获取在Project视图中选择的所有游戏对象
        Object[] objs = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

        if (objs != null)
        {
            foreach (Object obj in objs)
            {
                if (objList != null && !objList.Contains(obj))
                    objList.Add(obj);

                string path = AssetDatabase.GetAssetPath(obj);
                if (pathList != null && !pathList.Contains(path))
                    pathList.Add(path);
            }
        }
        
    }

    void drawResPath(List<string> pathList)
    {
        if (pathList != null && pathList.Count > 0)
        {
            for (int i = 0; i < pathList.Count; i++)
            {
                EditorGUILayout.LabelField(pathList[i]);
            }
        }
    }
}
