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
        public static string MANIFEST_JSON_PATH = "Assets/CustomAssets/file.json";

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
        public static string RuledAssetBundleName(string name)
        {
            return name.Replace("\\", "/").ToLower() + ".unity3d";
        }

        public static MarkRules GetMarkRulesAssetData()
        {
            return GetScriptableObjectAsset<MarkRules>(Settings.MARK_RULES_PATH);
        }

        public static Manifest GetManifestAssetData()
        {
            return GetScriptableObjectAsset<Manifest>(Settings.MANIFEST_PATH);
        }

        public static T GetScriptableObjectAsset<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
            }

            return asset;
        }
    }
}
