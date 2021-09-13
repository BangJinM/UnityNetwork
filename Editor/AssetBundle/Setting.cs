using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace US
{

    public sealed class Settings : ScriptableObject
    {
        public static string MANIFEST_PATH = "Assets/Editor/manifest.asset";
        public static string MARK_RULES_PATH = "Assets/Editor/MarkRules.asset";

        public static string m_BunleTargetPath = Application.dataPath + "/../AssetBundle/" + EditorUserBuildSettings.activeBuildTarget.ToString();
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
