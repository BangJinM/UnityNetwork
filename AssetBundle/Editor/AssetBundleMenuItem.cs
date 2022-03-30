//using System.IO;
//using UnityEditor;
//using UnityEngine;

//namespace US
//{
//    public class UnitySupportMenuItem
//    {
//        static bool isBuild = false;

//        static string abRes = "Assets/Game/Res/";

//        private const string BUILD = "UnitySupport/AssetBundle/构建";
//        private const string MARK_ASSET_WITH_DIR = "UnitySupport/AssetBundle/分析热更包";
//        private const string BUILD_ASSET_BUILD = "UnitySupport/AssetBundle/构建热更包";
//        private const string CLEAR_ASSET_BUILD_NAME = "UnitySupport/AssetBundle/清除热更包名";
//        private const string CLEAR_ASSET_BUILD = "UnitySupport/AssetBundle/清除热更包";


//        [MenuItem(BUILD)]
//        public static void Build()
//        {
//            Caching.ClearCache();
//            AnalyseAssetBundle();
//            BuildAssetBundle();
//            ClearAssetBundleName();
//        }

//        [MenuItem(MARK_ASSET_WITH_DIR)]
//        public static void AnalyseAssetBundle()
//        {
//            isBuild = true;

//            //var dir = abRes;
//            //int dirLength = dir.Length;
//            //var files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
//            //foreach (var filepath in files)
//            //{
//            //    if (filepath.EndsWith(".meta") && filepath.EndsWith(".cs")) continue;

//            //    var importer = AssetImporter.GetAtPath(filepath);
//            //    if (importer == null)
//            //    {
//            //        continue;
//            //    }

//            //    var bundleName = filepath;//.Substring(dirLength, filepath.Length - dirLength);
//            //    var file = new FileInfo(filepath);
//            //    bundleName = bundleName.Replace(file.Extension, "");//去掉后缀，原因：abBrowser中无法识别abName带有多个.
//            //    importer.assetBundleName = bundleName + ".ab";
//            //}
//        }


//        [MenuItem(BUILD_ASSET_BUILD)]
//        public static void BuildAssetBundle()
//        {
//            if (!isBuild)
//                return;
//            //if (EditorApplication.isPlaying)
//            //{
//            //    return;
//            //}
//            //var outputPath = US.PathHelper.GetStreamingRootPath() + "/" + EditorUserBuildSettings.activeBuildTarget;

//            //if (!Directory.Exists(outputPath))
//            //{
//            //    Directory.CreateDirectory(outputPath);
//            //}
//            ////压缩算法不建议用Lzma，要用LZ4 . Lzma读全部的buffer Lz4一个一个block读取，只读取4字节
//            //var opt = BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression;//BuildAssetBundleOptions.AppendHashToAssetBundleName;
//            //BuildPipeline.BuildAssetBundles(outputPath, opt, EditorUserBuildSettings.activeBuildTarget);
//        }

//        [MenuItem(CLEAR_ASSET_BUILD_NAME)]
//        public static void ClearAssetBundleName()
//        {
//            string dir = abRes;
//            // Check marked asset bundle whether real
//            foreach (var assetGuid in AssetDatabase.FindAssets(""))
//            {
//                var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
//                var assetImporter = AssetImporter.GetAtPath(assetPath);
//                var bundleName = assetImporter.assetBundleName;
//                if (string.IsNullOrEmpty(bundleName))
//                {
//                    continue;
//                }
//                if (!assetPath.StartsWith(dir))
//                {
//                    assetImporter.assetBundleName = null;
//                }
//            }
//        }

//        [MenuItem(CLEAR_ASSET_BUILD)]
//        public static void ClearAssetBundle()
//        {
//        }
//    }
//}

using System.IO;
using UnityEditor;
using UnityEngine;

namespace US
{
    public class UnitySupportMenuItem
    {
        private const string BUILD = "UnitySupport/AssetBundle/构建";
        private const string MARK_ASSET_WITH_DIR = "UnitySupport/AssetBundle/分析热更包";
        private const string BUILD_MANIFEST = "UnitySupport/AssetBundle/构建Manifest";
        private const string BUILD_ASSET_BUILD = "UnitySupport/AssetBundle/构建热更包";
        private const string CLEAR_ASSET_BUILD = "UnitySupport/AssetBundle/清除热更包";

        static string OutputPath
        {
            get
            {
                return US.PathHelper.GetStreamingRootPath();
            }
        }


        [MenuItem(BUILD)]
        public static void Build()
        {
            Caching.ClearCache();
            ClearAssetBundle();
            AnalyseAssetBundle();
            BuildManifest();
            BuildAssetBundle();
        }

        [MenuItem(MARK_ASSET_WITH_DIR)]
        public static void AnalyseAssetBundle()
        {
            AssetBundlePackager.CollectResources();
        }
        [MenuItem(BUILD_MANIFEST)]
        public static void BuildManifest()
        {
            AssetBundlePackager.CollectManifest();
        }

        [MenuItem(BUILD_ASSET_BUILD)]
        public static void BuildAssetBundle()
        {
            AssetBundlePackager.PackAssetResources(OutputPath, BuildTarget.StandaloneWindows, true);
        }

        [MenuItem(CLEAR_ASSET_BUILD)]
        public static void ClearAssetBundle()
        {
            if (Directory.Exists(OutputPath))
            {
                Directory.Delete(OutputPath, true);
            }
        }
    }
}
