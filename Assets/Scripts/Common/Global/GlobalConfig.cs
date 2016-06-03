using UnityEngine;
using System.Collections;

/// <summary>
/// 全局配置类
/// </summary>
public class GlobalConfig
{
    /// <summary>
    /// 初始化配置
    /// </summary>
    public static void Init()
    {
        SetPath();
        SetLayer();
        OriginResolution = Screen.currentResolution;
        Application.targetFrameRate = TargetFraneRate;
    }

    #region 系统路径
    public static string DataPath { get; private set; }
    public static string PersistentDataPath { get; private set; }
    public static string StreamingPath { get; private set; }

    /*****************************************************************************/
    /// <summary>
    /// PlatformStreamingAssetsPath各个平台对应的PlatformStreaming文件夹目录
    /// </summary>
    public static string PlatformStreamingAssetsPath
    {
        get
        {
            switch (Application.platform)
            {
                case RuntimePlatform.OSXEditor:
                    return "file://" + StreamingPath;
                case RuntimePlatform.IPhonePlayer:
                    return "file://" + DataPath + "/Raw";
                case RuntimePlatform.Android:
                    return "jar:file://" + DataPath + "!/assets";
                case RuntimePlatform.WindowsEditor:
                    return "file://" + StreamingPath;
                default:
                    return "file://" + StreamingPath;
            }
        }
    }
    /*****************************************************************************/

    /// <summary>
    /// 设置平台路径
    /// </summary>
    public static void SetPath()
    {
        DataPath = Application.dataPath;
        PersistentDataPath = Application.persistentDataPath;
        StreamingPath = Application.streamingAssetsPath;
    }
    /*****************************************************************************/

    #endregion

    #region 分辨率,帧率
    public static Resolution OriginResolution {get; set;}
    public static int TargetFraneRate = 30;
    #endregion

    #region 射线与层
    // Layer Name
    public static string LAYER_NAME_UI = "UI";
    public static string LAYER_NAME_3DUI = "3DUI";
    public static string LAYER_NAME_PLAYER = "Player";
    public static string LAYER_NAME_MONSTER = "Monster";

    // Layer Mask
    public static int LAYER_MASK_UI = 0;
    public static int LAYER_MASK_3DUI = 0;

    // Layer Value
    public static int LAYER_VALUE_UI = 0;
    public static int LAYER_VALUE_3DUI = 0;
    public static int LAYER_VALUE_PLAYER = 0;
    public static int LAYER_VALUE_MONSTER = 0;

    /// <summary>
    /// 设置层
    /// </summary>
    public static void SetLayer()
    {
        LAYER_VALUE_UI = LayerMask.NameToLayer(LAYER_NAME_UI);
        LAYER_VALUE_3DUI = LayerMask.NameToLayer(LAYER_NAME_3DUI);
        LAYER_VALUE_PLAYER = LayerMask.NameToLayer(LAYER_NAME_PLAYER);
        LAYER_VALUE_MONSTER = LayerMask.NameToLayer(LAYER_NAME_MONSTER);

        LAYER_MASK_UI = LayerMask.GetMask(LAYER_NAME_UI);
        LAYER_MASK_3DUI = LayerMask.GetMask(LAYER_NAME_3DUI);
    }
    /*****************************************************************************/
#endregion

    #region 生物配置
    //生物配置文件
    public const string CreatureConfName = "/Config/CreatureConfig.conf";
    public static string CreatureConfPathStraming
    {
        get { return PlatformStreamingAssetsPath + CreatureConfName; }
    }

    //全局的生物配置对象
    public static CreatureConfig CreatureConf { get; set; }

    public static string MonsterWolfKingName = "头狼";
    public static string MonsterWolfName = "狼";

    public static CreatureProp GetMonsterWolfProp(string name)
    {
        var wolfs = CreatureConf.MonsterConfig.MonsterWolfConfig;
        if(wolfs == null || wolfs.Length ==0)
        {
            return null;
        }

        for(int i = 0; i< wolfs.Length; i++)
        {
            if(wolfs[i].name == name)
            {
                return wolfs[i];
            }
        }

        return null;
    }

    public static CreatureProp[] GetMonsterWolfPropAll()
    {
        return CreatureConf.MonsterConfig.MonsterWolfConfig;
    }

    public static CreatureProp GetHeroProp()
    {
        return CreatureConf.HeroConfig.HeroJUGGConfig;
    }

    #endregion

    #region 动画控制器中的动画状态
    //常用状态，是可以英雄和怪物公用的
    public const int AC_STATE_Spwan    = 1;
    public const int AC_STATE_Idle     = 2;
    public const int AC_STATE_Walk     = 3;
    public const int AC_STATE_Run      = 4;
    public const int AC_STATE_Atk      = 5;
    public const int AC_STATE_Hit      = 6;
    public const int AC_STATE_Die      = 7;

    //jugg技能
    public const int AC_STATE_JUGG_SKILL_Showoff = 11;
    public const int AC_STATE_JUGG_SKILL_Spin    = 12;
    #endregion
}
