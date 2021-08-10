using UnityEngine;

namespace US
{

    public delegate void LoadedCallback(IResourceLoader loader);

    public enum LoadType
    {
        DEFAULT = -1,       // 
        STREAM,             // 流
        ASSET,              // Asset目录下的资源
        ASSETBUNDLE,        // AssetBundle
        RESOURCES,          // Resource 资源目录
    }

    public enum LoadState
    {
        ERROR = -1,         // 
        LOADING,            // 加载中
        FINISHED,           // 完成
    }

    public class IResourceLoader
    {
        protected Object mObject;
        protected string mPath = "";
        protected LoadedCallback mFinishCallback = null;
        protected LoadType loadType = LoadType.DEFAULT;
        protected LoadState loadState = LoadState.ERROR;

        public IResourceLoader(string path, LoadedCallback loadedCallback = null, LoadType loadType = LoadType.DEFAULT)
        {
            this.mPath = path;
            this.loadType = loadType;
            loadState = LoadState.LOADING;
            mFinishCallback = loadedCallback;
        }
        /// <summary>
        /// 加载完成
        /// </summary>
        public virtual void Finish()
        {
            mFinishCallback?.Invoke(this);
            loadState = LoadState.FINISHED;
        }
        /// <summary>
        /// 加载
        /// </summary>
        public virtual void Load()
        {
            loadState = LoadState.LOADING;
        }
        /// <summary>
        /// 异步加载
        /// </summary>
        public virtual void LoadAsyc()
        {
            loadState = LoadState.LOADING;
        }
        /// <summary>
        /// 结果
        /// </summary>
        /// <returns>结果</returns>
        public virtual Object GetResult() { return mObject; }
    }
}
