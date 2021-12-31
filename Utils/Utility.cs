using System.IO;
using UnityEngine;

namespace US
{
    public static class Utility
    {
        public const string buildPath = "Bundles";
        public static string GetPlatformName()
        {
            if (Application.platform == RuntimePlatform.Android) return "Android";

            if (Application.platform == RuntimePlatform.WindowsPlayer) return "Windows";

            if (Application.platform == RuntimePlatform.IPhonePlayer) return "iOS";

            return Application.platform == RuntimePlatform.WebGLPlayer ? "WebGL": "Unsupported";
        }
    }
}
