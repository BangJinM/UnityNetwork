using System;
using System.Collections.Generic;
using UnityEngine;


namespace US
{
    public interface IRegistrations
    {

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
