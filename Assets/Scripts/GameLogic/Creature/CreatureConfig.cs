using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 生物属性配置，从文件读取
/// 变量名要和json串key一致
/// </summary>
public class CreatureConfig
{
    public HeroConf HeroConfig;
    public MonsterConf MonsterConfig;
}

/// <summary>
/// 英雄属性配置
/// </summary>
public class HeroConf
{
    public CreatureProp HeroJUGGConfig;
}

/// <summary>
/// 怪物属性配置
/// </summary>
public class MonsterConf
{
    public CreatureProp[] MonsterWolfConfig;
    public CreatureProp MonsterGoblinConfig;
}

public class CreatureProp
{
    public string name;     //游戏中显示的名称

    public int HP;

    public int MP;

    public int level;

    public int attackPower;

    public int defencePower;

    public float attackDistance;

    public string position;

    public string lifeBar;

    public string scaleSize;
}

