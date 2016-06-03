using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 统一的动画事件类，使用时需要挂到带动画的物体上
/// </summary>
public class AnimEvent : MonoBehaviour
{
    //动画事件回调的字典，用来保存回调的调用顺序
    private Dictionary<int, System.Action> onAnimEventDic = new Dictionary<int, System.Action>();
    //带参数的动画事件回调的字典，用来保存回调的调用顺序
    private Dictionary<int, System.Action<object>> onAnimEventObjDic = new Dictionary<int, System.Action<object>>();
    //参数字典
    private Dictionary<int, object> paramDic = new Dictionary<int, object>();


    /// <summary>
    /// 注册无参事件
    /// </summary>
    /// <param name="clip">动画片</param>
    /// <param name="time">事件</param>
    /// <param name="onAnimEvent">回调</param>
    public void AddAnimEvent(AnimationClip clip, float time, System.Action onAnimEvent)
    {
        AnimationEvent animEvent = new AnimationEvent();
        //固定一个事件方法，就不需要对每个事件都写一个
        animEvent.functionName = "OnAnimEvent";
        animEvent.time = time;
        animEvent.messageOptions = SendMessageOptions.RequireReceiver;
        //用hash码记录是哪一个回调函数，还没想到更好的方法
        animEvent.intParameter = onAnimEvent.GetHashCode();

        if (!onAnimEventDic.ContainsKey(animEvent.intParameter))
        {
            onAnimEventDic.Add(animEvent.intParameter, onAnimEvent);
        }

        clip.AddEvent(animEvent);
    }

    /// <summary>
    /// 引擎动画事件的回调，在这里将注册进来的回调分发出去
    /// </summary>
    /// <param name="code"></param>
    public void OnAnimEvent(int code)
    {
        //Debug.Log("OnAnimEvent " + code);
        System.Action onEvent;
        //取到注册进某一时间的那个回调
        if (onAnimEventDic.TryGetValue(code, out onEvent))
        {
            if(onEvent != null)
            {
                onEvent();
            }
        }
    }

    
    public void AddAnimEvent(object obj, AnimationClip clip, float time, System.Action<object> onAnimEventObj)
    {
        AnimationEvent animEvent = new AnimationEvent();
        animEvent.functionName = "OnAnimEventParam";
        animEvent.time = time;
        animEvent.messageOptions = SendMessageOptions.RequireReceiver;
        //用hash码记录回调函数
        animEvent.intParameter = onAnimEventObj.GetHashCode();

        if (!onAnimEventObjDic.ContainsKey(animEvent.intParameter))
        {
            onAnimEventObjDic.Add(animEvent.intParameter, onAnimEventObj);
        }
        if (!paramDic.ContainsKey(animEvent.intParameter))
        {
            paramDic.Add(animEvent.intParameter, obj);
        }

        clip.AddEvent(animEvent);
    }

    public void OnAnimEventParam(int code)
    {
        //Debug.Log("OnAnimEventParam " + code);

        System.Action<object> onEvent;
        if (onAnimEventObjDic.TryGetValue(code, out onEvent))
        {
            object obj;
            if (paramDic.TryGetValue(code, out obj))
            {
                if (onEvent != null)
                {
                    onEvent(obj);
                }
            }           
        }
    }
    
}


