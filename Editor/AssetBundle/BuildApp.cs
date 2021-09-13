using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace US
{
    public class BuildApp
    {
        public static void BuildAssetBundle()
        {
            var makeRules = AssetBundleEditorUtils.GetMarkRulesAssetData();
            var analyseData = makeRules.analyseData;
            foreach (var data in analyseData)
            {
                foreach (var name in data.Value)
                {
                    SetABName(data.Key, name);
                }
            }

            Manifest manifest = AssetBundleEditorUtils.GetManifestAssetData();
            manifest.bundles.Clear();
            foreach (var data in makeRules.path2ABName)
            {
                var abName = data.Value;
                var path = data.Key;
                if (Directory.Exists(path)) continue;
                Bundle bundle = new Bundle();
                bundle.path = path;
                bundle.id = CRC32.ComputeCRC32(path);
                bundle.abName = abName;
 
                string[] resDependce = AssetDatabase.GetDependencies(path);
                List<uint> uintList = new List<uint>();
                for (int i = 0; i < resDependce.Length; i++)
                {
                    string tempPath = resDependce[i];
                    if (tempPath == path || tempPath.EndsWith(".cs"))
                        continue;

                    string depABName = "";
                    if (makeRules.path2ABName.TryGetValue(tempPath, out depABName))
                    {
                        if (depABName == abName)
                            continue;
                        var depID = CRC32.ComputeCRC32(tempPath);
                        if (!uintList.Contains(depID))
                        {
                            uintList.Add(depID);
                        }
                    }
                }
                bundle.deps = uintList.ToArray();
                manifest.bundles.Add(bundle);
            }
            WriteManifest(manifest);
            EditorUtility.SetDirty(manifest);
            AssetDatabase.SaveAssets();
            var outputPath = Settings.PlatformBuildPath;
            AssetBundleManifest outManifest = BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
            if (outManifest == null)
            {
                Debug.LogError("AssetBundle 打包失败！");
            }
            else
            {
                Debug.Log("AssetBundle 打包完毕");
            }
        }

        public static void WriteManifest(Manifest manifest) {
            var str = JsonUtility.ToJson(manifest);
            File.WriteAllText("Assets/CustomAssets/file.json", str);
        }

        public static void ClearAssetBundle()
        {
            var makeRules = AssetBundleEditorUtils.GetMarkRulesAssetData();
            makeRules.Analyse();
            var analyseData = makeRules.analyseData;

            foreach (var data in analyseData)
            {
                foreach (var name in data.Value)
                {
                    ClearABName(name);
                }
            }
        }

        static void SetABName(string name, string path)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(path);
            if (assetImporter == null)
            {
                Debug.LogError("不存在此路径文件：" + path);
            }
            else
            {
                assetImporter.assetBundleName = name;
            }
        }

        static void ClearABName(string path)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(path);
            if (assetImporter == null)
            {
                Debug.LogError("不存在此路径文件：" + path);
            }
            else
            {
                assetImporter.assetBundleName = null;
            }
        }
    }
}
