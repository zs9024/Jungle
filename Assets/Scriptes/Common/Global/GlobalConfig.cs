using UnityEngine;
using System.Collections;

/// <summary>
/// 全局配置类
/// </summary>
public class GlobalConfig
{
    public static string DataPath { get; set; }
    public static string PersistentDataPath { get; set; }
    public static string StreamingPath { get; set; }

    public static Resolution OriginResolution {get; set;}
 

    /// <summary>
    /// 设置平台路径
    /// </summary>
    public static void SetPath()
    {
        DataPath = Application.dataPath;
        PersistentDataPath = Application.persistentDataPath;
        StreamingPath = Application.streamingAssetsPath;
    }
}
