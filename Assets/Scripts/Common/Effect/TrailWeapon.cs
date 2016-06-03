using UnityEngine;
using System.Collections;

public class TrailWeapon : MonoBehaviour {

    public WeaponTrail TrailObj;//武器柄的节点
    private float t = 0f;
    private float tempT = 0f;
    private float animationIncrement = 0.003f;

    private AnimEvent animEvent;//事件接收

    void Start()
    {
        //先把残影关掉
        TrailObj.SetTime(0.0f, 0.0f, 1.0f);
        initAnimEvent();
    }

    private void initAnimEvent()
    {
        animEvent = GetComponent<AnimEvent>();
        if(animEvent == null)
        {
            animEvent = gameObject.AddComponent<AnimEvent>();
        }

        //animEvent.AddAnimEvent()
    }
    //开启残影函数(用于动画关键帧调用)
    public void TrailStart()
    {
        Debug.Log("TrailStart...");
        //设置拖尾时长
        TrailObj.SetTime(0.3f, 0.3f, 1.0f);
        //开始进行拖尾
        TrailObj.StartTrail(0.5f, 0.4f);
    }

    //关闭残影函数(用于动画关键帧调用)
    public void TrailStop()
    {
        Debug.Log("TrailStop...");
        //清除拖尾
        TrailObj.ClearTrail();
    }

    void LateUpdate()
    {
        t = Mathf.Clamp(Time.deltaTime, 0, 0.066f);
        if (t > 0)
        {
            while (tempT < t)
            {
                tempT += animationIncrement;
                if (TrailObj.time > 0)
                {
                    TrailObj.Itterate(Time.time - t + tempT);
                }
                else
                {
                    TrailObj.ClearTrail();
                }
            }
            tempT -= t;
            if (TrailObj.time > 0)
            {
                TrailObj.UpdateTrail(Time.time, t);
            }
        }
    }
}
