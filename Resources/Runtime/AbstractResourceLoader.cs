using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace US
{
    /// <summary>
    /// 加载模式，同步或异步
    /// </summary>
    public enum LoaderMode
    {
        Async,
        Sync,
    }

    public delegate void LoaderActionCallBack(bool isOk, object resultObject);

    /// <summary>
    /// 资源加载基类
    /// </summary>
    public class AbstractResourceLoader : Reference, IAsyncLoader
    {
        public object AsyncResult { get; set; }
        public AsyncStates AsyncState { get; set; }
        public bool AsyncError { get; set; }
        public bool IsReadyDestory { get; set; }
        
        /// <summary>
        /// 类型
        /// </summary>
        public Type RLType { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public string Url { get; private set; }
        /// <summary>
        /// 百分比
        /// </summary>
        public virtual float Progress { get; protected set; }

        /// <summary>
        /// 成功回调
        /// </summary>
        private readonly List<LoaderActionCallBack> afterFinishedCallbacks = new List<LoaderActionCallBack>();

        /// <summary>
        /// 新建Loader
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        protected static T NewLoader<T>(string url) where T : AbstractResourceLoader, new()
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }

            AbstractResourceLoader loader = ResourceManager.Instance.GetResourceLoader<T>(url);
            if (loader != null)
                return loader as T;
            loader = new T();
            loader.Retain();
            loader.RLType = typeof(T);
            loader.Init();
            ResourceManager.Instance.AddResourceLoader(loader);
            return loader as T;
        }

        protected AbstractResourceLoader() : base()
        {
            AsyncResult = null;
            AsyncState = AsyncStates.INVALID;
            AsyncError = false;
            IsReadyDestory = false;

            Url = "";
            Progress = 0;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected virtual void Init()
        {
            AsyncState = AsyncStates.INITED;
        }

        /// <summary>
        /// load结束
        /// </summary>
        /// <param name="resultObj"></param>
        protected virtual void OnFinish(object resultObj)
        {
            AsyncResult = resultObj;
            AsyncError = AsyncResult == null;
            AsyncState = AsyncStates.FINISHED;

            Progress = 1;

            foreach (var callback in afterFinishedCallbacks)
            {
                callback(AsyncState == AsyncStates.FINISHED && !AsyncError, AsyncResult);
            }
            afterFinishedCallbacks.Clear();
        }

        /// <summary>
        /// 减少引用次数
        /// </summary>
        /// <param name="gcNow"></param>
        public virtual void Release(bool gcNow)
        {
            Release();
            if (gcNow) 
                ResourceManager.Instance.GarbageCollect();
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public virtual void Dispose() { }

        /// <summary>
        /// 在Finisehd后会执行的回调
        /// </summary>
        /// <param name="callback"></param>
        public void AddCallback(LoaderActionCallBack callback)
        {
            if (callback != null && !IsReadyDestory)
            {
                if (AsyncState == AsyncStates.FINISHED)
                    callback(!AsyncError, AsyncResult);
                else
                    afterFinishedCallbacks.Add(callback);
            }
        }
    }
}