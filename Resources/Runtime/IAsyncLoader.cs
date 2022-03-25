namespace US
{
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

    /// <summary>
    /// 用于协程，线程，结果调度类
    /// </summary>
    public interface IAsyncLoader
    {
        /// <summary>
        /// 最终加载结果的资源
        /// </summary>
        object AsyncResult { get; set; }

        /// <summary>
        /// 是否已经完成
        /// </summary>
        AsyncStates AsyncState { get; set; }

        /// <summary>
        /// 标记是否销毁
        /// </summary>
        bool IsReadyDestory { get; set; }

        /// <summary>
        /// 标记是否报错 
        /// </summary>
        bool AsyncError { get; set; }
    }
}