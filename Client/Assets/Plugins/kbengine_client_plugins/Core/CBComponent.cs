using CBFrame.Sys;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CBFrame.Core
{
    [RequireComponent(typeof(CBGoEventDispatcher))]

    public class CBComponent : MonoBehaviour
    {
        private CBGoEventDispatcher _dispatcher;

        private void Awake()
        {
            _dispatcher = GetComponent<CBGoEventDispatcher>();
            Debug.Log(_dispatcher);
           
        }

        /// <summary>
        ///  增加监听器， 不带参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener(int eventType, Action handler)
        {
            _dispatcher.AddEventListener(eventType, handler);
        }

        public void AddEventListener<T>(int eventType, Action<T> handler)
        {
            _dispatcher.AddEventListener(eventType, handler);
        }

        public void RemoveEventListener(int eventType, Action handler)
        {
            _dispatcher.RemoveEventListener(eventType, handler);
        }

        public void RemoveEventListener<T>(int eventType, Action<T> handler)
        {
            _dispatcher.RemoveEventListener(eventType, handler);
        }

        public void TriggerEvent(int eventType)
        {
            _dispatcher.TriggerEvent(eventType);
        }

        public void TriggerEvent<T>(int eventType, T arg1)
        {
            _dispatcher.TriggerEvent(eventType, arg1);
        }

        /// <summary>
        ///  触发事件， 带2个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        public void TriggerEvent<T, U>(int eventType, T arg1, U arg2)
        {
            _dispatcher.TriggerEvent(eventType, arg1, arg2);
        }

        /// <summary>
        ///  触发事件， 带3个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        public void TriggerEvent<T, U, V>(int eventType, T arg1, U arg2, V arg3)
        {
            _dispatcher.TriggerEvent(eventType, arg1, arg2, arg3);
        }

        /// <summary>
        ///  触发事件， 带4个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        public void TriggerEvent<T, U, V, W>(int eventType, T arg1, U arg2, V arg3, W arg4)
        {
            _dispatcher.TriggerEvent(eventType, arg1, arg2, arg3, arg4);
        }

        private void OnDestroy()
        {
            _dispatcher.RemoveEventListener(this);
        }

    }

    public enum EVENT_ID
    {
        EVENT_COM,
        EVENT_UI,
        EVENT_WORLD,
        EVENT_PLAYER,
        EVENT_CREAT_PLAYER,
        EVENT_ENTER_WORLD,
        EVENT_LEAVE_WORLD,
        EVENT_CAMERA_FOLLOW,
        EVENT_PLAYER_HEALTH,
        EVENT_FRAME_UPDATE,
        EVENT_FRAME_TICK,
        EVENT_FRAME_CMD,
    };
}