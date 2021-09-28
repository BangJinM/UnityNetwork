using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US
{
    public class PoolManager : MonoSingleton<PoolManager>
    {
        private Dictionary<Type, IObjectPool> pools;

        PoolManager()
        {
            pools = new Dictionary<Type, IObjectPool>();
        }

        IObjectPool GetOrCreateObjectPool<T>() where T : new()
        {
            Type type = typeof(T);
            IObjectPool objectPool;
            if (pools.TryGetValue(type, out objectPool))
            {
                return objectPool;
            }
            objectPool = new ObjectPool<T>(() => new T(), 10);
            return objectPool;
        }

        public ObjectPoolContainer<T> GetPoolContainer<T>() where T : new()
        {
            ObjectPool<T> objectPool = (ObjectPool<T>)GetOrCreateObjectPool<T>();
            return objectPool.GetItem();
        }

        public void Update()
        {
            foreach (var poolInfo in pools)
            {
                poolInfo.Value.Update();
            }
        }

    }
}
