using UnityEditor;
using UnityEngine;

namespace US
{
    public class ABMenuItem
    {
        private const string MARK_ASSET_WITH_DIR = "UnitySupport/AssetBundle/分析热更包";
        private const string BUILD_ASSET_BUILD = "UnitySupport/AssetBundle/构建热更包";
        private const string CLEAR_ASSET_BUILD = "UnitySupport/AssetBundle/清除热更包";

        private const string TEST_ASSET_BUILD = "UnitySupport/AssetBundle/Test/清除热更包";

        [MenuItem(MARK_ASSET_WITH_DIR)]
        public static void AnalyseAssetBundle()
        {
            var makeRules = AssetBundleEditorUtils.GetMarkRulesAssetData();
            makeRules.Analyse();
            EditorUtility.SetDirty(makeRules);
            AssetDatabase.SaveAssets();
        }

        [MenuItem(BUILD_ASSET_BUILD)]
        public static void BuildAssetBundle()
        {
            var makeRules = AssetBundleEditorUtils.GetMarkRulesAssetData();
            makeRules.Analyse();
            BuildApp.BuildAssetBundle();
            BuildApp.ClearAssetBundle();
        }

        [MenuItem(CLEAR_ASSET_BUILD)]
        public static void ClearAssetBundle()
        {
            BuildApp.ClearAssetBundle();
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
