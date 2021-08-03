using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace US.AB
{
    public static class MenuItems
    {
        public static string buildRulesPath = "Assets/Rules.asset";
        public static string manifestPath = "Assets/Manifest.asset";

        [MenuItem("USC/Test/CreateRules", false, 1)]
        public static void TestCreateRules()
        {
            GetBuildRules();
        }

        [MenuItem("USC/Test/CreateManifest", false, 1)]
        public static void TestManifest()
        {
            GetManifest();
        }

        internal static BuildRules GetBuildRules()
        {
            return GetAsset<BuildRules>(buildRulesPath);
        }

        internal static Manifest GetManifest()
        {
            return GetAsset<Manifest>(manifestPath);
        }

        private static T GetAsset<T>(string path) where T : ScriptableObject
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
