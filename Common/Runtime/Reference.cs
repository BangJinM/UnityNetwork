using System;
using UnityEngine;

namespace US
{
    [SerializeField]
    public class Reference
    {
        /// <summary>
        /// 引用次数
        /// </summary>
        public int refCount = 0;

        /// <summary>
        /// 初始化
        /// </summary>
        protected Reference() { refCount = 0; }

        /// <summary>
        /// 未使用
        /// </summary>
        /// <returns></returns>
        public bool IsUnused() { return refCount <= 0; }

        /// <summary>
        /// 引用次数+1
        /// </summary>
        public void Retain() { refCount++; }

        /// <summary>
        /// 引用次数-1
        /// </summary>
        public void Release() { refCount--; }

        /// <summary>
        /// 引用次数归零
        /// </summary>
        public void ReleaseAll() { refCount = 0; }

        /// <summary>
        /// 获取引用次数
        /// </summary>
        /// <returns></returns>
        public int GetRefCount() { return refCount; }
    }
}
