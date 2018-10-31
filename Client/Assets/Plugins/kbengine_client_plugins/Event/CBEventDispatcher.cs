
using CBFrame.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBFrame.Sys
{
    public enum TriggerType
    {
        TT_GLOBAL,
        TT_GAMEOBJECT,
        TT_ROUTING
    }

    /// <summary>
    /// 事件分发器。
    /// </summary>
    public class CBEventDispatcher
    {
        private Dictionary<int, List<Delegate>> _theRouter = new Dictionary<int, List<Delegate>>();

        //public Dictionary<int, Delegate> TheRouter
        //{
        //    get { return _theRouter; }
        //}

        /// <summary>
        /// 判断是否已经包含事件
        /// </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        public bool ContainsEvent(int eventType)
        {
            return _theRouter.ContainsKey(eventType);
        }

        /// <summary>
        /// 清除注册的事件
        /// </summary>
        public void Cleanup()
        {
            _theRouter.Clear();
        }

        /// <summary>
        /// 处理增加监听器前的事项， 检查 参数等
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="listenerBeingAdded"></param>
        private void OnListenerAdding(int eventType, Delegate listenerBeingAdded)
        {
            if (!_theRouter.ContainsKey(eventType))
            {
                _theRouter.Add(eventType, new List<Delegate>());
            }

            List<Delegate> d = _theRouter[eventType];

            if (d.Count > 0 && d[0].GetType() != listenerBeingAdded.GetType())
            {
                throw new EventException(string.Format(
                       "Try to add not correct event {0}. Current type is {1}, adding type is {2}.",
                       eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
            }
        }

        /// <summary>
        /// 移除监听器之前的检查
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="listenerBeingRemoved"></param>
        private bool OnListenerRemoving(int eventType, Delegate listenerBeingRemoved)
        {
            if (!_theRouter.ContainsKey(eventType))
            {
                return false;
            }

            List<Delegate> d = _theRouter[eventType];

            if ((d.Count > 0) && (d[0].GetType() != listenerBeingRemoved.GetType()))
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
        private void OnListenerRemoved(int eventType)
        {
            if (_theRouter.ContainsKey(eventType) && _theRouter[eventType] == null)
            {
                _theRouter.Remove(eventType);
            }
        }

        #region 增加监听器
        /// <summary>
        ///  增加监听器， 不带参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener(int eventType, Action handler)
        {
            OnListenerAdding(eventType, handler);
            _theRouter[eventType].Add(handler);
        }

        /// <summary>
        ///  增加监听器， 1个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T>(int eventType, Action<T> handler)
        {
            OnListenerAdding(eventType, handler);
            _theRouter[eventType].Add(handler);
        }

        /// <summary>
        ///  增加监听器， 2个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T, U>(int eventType, Action<T, U> handler)
        {
            OnListenerAdding(eventType, handler);
            _theRouter[eventType].Add(handler);
        }

        /// <summary>
        ///  增加监听器， 3个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T, U, V>(int eventType, Action<T, U, V> handler)
        {
            OnListenerAdding(eventType, handler);
            _theRouter[eventType].Add(handler);
        }

        /// <summary>
        ///  增加监听器， 4个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T, U, V, W>(int eventType, Action<T, U, V, W> handler)
        {
            OnListenerAdding(eventType, handler);
            _theRouter[eventType].Add(handler);
        }
        #endregion

        #region 移除监听器

        public void RemoveEventListener(object listener)
        {
            var msg = _theRouter.Keys.ToArray();

            for (int i = msg.Length - 1; i >= 0; i--)
            {
                var pending = _theRouter[msg[i]].FindAll(item => { return item.Target == listener; });

                for(int j = 0; j< pending.Count; j++)
                {
                    _theRouter[msg[i]].Remove(pending[j]);
                }
            }
        }

        /// <summary>
        ///  移除监听器， 不带参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener(int eventType, Action handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                _theRouter[eventType].Remove(handler);
                OnListenerRemoved(eventType);
            }
        }

        /// <summary>
        ///  移除监听器， 1个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T>(int eventType, Action<T> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                _theRouter[eventType].Remove(handler);
                OnListenerRemoved(eventType);
            }
        }

        /// <summary>
        ///  移除监听器， 2个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T, U>(int eventType, Action<T, U> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                _theRouter[eventType].Remove(handler);
                OnListenerRemoved(eventType);
            }
        }

        /// <summary>
        ///  移除监听器， 3个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T, U, V>(int eventType, Action<T, U, V> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                _theRouter[eventType].Remove(handler);
                OnListenerRemoved(eventType);
            }
        }

        /// <summary>
        ///  移除监听器， 4个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T, U, V, W>(int eventType, Action<T, U, V, W> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                _theRouter[eventType].Remove(handler);
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
        public void TriggerEvent(int eventType)
        {
            List<Delegate> callbacks;

            if (!_theRouter.TryGetValue(eventType, out callbacks))
            {
                return;
            }

            for (int i = 0; i < callbacks.Count; i++)
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

                }
            }
        }

        /// <summary>
        ///  触发事件， 带1个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void TriggerEvent<T>(int eventType, T arg1)
        {
            List<Delegate> callbacks;

            if (!_theRouter.TryGetValue(eventType, out callbacks))
            {
                return;
            }

            for (int i = 0; i < callbacks.Count; i++)
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

                }
            }
        }

        /// <summary>
        ///  触发事件， 带2个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void TriggerEvent<T, U>(int eventType, T arg1, U arg2)
        {
            List<Delegate> callbacks;

            if (!_theRouter.TryGetValue(eventType, out callbacks))
            {
                return;
            }

            for (int i = 0; i < callbacks.Count; i++)
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

                }
            }
        }

        /// <summary>
        ///  触发事件， 带3个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void TriggerEvent<T, U, V>(int eventType, T arg1, U arg2, V arg3)
        {
            List<Delegate> callbacks;

            if (!_theRouter.TryGetValue(eventType, out callbacks))
            {
                return;
            }

            for (int i = 0; i < callbacks.Count; i++)
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

                }
            }
        }

        /// <summary>
        ///  触发事件， 带4个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void TriggerEvent<T, U, V, W>(int eventType, T arg1, U arg2, V arg3, W arg4)
        {
            List<Delegate> callbacks;

            if (!_theRouter.TryGetValue(eventType, out callbacks))
            {
                return;
            }

            for (int i = 0; i < callbacks.Count; i++)
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

                }
            }
        }

        #endregion
    }

   
}