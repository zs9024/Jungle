
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;



/// <summary>
/// 事件管理器
/// </summary>
public class EventManager : Singleton<EventManager>, IInit, ITick
{
    private Queue _eventQueue = new Queue();

    private Hashtable _listenerTable = new Hashtable();
    private float _weakReferenceGCTime = 0f;
    public bool LimitQueueProcesing = false;
    public float QueueProcessTime = 0f;
    public float weakReferenceGCDelay = 30f;

    /// <summary>
    /// 实现接口
    /// </summary>
    public void Init()
    {
        Engine.AddTick(this);
    }

    /// <summary>
    /// 实现接口
    /// </summary>
    public void Update()
    {
        float num = 0f;
        while (this._eventQueue.Count > 0)
        {
            if (this.LimitQueueProcesing && (num > this.QueueProcessTime))
            {
                return;
            }
            Event event2 = this._eventQueue.Dequeue() as Event;
            if (event2 != null)
            {
                this._TriggerEvent(event2.Context, event2.Name, event2.Data);
            }
            if (this.LimitQueueProcesing)
            {
                num += Time.deltaTime;
            }
        }

        this._weakReferenceGCTime += Time.deltaTime;

        //说明：弱引用判断的时间是eventManager的时间，而不是每一个事件的弱引用时间
        if (this._weakReferenceGCTime > this.weakReferenceGCDelay)
        {
            this._weakReferenceGCTime = 0f;
            this._GCWeakReference();
        }
    }

    #region 添加事件监听
    public bool AddEventListener<T>(object context, string eventName, T listener, bool useWeakReference = false, bool isOnce = false)
    {
        return this._AddEventListener(context, eventName, listener as Delegate, useWeakReference, isOnce);
    }

    public bool AddEventListener(object context, string eventName, EventListener listener, bool useWeakReference = false, bool isOnce = false)
    {
        return this._AddEventListener(context, eventName, listener, useWeakReference, isOnce);
    }

    /// <summary>
    /// 添加事件监听,默认不使用弱引用
    /// </summary>
    /// <param name="context"></param>
    /// <param name="eventName">事件名</param>
    /// <param name="listener"></param>
    /// <param name="useWeakReference">listener是否弱引用</param>
    /// <param name="isOnce">是否只调用一次</param>
    /// <returns>是否添加成功</returns>
    private bool _AddEventListener(object context, string eventName, Delegate listener, bool useWeakReference = false, bool isOnce = false)
    {
        if (((context == null) || (listener == null)) || string.IsNullOrEmpty(eventName))
        {
            Debug.Log( "Event Manager: AddListener failed due to no listener or event name specified.");
            return false;
        }

        List<EventReference> list = this._GetEventListenerList(context, eventName, true);
        if (list.Exists(l => l.Listener == listener))
        {
            Debug.Log( "Event Manager: Listener: " + listener.GetType().ToString() + " is already in list for event: " + eventName);
            return false;
        }
        list.Add(new EventReference(listener, useWeakReference, isOnce));

        return true;
    }

    #endregion

    #region 分发事件
    /// <summary>
    /// 分发事件
    /// </summary>
    /// <param name="context">标记对象，监听符合触发函数所属对象</param>
    /// <param name="eventName"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public bool DispatchEvent(object context, string eventName, params object[] data)
    {
        return this._DispatchEvent(context, eventName, data);
    }

    private bool _DispatchEvent(object context, string eventName, object[] data)
    {
        if (!this._listenerTable.ContainsKey(context))
        {
            return false;
        }
        this._eventQueue.Enqueue(new Event(context, eventName, data));
        return true;
    }
    #endregion

    #region 触发事件
    public bool TriggerEvent(object context, string eventName, params object[] data)
    {
        return this._TriggerEvent(context, eventName, data);
    }

    private bool _TriggerEvent(object context, string eventName, object[] data)
    {
        if (!this._listenerTable.ContainsKey(context))
        {
            Debug.Log("Event Manager: Event \"" + eventName + "\" triggered has no listeners!");
            return false;
        }

        List<EventReference> list = this._GetEventListenerList(context, eventName, false);

        if (list == null)
        {
            return false;
        }

        for (int i = list.Count - 1; i >= 0; i--)
        {
            Delegate listener = list[i].Listener;

            if (listener != null)
            {
                try
                {
                    if (listener is EventListener)
                    {
                        (listener as EventListener)(data);
                    }
                    else
                    {
                        listener.DynamicInvoke(data);
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogError("TriggerEvent: " + exception.ToString());
                }

                if (list != null && i < list.Count && list[i].IsOnce)
                {
                    this._RemoveEventListener(context, eventName, listener);
                }
            }
        }

        return true;
    }
    #endregion

    #region 移除事件监听
    public bool RemoveEventListener(object context, string eventName, EventListener listener)
    {
        return this._RemoveEventListener(context, eventName, listener);
    }

    public bool RemoveEventListener<T>(object context, string eventName, T listener)
    {
        return this._RemoveEventListener(context, eventName, listener as Delegate);
    }

    private bool _RemoveEventListener(object context, string eventName, Delegate listener)
    {
        if (!this._listenerTable.ContainsKey(context))
        {
            return false;
        }
        List<EventReference> list = this._GetEventListenerList(context, eventName, false);
        if (list == null)
        {
            return false;
        }
        EventReference item = list.Find(l => l.Listener == listener);
        if (item == null)
        {
            return false;
        }
        list.Remove(item);
        if (list.Count <= 0)
        {
            IDictionary<string, List<EventReference>> dictionary = this._listenerTable[context] as IDictionary<string, List<EventReference>>;
            dictionary.Remove(eventName);
            if (dictionary.Count <= 0)
            {
                this._listenerTable.Remove(context);
            }
        }
        return true;
    }
    #endregion
    public bool HasEventListener(object context, string eventName, EventListener listener)
    {
        if (!this._listenerTable.ContainsKey(context))
        {
            return false;
        }
        List<EventReference> list = this._GetEventListenerList(context, eventName, false);
        if (list == null)
        {
            return false;
        }

        if (!list.Exists(l => l.Listener == listener))
        {
            return false;
        }

        return true;
    }

    private List<EventReference> _GetEventListenerList(object context, string eventName, bool createIfNotExist = false)
    {
        if (!this._listenerTable.ContainsKey(context))
        {
            if (!createIfNotExist)
            {
                return null;
            }

            this._listenerTable.Add(context, new Dictionary<string, List<EventReference>>());
        }

        IDictionary<string, List<EventReference>> dictionary = this._listenerTable[context] as IDictionary<string, List<EventReference>>;
        if (!dictionary.ContainsKey(eventName))
        {
            if (!createIfNotExist)
            {
                return null;
            }

            dictionary[eventName] = new List<EventReference>();
        }

        return dictionary[eventName];
    }

    private void _GCWeakReference()
    {
        IDictionary<string, List<EventReference>> dictionary;
        List<EventReference> list;
        List<EventReference> gcList = new List<EventReference>();
        List<string> list1 = new List<string>();
        List<object> list2 = new List<object>();

        foreach (DictionaryEntry entry in this._listenerTable)
        {
            dictionary = entry.Value as IDictionary<string, List<EventReference>>;

            foreach (KeyValuePair<string, List<EventReference>> pair in dictionary)
            {
                list = pair.Value;

                for (int j = list.Count - 1; j >= 0; j--)
                {
                    if (list[j].Listener == null)
                        gcList.Add(list[j]);
                }

                for (int j = gcList.Count - 1; j >= 0; j--)
                {
                    if (list.Contains(gcList[j]))
                        list.Remove(gcList[j]);
                }

                gcList.Clear();

                if (list.Count == 0)
                {
                    list1.Add(pair.Key);
                }
            }

            for (int i = list1.Count - 1; i >= 0; i--)
            {
                if (dictionary.ContainsKey(list1[i]))
                    dictionary.Remove(list1[i]);
            }

            list1.Clear();

            if (dictionary.Count == 0)
            {
                list2.Add(entry.Key);
            }
        }

        for (int i = list2.Count - 1; i >= 0; i--)
        {
            if (this._listenerTable.ContainsKey(list2[i]))
                this._listenerTable.Remove(list2[i]);
        }

        list2.Clear();
    }


}

