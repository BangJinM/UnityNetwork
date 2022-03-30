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
        /// <summary>
        /// 异步
        /// </summary>
        Async,
        /// <summary>
        /// 同步
        /// </summary>
        Sync,
    }

    /// <summary>
    /// 协程的状态
    /// </summary>
    public enum AsyncStates
    {
        /// <summary>
        ///  无效的
        /// </summary>
        INVALID,
        /// <summary>
        /// 初始化
        /// </summary>
        INITED,
        /// <summary>
        /// 完成
        /// </summary>
        FINISHED
    }

    public delegate void LoaderActionCallBack(bool isOk, object resultObject);

    /// <summary>
    /// 资源加载基类
    /// </summary>
    [Serializable]
    public class AbstractResourceLoader : Reference
    {
        /// <summary>
        /// 最终加载结果的资源
        /// </summary>
        public object AsyncResult = null;

        /// <summary>
        /// 是否已经完成
        /// </summary>
        public AsyncStates AsyncState = AsyncStates.INVALID;

        /// <summary>
        /// 标记是否销毁
        /// </summary>
        public bool IsReadyDestory = false;

        /// <summary>
        /// 标记是否报错 
        /// </summary>
        public bool AsyncError = false;

        /// <summary>
        /// 类型
        /// </summary>
        public Type RLType = null;
        /// <summary>
        /// 路径
        /// </summary>
        public string Url = "";
        /// <summary>
        /// 百分比
        /// </summary>
        public float Progress = 0.0f;

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
        protected static T NewLoader<T>(string url, LoaderActionCallBack callback = null) where T : AbstractResourceLoader, new()
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
            loader.Url = url;
            if (callback != null)
                loader.afterFinishedCallbacks.Add(callback);
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
        protected virtual void Finish(object resultObj)
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
        /// <param name="gcNow"> 是否立即GC </param>
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