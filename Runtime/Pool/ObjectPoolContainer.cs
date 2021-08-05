using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace US
{
    /// <summary>
    /// 供对象池使用的容器（Container）：容器内对象的内容、标记容器内对象为已被（未被）使用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPoolContainer<T>
    {
        /// <summary>
        /// 容器内对象
        /// </summary>
        public T Item { get; set; }
        /// <summary>
        /// 容器内对象是否已被使用的标记
        /// </summary>
        public bool Used { get; private set; }

        /// <summary>
        /// 标记为已被使用
        /// </summary>
        public void Consume()
        {
            Used = true;
        }

        /// <summary>
        /// 标记为未被使用
        /// </summary>
        public void Release()
        {
            Used = false;
        }
    }
}
