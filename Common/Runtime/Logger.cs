using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace US
{
    public static class Logger
    {
        public static bool Loggable = true;

        public static void Error(string format, params object[] args)
        {
            Debug.LogErrorFormat(format, args);
        }

        public static void Warning(string format, params object[] args)
        {
            Debug.LogWarningFormat(format, args);
        }

        [Conditional("DEBUG")]
        public static void Info(string format, params object[] args)
        {
            if (!Loggable) return;

            Debug.LogFormat(format, args);
        }
    }
}