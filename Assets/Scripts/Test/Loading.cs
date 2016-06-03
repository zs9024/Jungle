using UnityEngine;
using System.Collections;

public class Loading : MonoBehaviour
{

    //异步对象
    private AsyncOperation mAsyn;
    //绑定Tip的GUIText
    private GUIText mTip;
    //Tip集合，实际开发中需要从外部文件中读取
    private string[] mTips = new string[]
	                {
	                    "异步加载过程中你可以浏览游戏攻略",
		                "异步加载过程中你可以查看当前进度",
		                "异步加载过程中你可以判断是否加载完成",
		                "博主不理解轩6的读条为什么那样慢",
		                "难道是因为DOOM不懂Unity3D",
	                };
    void Start()
    {
        mTip = GameObject.Find("GUITips").GetComponent<GUIText>();
        StartCoroutine("Load");
    }

    IEnumerator Load()
    {
        mAsyn = Application.LoadLevelAsync("Main");
        yield return mAsyn;
    }

    void Update()
    {
        //如果场景没有加载完则显示Tip，否则显示最终的界面
        if (mAsyn != null && !mAsyn.isDone)
        {
            mTip.GetComponent<GUIText>().text = mTips[Random.Range(0, 5)] + "(" + (float)mAsyn.progress * 100 + "%" + ")";
        }
        else
        {
            Application.LoadLevel("Main");
        }
    }
}