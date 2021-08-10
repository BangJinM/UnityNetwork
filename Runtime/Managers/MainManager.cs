using System;
using System.Collections.Generic;
using UnityEngine;

namespace US
{
    public class MainManager : MonoSingleton<MainManager>
    {
        private void Awake()
        {
            var network = NetworkManager.Instance;
            var pool = PoolManager.Instance;
            var eventManager = EventManager.Instance;
        }
    }
}
