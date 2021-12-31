using System;
using System.Collections.Generic;
using UnityEngine;

namespace US
{
    public interface IObjectPool
    {
        void Update();
    }
    /// <summary>
    /// 一般对象的池子
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPool<T> : IObjectPool
    {
        // 池中所有对象
        private List<ObjectPoolContainer<T>> list;
        private Func<T> factoryFunc;
        private int lastIndex;
        private int initialSize = 10;

        public int Count { get { return list.Count; } }

        public ObjectPool(Func<T> factoryFunc, int initialSize)
        {
            // 该委托用于返回一个（新创建的）对象实例
            this.factoryFunc = factoryFunc;
            this.initialSize = initialSize;

            list = new List<ObjectPoolContainer<T>>(initialSize);
        }

        private ObjectPoolContainer<T> CreateContainer()
        {
            var container = new ObjectPoolContainer<T>();
            container.Item = factoryFunc();
            list.Add(container);
            return container;
        }

        public ObjectPoolContainer<T> GetItem()
        {
            ObjectPoolContainer<T> container = null;

            // 若在list中找得到还未被使用的对象，则使用该对象作为返回值
            for (int i = 0; i < list.Count; i++)
            {
                lastIndex++;
                // 最可能没使用的就是上次使用了的下一个。若上次使用的是最后1个，则这次只要从第0个开始查
                if (lastIndex > list.Count - 1)
                    lastIndex = 0;

                if (list[lastIndex].Used)
                    continue;
                else
                {
                    container = list[lastIndex];
                    break;
                }
            }

            // list中所有对象都被使用了，新创建一个对象
            if (container == null)
                container = CreateContainer();

            // 标记为已使用
            container.Consume();

            return container;
        }

        public void Update()
        {
            int unUsed = 0;
            foreach (var poolInfo in list)
            {
                if (!poolInfo.Used) unUsed++;
            }

            if (list.Count > initialSize && unUsed > (list.Count - initialSize) && unUsed > initialSize / 2)
            {
                int count = 1;
                for (int index = 0; index < count; index++)
                {
                    var item = GetItem();
                    list.Remove(item);
                }
            }
        }
    }
}
