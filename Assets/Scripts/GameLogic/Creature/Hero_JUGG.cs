using UnityEngine;
using System.Collections;

public class Hero_JUGG : Hero
{
    
    public Hero_JUGG()
    {
        SetAssetPath(ABConfig.HERO_JUGG);
        assetName = "Hero_JUGG";
        heroName = "JUGG";
    }

    protected override void Init()
    {
        base.Init();
        Debug.Log("Hero_JUGG Init()...");

        creatureProp = GlobalConfig.GetHeroProp();
    }

    protected override void OnShow()
    {
        base.OnShow();

        gObj.name = assetName;
        gObj.ApplyLayer(GlobalConfig.LAYER_VALUE_PLAYER);
        //add 行为树
        if(heroGo == null)
        {
            return;
        }
        var btHero = heroGo.AddComponent<BTCharacator>();
        btHero.speed = 2f;

        var seeker = heroGo.AddComponent<Seeker>();
    }
}
