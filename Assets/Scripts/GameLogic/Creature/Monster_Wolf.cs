using UnityEngine;
using System.Collections;

public class Monster_Wolf : Monster2
{
    public Monster_Wolf()
    {
        SetAssetPath("Prefabs/Monster_Wolf");
        assetName = "Monster_Wolf";
    }

    protected override void Init()
    {
        base.Init();
        Debug.Log("Monster_Wolf Init()...");

        creatureProp = GlobalConfig.GetMonsterWolfProp(param[0] as string);
    }

    protected override void OnShow()
    {
        base.OnShow();

        //scale
        Vector3 scale = creatureProp.scaleSize.String2Vector3();
        trans.localScale = scale;

        gObj.ApplyLayer(GlobalConfig.LAYER_VALUE_MONSTER);
        //add 怪物行为树
        var btMonster = gObj.AddComponent<BTMonster>();
        btMonster.target = GameObject.Find("Hero_JUGG/JUGG").transform;
        btMonster.tolerance = creatureProp.attackDistance;
        bTree = btMonster;
    }
}
