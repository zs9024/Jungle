using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * 场景管理类，负责场景的加载和销毁，最主要是管理场景的跳转和显示顺序
 * 场景之间跳转会有多种情况，有顺序跳转（栈管理），也会有强制跳的，视情况而定
 */ 
public class SceneManager : Singleton<SceneManager>,IInit
{
    //当前场景
    private SceneBase currentScene;
    public SceneBase CurrentScene { get { return currentScene; } }

    //场景栈
    private LinkedList<SceneBase> sceneStack;

    //场景字典
    private Dictionary<string, SceneBase> sceneDic;

    public void Init()
    {
        sceneStack = new LinkedList<SceneBase>();
        sceneDic = new Dictionary<string, SceneBase>();
    }

    /// <summary>
    /// 根据场景节点创建场景对象
    /// </summary>
    /// <param name="snode"></param>
    /// <returns></returns>
    public SceneBase CreateScene(SceneNode snode)
    {
        switch(snode)
        {
            case SceneNode.Login:
                return new SceneLogin();
            case SceneNode.Main:
                return new SceneMain();
            default:
                return new SceneMain();
        }
    }

    /// <summary>
    /// 创建场景
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T CreateScene<T>() where T : SceneBase,new()
    {
        string sceneName = typeof(T).Name;
        if(sceneDic != null && !sceneDic.ContainsKey(sceneName))
        {
            sceneDic.Add(sceneName, new T());
        }

        return sceneDic[sceneName] as T;
    }
   

    /// <summary>
    /// 将当前场景入栈，并加载新场景
    /// </summary>
    /// <param name="next">待加载的场景</param>
    public void PushScene(SceneBase next)
    {
        if(next == null)
        {
            Debug.LogWarning("SceneManager.PushScene : Null GameScene.");
            return;
        }

        if(currentScene != null) 
        {
            if(currentScene.SNode == next.SNode)
            {
                Debug.LogWarning("SceneManager.PushScene : The Same GameScene.");
                return;
            }

            currentScene.OnPushed(next);
            sceneStack.AddLast(currentScene);
        }

        next.LoadScene();
        next.UpdateAudioState();
        next.UpdateGlobalLight();
        currentScene = next;
    }

    /// <summary>
    /// 弹出栈顶场景，并将当前场景销毁
    /// </summary>
    public void PopScene()
    {
        if(sceneStack.Count <= 0 || sceneStack.Last == null)
        {
            return;
        }

        SceneBase last = currentScene;
        SceneBase bePoped = sceneStack.Last.Value;

        if(currentScene != null)
        {
            currentScene.OnDestroy();
        }

        if(bePoped != null)
        {
            bePoped.OnPoped(last);

            bePoped.LoadScene();
            bePoped.UpdateAudioState();
            bePoped.UpdateGlobalLight();

            currentScene = bePoped;
        }

        sceneStack.RemoveLast();
    }
	
    /// <summary>
    /// 直接替换当前场景，无出入栈操作
    /// </summary>
    /// <param name="next"></param>
    public void ReplaceScene(SceneBase next)
    {
        if (next == null)
        {
            Debug.LogWarning("SceneManager.ReplaceScene : Null GameScene.");
            return;
        }

        if(currentScene != null)
        {
            currentScene.OnReplaced(next);
            currentScene.OnDestroy();
        }

        next.LoadScene();
        next.UpdateAudioState();
        next.UpdateGlobalLight();
        currentScene = next;
    }

    /// <summary>
    /// 同ReplaceScene，但将栈清空
    /// </summary>
    /// <param name="target"></param>
    public void SetScene(SceneBase target)
    {
        if(target != null)
        {
            ReplaceScene(target);
            sceneStack.Clear();
        }
    }

    /// <summary>
    /// 获取当前场景的上一个场景
    /// </summary>
    /// <returns></returns>
    public SceneNode GetLastScene()
    {
        if (sceneStack == null || sceneStack.Count <= 0) 
            return SceneNode.Login;

        LinkedListNode<SceneBase> last = sceneStack.Last;
        if (last != null && last.Value != null)
            return last.Value.SNode;

        return SceneNode.Main;
    }

}
