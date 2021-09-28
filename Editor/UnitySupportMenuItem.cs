using System.IO;
using UnityEditor;
using UnityEngine;

namespace US
{
    public class UnitySupportMenuItem
    {
        static bool isBuild = false;

        private const string BUILD = "UnitySupport/AssetBundle/构建";
        private const string MARK_ASSET_WITH_DIR = "UnitySupport/AssetBundle/分析热更包";
        private const string BUILD_MANIFEST = "UnitySupport/AssetBundle/构建Manifest";
        private const string BUILD_ASSET_BUILD = "UnitySupport/AssetBundle/构建热更包";
        private const string CLEAR_ASSET_BUILD = "UnitySupport/AssetBundle/清除热更包";


        private const string TEST_ASSET_BUILD = "UnitySupport/AssetBundle/Test/加载AB包";

        [MenuItem(BUILD)]
        public static void Build()
        {
            AnalyseAssetBundle();
            BuildManifest();
            BuildAssetBundle();
        }

        [MenuItem(MARK_ASSET_WITH_DIR)]
        public static void AnalyseAssetBundle()
        {
            AssetBundlePackager.CollectResources();
            isBuild = true;
        }

        [MenuItem(BUILD_MANIFEST)]
        public static void BuildManifest()
        {
            if (!isBuild)
                return;
            AssetBundlePackager.CollectManifest();
        }

        [MenuItem(BUILD_ASSET_BUILD)]
        public static void BuildAssetBundle()
        {
            if (!isBuild)
                return;
            AssetBundlePackager.PackAssetResources(Settings.PlatformBuildPath, BuildTarget.StandaloneWindows, true);
        }

        [MenuItem(CLEAR_ASSET_BUILD)]
        public static void ClearAssetBundle()
        {
            if (Directory.Exists(Settings.PlatformBuildPath))
            {
                Directory.Delete(Settings.PlatformBuildPath, true);
            }
            AssetDatabase.DeleteAsset(Settings.MANIFEST_JSON_PATH);
        }

        [MenuItem(TEST_ASSET_BUILD)]
        public static void Test()
        {
            var bundle = AssetBundle.LoadFromFile(Settings.PlatformBuildPath + "/assets/customassets.unity3d");
            Object oj = bundle.LoadAsset("Assets/CustomAssets/file.json");
            bundle.Unload(oj);
        }
    }
}
