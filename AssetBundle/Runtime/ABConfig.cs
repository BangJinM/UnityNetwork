
using System.IO;
using UnityEngine;

namespace US
{
    class ABConfig
    {
        public static string PlatformBuildPath
        {
            get
            {
                var dir = Utility.buildPath + $"/{Utility.GetPlatformName()}";
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                return dir;
            }
        }
    }
}
