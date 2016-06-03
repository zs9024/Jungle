using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Event
{
    /// <summary>
    /// 事件处理类。
    /// </summary>
    public class EventController
    {
        private Dictionary<string, Delegate> m_theRouter = new Dictionary<string, Delegate>();

        public Dictionary<string, Delegate> TheRouter
        {
            get { return m_theRouter; }
        }

        /// <summary>
        /// 永久注册的事件列表
        /// </summary>
        private List<string> m_permanentEvents = new List<string>();

        /// <summary>
        /// 标记为永久注册事件
        /// </summary>
        /// <param name="eventType"></param>
        public void MarkAsPermanent(string eventType)
        {
            m_permanentEvents.Add(eventType);
        }

        /// <summary>
        /// 判断是否已经包含事件
        /// </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        public bool ContainsEvent(string eventType)
        {
            return m_theRouter.ContainsKey(eventType);
        }

        /// <summary>
        /// 清除非永久性注册的事件
        /// </summary>
        public void Cleanup()
        {
            List<string> eventToRemove = new List<string>();

            foreach (KeyValuePair<string, Delegate> pair in m_theRouter)
            {
                bool wasFound = false;
                foreach (string Event in m_permanentEvents)
                {
                    if (pair.Key == Event)
                    {
                        wasFound = true;
                        break;
                    }
                }

                if (!wasFound)
                    eventToRemove.Add(pair.Key);
            }

            foreach (string Event in eventToRemove)
            {
                m_theRouter.Remove(Event);
            }
        }

        /// <summary>
        /// 处理增加监听器前的事项， 检查 参数等
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="listenerBeingAdded"></param>
        private bool OnListenerAdding(string eventType, Delegate listenerBeingAdded)
        {
            if (!m_theRouter.ContainsKey(eventType))
            {
                m_theRouter.Add(eventType, null);
            }

            Delegate d = m_theRouter[eventType];
            if (d != null && d.GetType() != listenerBeingAdded.GetType())
            {
                Debug.LogError("d != null" + (d != null));
                if (d != null) Debug.LogError("d.GetType() != listenerBeingAdded.GetType()" + (d.GetType() != listenerBeingAdded.GetType()));
                throw new EventException(string.Format(
                       "Try to add not correct event {0}. Current type is {1}, adding type is {2}.",
                       eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
            }
            if (d == null)
            {
                return true;
            }
            int index = Array.IndexOf<Delegate>(d.GetInvocationList(), listenerBeingAdded);
            if (index != -1)
            {
                Debug.LogWarning(string.Format("repeatedly adding the same event listener,event name is {0}, listener is {1}", eventType, listenerBeingAdded.GetType().Name));
            }
            return index == -1;
        }

        /// <summary>
        /// 移除监听器之前的检查
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="listenerBeingRemoved"></param>
        private bool OnListenerRemoving(string eventType, Delegate listenerBeingRemoved)
        {
            if (!m_theRouter.ContainsKey(eventType))
            {
                return false;
            }

            Delegate d = m_theRouter[eventType];
            if ((d != null) && (d.GetType() != listenerBeingRemoved.GetType()))
            {
                throw new EventException(string.Format(
                    "Remove listener {0}\" failed, Current type is {1}, adding type is {2}.",
                    eventType, d.GetType(), listenerBeingRemoved.GetType()));
            }
            else
                return true;
        }

        /// <summary>
        /// 移除监听器之后的处理。删掉事件
        /// </summary>
        /// <param name="eventType"></param>
        private void OnListenerRemoved(string eventType)
        {
            if (m_theRouter.ContainsKey(eventType) && m_theRouter[eventType] == null)
            {
                m_theRouter.Remove(eventType);
            }
        }

        #region 增加监听器
        /// <summary>
        ///  增加监听器， 不带参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener(string eventType, Action handler)
        {
            if (OnListenerAdding(eventType, handler))
            {
                m_theRouter[eventType] = (Action)m_theRouter[eventType] + handler;
            }
        }

        /// <summary>
        ///  增加监听器， 1个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T>(string eventType, Action<T> handler)
        {
            if (OnListenerAdding(eventType, handler))
            {
                m_theRouter[eventType] = (Action<T>)m_theRouter[eventType] + handler;
            }
        }

        /// <summary>
        ///  增加监听器， 2个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T, U>(string eventType, Action<T, U> handler)
        {
            if (OnListenerAdding(eventType, handler))
            {
                m_theRouter[eventType] = (Action<T, U>)m_theRouter[eventType] + handler;
            }
        }

        /// <summary>
        ///  增加监听器， 3个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T, U, V>(string eventType, Action<T, U, V> handler)
        {
            if (OnListenerAdding(eventType, handler))
            {
                m_theRouter[eventType] = (Action<T, U, V>)m_theRouter[eventType] + handler;
            }
        }

        /// <summary>
        ///  增加监听器， 4个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T, U, V, W>(string eventType, Action<T, U, V, W> handler)
        {
            if (OnListenerAdding(eventType, handler))
            {
                m_theRouter[eventType] = (Action<T, U, V, W>)m_theRouter[eventType] + handler;
            }
        }
        #endregion

        #region 移除监听器
        /// <summary>
        ///  移除监听器， 不带参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener(string eventType, Action handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_theRouter[eventType] = (Action)m_theRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }

        /// <summary>
        ///  移除监听器， 1个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T>(string eventType, Action<T> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_theRouter[eventType] = (Action<T>)m_theRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }

        /// <summary>
        ///  移除监听器， 2个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T, U>(string eventType, Action<T, U> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_theRouter[eventType] = (Action<T, U>)m_theRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }

        /// <summary>
        ///  移除监听器， 3个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T, U, V>(string eventType, Action<T, U, V> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_theRouter[eventType] = (Action<T, U, V>)m_theRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }

        /// <summary>
        ///  移除监听器， 4个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T, U, V, W>(string eventType, Action<T, U, V, W> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_theRouter[eventType] = (Action<T, U, V, W>)m_theRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }
        #endregion

        #region 触发事件
        /// <summary>
        ///  触发事件， 不带参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void TriggerEvent(string eventType)
        {
            Delegate d;
            if (!m_theRouter.TryGetValue(eventType, out d))
            {
                return;
            }

            var callbacks = d.GetInvocationList();
            for (int i = 0; i < callbacks.Length; i++)
            {
                Action callback = callbacks[i] as Action;

                if (callback == null)
                {
                    throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
                }

                try
                {

                    callback();
                }
                catch (Exception ex)
                {
                    //LoggerHelper.Except(ex);
                    Debug.LogError(ex);
                }
            }
        }

        /// <summary>
        ///  触发事件， 带1个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void TriggerEvent<T>(string eventType, T arg1)
        {
            Delegate d;
            if (!m_theRouter.TryGetValue(eventType, out d))
            {
                return;
            }

            var callbacks = d.GetInvocationList();
            for (int i = 0; i < callbacks.Length; i++)
            {
                Action<T> callback = callbacks[i] as Action<T>;

                if (callback == null)
                {
                    throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
                }

                try
                {
                    callback(arg1);
                }
                catch (Exception ex)
                {
                    //LoggerHelper.Except(ex);
                    Debug.LogError(ex);
                }
            }
        }

        /// <summary>
        ///  触发事件， 带2个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void TriggerEvent<T, U>(string eventType, T arg1, U arg2)
        {
            Delegate d;
            if (!m_theRouter.TryGetValue(eventType, out d))
            {
                return;
            }
            var callbacks = d.GetInvocationList();
            for (int i = 0; i < callbacks.Length; i++)
            {
                Action<T, U> callback = callbacks[i] as Action<T, U>;

                if (callback == null)
                {
                    throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
                }

                try
                {
                    callback(arg1, arg2);
                }
                catch (Exception ex)
                {
                    //LoggerHelper.Except(ex);
                    Debug.LogError(ex);
                }
            }
        }

        /// <summary>
        ///  触发事件， 带3个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void TriggerEvent<T, U, V>(string eventType, T arg1, U arg2, V arg3)
        {
            Delegate d;
            if (!m_theRouter.TryGetValue(eventType, out d))
            {
                return;
            }
            var callbacks = d.GetInvocationList();
            for (int i = 0; i < callbacks.Length; i++)
            {
                Action<T, U, V> callback = callbacks[i] as Action<T, U, V>;

                if (callback == null)
                {
                    throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
                }
                try
                {
                    callback(arg1, arg2, arg3);
                }
                catch (Exception ex)
                {
                    //LoggerHelper.Except(ex);
                    Debug.LogError(ex);
                }
            }
        }

        /// <summary>
        ///  触发事件， 带4个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void TriggerEvent<T, U, V, W>(string eventType, T arg1, U arg2, V arg3, W arg4)
        {
            Delegate d;
            if (!m_theRouter.TryGetValue(eventType, out d))
            {
                return;
            }
            var callbacks = d.GetInvocationList();
            for (int i = 0; i < callbacks.Length; i++)
            {
                Action<T, U, V, W> callback = callbacks[i] as Action<T, U, V, W>;

                if (callback == null)
                {
                    throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
                }
                try
                {
                    callback(arg1, arg2, arg3, arg4);
                }
                catch (Exception ex)
                {
                    //LoggerHelper.Except(ex);
                    Debug.LogError(ex);
                }
            }
        }

        #endregion
    }
}
