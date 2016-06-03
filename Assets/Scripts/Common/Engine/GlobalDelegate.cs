using UnityEngine;
using System.Collections;

/**
 * 全局的代理，持有AbstractView，来调用和管理生命周期、开协程等
 * 也可以通过Engine来做，但是Engine作为入口功能最好单一
 */
public class GlobalDelegate : Singleton<GlobalDelegate>, IInit
{
    private AbstractView view = null;
    public AbstractView View { get { return view; } set { view = value; } }

    public void Init()
    {
        if (view == null)
        {
            GameObject go = GameObject.Find("GlobalDelegate");
            if (go != null)
            {
                view = go.GetComponent<AbstractView>();
                if (view == null)
                {
                    view = go.AddComponent<AbstractView>();
                }
            }
            else
            {
                go = new GameObject("GlobalDelegate");
                view = go.AddComponent<AbstractView>();
            }

            view.OnUpdate += this.update;
            view.OnLevelLoaded += () =>
            {
                if (SceneManager.Instance != null && SceneManager.Instance.CurrentScene != null)
                {
                    SceneManager.Instance.CurrentScene.UpdateGlobalLight();
                }
            };

            GameObject.DontDestroyOnLoad(go);
        }
    }

    private void update()
    {

    }
}
