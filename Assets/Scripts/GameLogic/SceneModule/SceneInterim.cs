using UnityEngine;
using System.Collections;

public class SceneInterim : SceneBase
{
    public SceneInterim()
    {
        SNode = SceneNode.Interim;
    }

    //重写的加载场景方法
    public override void LoadScene()
    {
        SceneLoader.LoadScene(LevelName, OnSceneLoaded);
    }

    public override void OnSceneLoaded()
    {
        Debug.Log("End LoadScene : " + LevelName + " ;End time ： " + System.DateTime.Now.ToString());

        UIManager.ShowPanel<PanelLoadingScene>();
    }
}
