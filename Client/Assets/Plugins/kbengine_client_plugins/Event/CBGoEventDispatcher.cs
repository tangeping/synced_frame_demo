using CBFrame.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CBFrame.Sys
{
    public class CBGoEventDispatcher : MonoBehaviour
    {
        private CBEventDispatcher _eventDispatcher = new CBEventDispatcher();

        /// <summary>
        /// 清除注册的事件
        /// </summary>
        public void Cleanup()
        {
            _eventDispatcher.Cleanup();
        }

        #region 增加监听器
        /// <summary>
        ///  增加监听器， 不带参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener(int eventType, Action handler)
        {
            _eventDispatcher.AddEventListener(eventType, handler);
        }

        /// <summary>
        ///  增加监听器， 1个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T>(int eventType, Action<T> handler)
        {
            _eventDispatcher.AddEventListener(eventType, handler);
        }

        /// <summary>
        ///  增加监听器， 2个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T, U>(int eventType, Action<T, U> handler)
        {
            _eventDispatcher.AddEventListener(eventType, handler);
        }

        /// <summary>
        ///  增加监听器， 3个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T, U, V>(int eventType, Action<T, U, V> handler)
        {
            _eventDispatcher.AddEventListener(eventType, handler);
        }

        /// <summary>
        ///  增加监听器， 4个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T, U, V, W>(int eventType, Action<T, U, V, W> handler)
        {
            _eventDispatcher.AddEventListener(eventType, handler);
        }
        #endregion

        #region 移除监听器

        public void RemoveEventListener(object listener)
        {
            _eventDispatcher.RemoveEventListener(listener);
        }

        /// <summary>
        ///  移除监听器， 不带参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener(int eventType, Action handler)
        {
            _eventDispatcher.RemoveEventListener(eventType, handler);
        }

        /// <summary>
        ///  移除监听器， 1个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T>(int eventType, Action<T> handler)
        {
            _eventDispatcher.RemoveEventListener(eventType, handler);
        }

        /// <summary>
        ///  移除监听器， 2个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T, U>(int eventType, Action<T, U> handler)
        {
            _eventDispatcher.RemoveEventListener(eventType, handler);
        }

        /// <summary>
        ///  移除监听器， 3个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T, U, V>(int eventType, Action<T, U, V> handler)
        {
            _eventDispatcher.RemoveEventListener(eventType, handler);
        }

        /// <summary>
        ///  移除监听器， 4个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T, U, V, W>(int eventType, Action<T, U, V, W> handler)
        {
            _eventDispatcher.RemoveEventListener(eventType, handler);
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
            _eventDispatcher.TriggerEvent(eventType);
        }

        /// <summary>
        ///  触发事件， 带1个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void TriggerEvent<T>(int eventType, T arg1)
        {
            _eventDispatcher.TriggerEvent(eventType, arg1);
        }

        /// <summary>
        ///  触发事件， 带2个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void TriggerEvent<T, U>(int eventType, T arg1, U arg2)
        {
            _eventDispatcher.TriggerEvent(eventType, arg1, arg2);
        }

        /// <summary>
        ///  触发事件， 带3个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void TriggerEvent<T, U, V>(int eventType, T arg1, U arg2, V arg3)
        {
            _eventDispatcher.TriggerEvent(eventType, arg1, arg2, arg3);
        }

        /// <summary>
        ///  触发事件， 带4个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void TriggerEvent<T, U, V, W>(int eventType, T arg1, U arg2, V arg3, W arg4)
        {
            _eventDispatcher.TriggerEvent(eventType, arg1, arg4);
        }

        #endregion
    }
}