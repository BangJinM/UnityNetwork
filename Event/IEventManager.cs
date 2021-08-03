using System;
using System.Collections.Generic;
using UnityEngine;


namespace UCS.Event
{
    public interface IRegistrations
    {

    }
    public class Registrations<T> : IRegistrations
    {
        /// <summary>
        /// 因为委托本身就可以一对多注册
        /// </summary>
        public Action<T> OnEvent = obj => { };
    }
    public interface IEventManager
    {
        /// <summary>
        /// 发送事件
        /// </summary>
        void Send<T>() where T : new();
        void Send<T>(T e);
        /// <summary>
        /// 注册事件
        /// </summary>
        void Register<T>(Action<T> onEvent);
        /// <summary>
        /// 注销事件
        /// </summary>
        void UnRegister<T>(Action<T> onEvent);
    }
}
