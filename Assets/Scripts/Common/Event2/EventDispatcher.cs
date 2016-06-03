
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Event
{
    /// <summary>
    /// 事件分发函数。
    /// 提供事件注册， 反注册， 事件触发
    /// 采用 delegate, dictionary 实现
    /// 支持自定义事件。 事件采用字符串方式标识
    /// 支持 0，1，2，3 等4种不同参数个数的回调函数
    /// </summary>
    public class EventDispatcher
    {
        private static EventController m_eventController = new EventController();

        public static Dictionary<string, Delegate> TheRouter
        {
            get { return m_eventController.TheRouter; }
        }

        /// <summary>
        /// 标记为永久注册事件
        /// </summary>
        /// <param name="eventType"></param>
        static public void MarkAsPermanent(string eventType)
        {
            m_eventController.MarkAsPermanent(eventType);
        }

        /// <summary>
        /// 清除非永久性注册的事件
        /// </summary>
        static public void Cleanup()
        {
            m_eventController.Cleanup();
        }

        #region 增加监听器
        /// <summary>
        ///  增加监听器， 不带参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        static public void AddEventListener(string eventType, Action handler)
        {
            m_eventController.AddEventListener(eventType, handler);
        }

        /// <summary>
        ///  增加监听器， 1个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        static public void AddEventListener<T>(string eventType, Action<T> handler)
        {
            m_eventController.AddEventListener(eventType, handler);
        }

        /// <summary>
        ///  增加监听器， 2个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        static public void AddEventListener<T, U>(string eventType, Action<T, U> handler)
        {
            m_eventController.AddEventListener(eventType, handler);
        }

        /// <summary>
        ///  增加监听器， 3个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        static public void AddEventListener<T, U, V>(string eventType, Action<T, U, V> handler)
        {
            m_eventController.AddEventListener(eventType, handler);
        }

        /// <summary>
        ///  增加监听器， 4个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        static public void AddEventListener<T, U, V, W>(string eventType, Action<T, U, V, W> handler)
        {
            m_eventController.AddEventListener(eventType, handler);
        }
        #endregion

        #region 移除监听器
        /// <summary>
        ///  移除监听器， 不带参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        static public void RemoveEventListener(string eventType, Action handler)
        {
            m_eventController.RemoveEventListener(eventType, handler);
        }

        /// <summary>
        ///  移除监听器， 1个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        static public void RemoveEventListener<T>(string eventType, Action<T> handler)
        {
            m_eventController.RemoveEventListener(eventType, handler);
        }

        /// <summary>
        ///  移除监听器， 2个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        static public void RemoveEventListener<T, U>(string eventType, Action<T, U> handler)
        {
            m_eventController.RemoveEventListener(eventType, handler);
        }

        /// <summary>
        ///  移除监听器， 3个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        static public void RemoveEventListener<T, U, V>(string eventType, Action<T, U, V> handler)
        {
            m_eventController.RemoveEventListener(eventType, handler);
        }

        /// <summary>
        ///  移除监听器， 4个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        static public void RemoveEventListener<T, U, V, W>(string eventType, Action<T, U, V, W> handler)
        {
            m_eventController.RemoveEventListener(eventType, handler);
        }
        #endregion

        #region 触发事件
        /// <summary>
        ///  触发事件， 不带参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        static public void TriggerEvent(string eventType)
        {
            m_eventController.TriggerEvent(eventType);
        }

        /// <summary>
        ///  触发事件， 带1个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        static public void TriggerEvent<T>(string eventType, T arg1)
        {
            m_eventController.TriggerEvent(eventType, arg1);
        }

        /// <summary>
        ///  触发事件， 带2个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        static public void TriggerEvent<T, U>(string eventType, T arg1, U arg2)
        {
            m_eventController.TriggerEvent(eventType, arg1, arg2);
        }

        /// <summary>
        ///  触发事件， 带3个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        static public void TriggerEvent<T, U, V>(string eventType, T arg1, U arg2, V arg3)
        {
            m_eventController.TriggerEvent(eventType, arg1, arg2, arg3);
        }

        /// <summary>
        ///  触发事件， 带4个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        static public void TriggerEvent<T, U, V, W>(string eventType, T arg1, U arg2, V arg3, W arg4)
        {
            m_eventController.TriggerEvent(eventType, arg1, arg2, arg3, arg4);
        }

        #endregion
    }
}