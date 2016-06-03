using UnityEngine;
using System.Collections;
using System;

public class SceneLoader
{
    private static AsyncOperation asynOpera;
    private static float time;
    //public static Action<int> SetLoadingProgress;

    private static bool otherLoaded = false;
    /// <summary>
    /// 同步加载场景
    /// </summary>
    /// <param name="levelName">场景名称</param>
    /// <param name="onLoadFinish">加载完成回调</param>
    public static void LoadScene(string levelName, System.Action onLoadFinish = null)
    {
        if (string.IsNullOrEmpty(levelName))
            return;

        Debug.Log("Start LoadScene : " + levelName + " ;Start time ： " + DateTime.Now.ToString());
        time = Time.realtimeSinceStartup;

        preLevelLoaded(levelName, onLoadFinish);
        Application.LoadLevel(levelName);
    }

    /// <summary>
    /// 异步加载场景，并设置平滑进度
    /// </summary>
    /// <param name="levelName"></param>
    /// <param name="onLoadFinish"></param>
    /// <param name="SetLoadingProgress"></param>
    /// <returns></returns>
    public static IEnumerator LoadSceneAsyn(string levelName, Action onLoadFinish = null, Action<int> SetLoadingProgress = null,Action other = null)
    {
        if (string.IsNullOrEmpty(levelName))
            yield break;

        Debug.Log("Start LoadSceneAsyn : " + levelName + " ;Start time ： " + DateTime.Now.ToString());
        time = Time.realtimeSinceStartup;
        preLevelLoaded(levelName, onLoadFinish);

        int displayProgress = 0;
        int toProgress = 0;

        asynOpera = Application.LoadLevelAsync(levelName);
        asynOpera.allowSceneActivation = false;             //禁止Unity加载完毕后自动切换场景,但是进度只能到90

        while (asynOpera.progress < 0.9f)
        {
            toProgress = (int)asynOpera.progress * 100;
            while (displayProgress < toProgress)
            {
                ++displayProgress;                          //使进度连续平滑
                if (SetLoadingProgress != null)
                {
                    SetLoadingProgress(displayProgress);
                }
                
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame();
        }

        toProgress = 100;
        while (displayProgress < toProgress)
        {
            ++displayProgress;
            if (SetLoadingProgress != null)
            {
                SetLoadingProgress(displayProgress);
            }

            if (other != null && !otherLoaded)
            {
                other();
                otherLoaded = true;
            }

            yield return new WaitForEndOfFrame();
        }

        asynOpera.allowSceneActivation = true;          //切换场景
        asynOpera = null;         
    }


    /// <summary>
    /// 场景加载完成的预处理，将完成回调注册
    /// </summary>
    /// <param name="levelName"></param>
    /// <param name="onLoadFinish"></param>
    private static void preLevelLoaded(string levelName, System.Action onLoadFinish = null)
    {
        System.Action onLevelLoaded = null;
        onLevelLoaded = () =>
        {
            if (levelName == Application.loadedLevelName)
            {
                GlobalDelegate.Instance.View.OnLevelLoaded -= onLevelLoaded;
                float endTime = Time.realtimeSinceStartup;
                Debug.Log("End LoadScene : " + levelName + " ;Cost time ： " + (endTime - time).ToString());

                if (onLoadFinish != null)
                {
                    GlobalDelegate.Instance.View.Attach(onLoadFinish);
                }                  
            }
        };

        GlobalDelegate.Instance.View.OnLevelLoaded += onLevelLoaded;
    }
}
