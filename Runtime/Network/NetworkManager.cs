using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US
{
    class NetworkManager : MonoSingleton<NetworkManager>
    {
        public NetService netService;

        public static string ip = "127.0.0.1";
        public static int port = 23456;

        void Awake()
        {
            netService = NetService.GetInstance();
            netService.Start("127.0.0.1", 23456);
        }

        void Update()
        {
            netService.Tick();
        }

        void Destroy()
        {
            netService.Close();
        }
    }
}
