using UnityEngine;
using System.Collections;
using System;
using GameLogic.IO;

/**
 * 主场景
 */ 
public class SceneMain : SceneBase
{
    private Action<int> onSetProgress;
    public SceneMain()
    {
        SNode = SceneNode.Main;
        SkyboxName = ABConfig.MAT_SKYBOX_MAIN;
    }


    //重写的加载场景方法
    public override void LoadScene()
    {
        OnLoadFinish = OnSceneLoaded;
        OnLoadSkyFinish = OnSkyboxLoaded;
        GlobalDelegate.Instance.View.StartCoroutine(SceneLoader.LoadSceneAsyn(LevelName, OnLoadFinish, onSetProgress, OnOtherLoad));
        //SceneLoader.LoadScene(LevelName, OnSceneLoaded);
    }

    public override void OnSceneLoaded()
    {
        Debug.Log("End LoadScene : " + LevelName + " ;End time ： " + System.DateTime.Now.ToString());
        UpdateSkybox();
        CreateCreatures();
    }

    public override void UpdateSkybox()
    {
        base.UpdateSkybox();
    }

    /// <summary>
    /// 设置加载主场景时的加载进度
    /// </summary>
    /// <param name="onSetProgress"></param>
    public void SetLoadingProgress(Action<int> onSetProgress)
    {
        this.onSetProgress = onSetProgress;
    }

    public void OnSkyboxLoaded()
    {
        UIManager.DestroyPanel<PanelLoadingScene>();
    }

    public void OnOtherLoad()
    {
        loadCreatures(ABConfig.HERO_JUGG, 1);
        loadCreatures(ABConfig.MONSTER_WOLF, 3);      
        loadLifeBar();
        var dm = DamageManager.Instance;
    }

    private void loadCreatures(string name,int initSize)
    {
        ResourceManager.LoadAsset(name, (UnityEngine.Object obj) =>
        {
            GameObjectPool.CreatePool(obj as GameObject, initSize);
        });
    }

    private void loadLifeBar()
    {
        ResourceManager.LoadAsset(ABConfig.UI_LIFEBAR, (UnityEngine.Object obj) =>
        {
            GameObjectPool.CreatePool(obj as GameObject, 3);
        });
    }

    protected virtual void CreateCreatures()
    {
        Debug.Log("CreateCreatures...");
        GlobalDelegate.Instance.View.StartCoroutine(ResLoader.ReadStreamingFile(GlobalConfig.CreatureConfPathStraming, (string text) =>
        {
            GlobalConfig.CreatureConf = JsonFx.Json.JsonReader.Deserialize<CreatureConfig>(text);
            Debug.Log(GlobalConfig.CreatureConf.MonsterConfig.MonsterGoblinConfig.name);
            CreatureManager.Spwan<Hero_JUGG>();

            CreatureManager.Spwan<Monster_Wolf>(new object[]{GlobalConfig.MonsterWolfKingName});
            CreatureManager.Spwan<Monster_Wolf>(new object[] { GlobalConfig.MonsterWolfName });          
        }));

        UIManager.ShowPanel<PanelMain>();
    }

}
