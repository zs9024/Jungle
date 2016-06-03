using UnityEngine;
using System.Collections;
// using Game.Asset;

/*
 * 场景基类
 * by zs
 */ 
public abstract class SceneBase
{
    //场景操作类型
    public enum SceneAction
    {
        Pushed, 
        Popped,
        Replaced, 
        Set
    }

    //场景节点，标识某个场景
    public SceneNode SNode { get; protected set; }

    //场景名称，加载场景时的名称
    public string LevelName { get{return SNode.ToString();} }

    //场景加载完成回调
    public System.Action OnLoadFinish = null;

    //天空盒资源名称 
    public string SkyboxName { get; protected set; }

    //天空盒材质
    public Material SkyboxMat { get; protected set; }

    //天空盒加载完成回调
    public System.Action OnLoadSkyFinish = null;

    //加载场景，每个场景的加载情况可能不同（如同步异步）
    public abstract void LoadScene();

    /// <summary>
    /// 场景加载完成
    /// </summary>
    public abstract void OnSceneLoaded();

    /// <summary>
    /// 更新场景声音
    /// </summary>
    public virtual void UpdateAudioState()
    {

    }

    /// <summary>
    /// 更新场景灯光
    /// </summary>
    public virtual void UpdateGlobalLight()
    {
        RenderSettings.ambientLight = Color.white;
    }

    public virtual void UpdateSkybox()
    {
        if(SkyboxMat != null)
        {
            RenderSettings.skybox = SkyboxMat;
            if(OnLoadSkyFinish != null)
            {
                OnLoadSkyFinish();
            }
        }
        else
        {
            if(string.IsNullOrEmpty(SkyboxName))
            {
                return;
            }

            ResourceManager.LoadMaterial(SkyboxName, (obj) =>
            {
                SkyboxMat = obj as Material;
                RenderSettings.skybox = SkyboxMat;
                if (OnLoadSkyFinish != null)
                {
                    OnLoadSkyFinish();
                }
            });
        }
    }

    /// <summary>
    /// 场景入栈时操作
    /// </summary>
    /// <param name="next">下一个场景</param>
    public virtual void OnPushed(SceneBase next)
    {
    }

    /// <summary>
    /// 场景出栈时操作
    /// </summary>
    /// <param name="last">上一个场景</param>
    public virtual void OnPoped(SceneBase last)
    {
    }

    /// <summary>
    /// 场景被替换时操作
    /// </summary>
    /// <param name="next"></param>
    public virtual void OnReplaced(SceneBase next)
    {

    }

    /// <summary>
    /// 场景销毁时操作
    /// </summary>
    public virtual void OnDestroy()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

}
