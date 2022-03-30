using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace US
{
    class AssetBundlePackager
    {
        public static string MANIFEST_JSON_PATH = "Assets/Game/Manifest/minifest.json";
        public static string bundleFileExt = ".ab";
        public static string resourcesRoot = "";

        public static Dictionary<string, AssetBundleBuild> bundleList = new Dictionary<string, AssetBundleBuild>();
        public static Dictionary<string, AssetBundleBuild> assetMap = new Dictionary<string, AssetBundleBuild>();

        /// <summary>
        /// 遍历某个文件夹下的所有资源，遍历完之后会存储在 resSet
        /// action 传入的两个路径，第一个是相对路径，第二个是绝对路径
        /// </summary>
        static private void ForEachAssets(string path, string filter, Action<string, string> action)
        {
            HashSet<string> resSet = new HashSet<string>();

            List<string> list = new List<string>();
            list.Add(resourcesRoot + path);
            string[] allStr = AssetDatabase.FindAssets(filter, list.ToArray());
            for (int i = 0; i < allStr.Length; i++)
            {
                if (!resSet.Contains(allStr[i]))
                {
                    string resPath = AssetDatabase.GUIDToAssetPath(allStr[i]);
                    if (File.Exists(resPath))
                        action(resPath, resPath);
                    resSet.Add(allStr[i]);
                }
            }
        }

        /// <summary>
        /// 把资源加入到打包列表中，每个资源单独打成一个 CustomAssetBundle
        /// </summary>
        public static void AddResourcesToPackages(string path, string filter = null, string exclude = null)
        {
            Regex regex = string.IsNullOrEmpty(exclude) ? null : new Regex(@exclude, RegexOptions.Singleline);

            ForEachAssets(path, filter, delegate (string resPath, string assetPath)
            {
                if (regex == null || !regex.IsMatch(resPath))
                {
                    AssetBundleBuild build = new AssetBundleBuild();
                    build.assetBundleName = resPath + bundleFileExt;
                    build.assetNames = new string[] { assetPath };
                    build.assetBundleName = build.assetBundleName.ToLower();

                    if (!bundleList.ContainsKey(build.assetBundleName))
                    {
                        bundleList[build.assetBundleName] = build;
                    }
                    else
                    {
                        Debug.LogWarning("same build in package " + build.assetBundleName);
                    }
                }
            });
        }
        /// <summary>
        /// 把资源加入到打包列表中，该路径下的资源打包成一个包
        /// </summary>
        static public void AddResourcesToOnePackage(string path, string filter = null, string exclude = null)
        {
            List<string> assetList = new List<string>();
            Regex regex = string.IsNullOrEmpty(exclude) ? null : new Regex(@exclude, RegexOptions.Singleline);
            ForEachAssets(path, filter, delegate (string resPath, string assetPath)
            {
                if (regex == null || !regex.IsMatch(resPath))
                {
                    assetList.Add(assetPath);
                }
            });


            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = path + bundleFileExt;
            build.assetBundleName = build.assetBundleName.ToLower();
            build.assetNames = assetList.ToArray();

            if (!bundleList.ContainsKey(build.assetBundleName))
            {
                bundleList[build.assetBundleName] = build;
            }
            else
            {
                Debug.LogWarning("same build in package " + build.assetBundleName);
            }
        }

        /// <summary>
        /// 把资源加入到打包列表中，每个子,孙子文件夹各打包成一个 CustomAssetBundle
        /// </summary>
        public static void AddSubFoldersForEachToPackages(string path, string filter = null, string exclude = null)
        {
            Dictionary<string, List<string>> assetList = new Dictionary<string, List<string>>();
            Regex regex = string.IsNullOrEmpty(exclude) ? null : new Regex(@exclude, RegexOptions.Singleline);
            ForEachAssets(path, filter, delegate (string resPath, string assetPath)
            {
                var dirPath = Path.GetDirectoryName(assetPath);
                dirPath = dirPath.Replace("\\", "/").ToLower();
                if (regex == null || !regex.IsMatch(assetPath))
                {
                    if (!assetList.ContainsKey(assetPath))
                    {
                        assetList[dirPath] = new List<string>();
                    }
                    assetList[dirPath].Add(assetPath);
                }
            });

            foreach (var kvp in assetList)
            {
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = kvp.Key + bundleFileExt;
                build.assetBundleName = build.assetBundleName.ToLower();
                build.assetNames = kvp.Value.ToArray();

                if (!bundleList.ContainsKey(build.assetBundleName))
                {
                    bundleList[build.assetBundleName] = build;
                }
                else
                {
                    Debug.LogWarning("same build in package " + build.assetBundleName);
                }
            }
        }

        /// <summary>
        /// 把资源加入到打包列表中，每个子文件夹各打包成一个 CustomAssetBundle
        /// </summary>
        public static void AddSubFoldersToPackages(string path, string filter = null, string exclude = null)
        {
            HashSet<string> folders = new HashSet<string>();
            string resPath = resourcesRoot + path;
            string[] resFolders = AssetDatabase.GetSubFolders(resPath);
            folders.UnionWith(resFolders);

            var itor = folders.GetEnumerator();
            while (itor.MoveNext())
            {
                AddResourcesToOnePackage(itor.Current, filter, exclude);
            }
        }

        /// <summary>
        /// 打包场景资源，只被一个场景引用的资源剔除掉，不单独打包成 CustomAssetBundle
        /// </summary>
        static public void AddSceneAssetsToPackages(string path, string filter = null, string exclude = null)
        {
            // key：资源名   value：被引用的场景的个数
            Dictionary<string, int> scenesDependenciesMap = new Dictionary<string, int>();
            // 获取所有场景的引用
            ForEachAssets("level", "t:scene", delegate (string resPath, string assetPath)
            {
                foreach (var patnTemp in AssetDatabase.GetDependencies(assetPath))
                {
                    if (scenesDependenciesMap.ContainsKey(patnTemp))
                    {
                        scenesDependenciesMap[patnTemp] += 1;
                    }
                    else
                    {
                        scenesDependenciesMap.Add(patnTemp, 1);
                    }
                }
            });


            Regex regex = string.IsNullOrEmpty(exclude) ? null : new Regex(@exclude, RegexOptions.Singleline);
            ForEachAssets(path, filter, delegate (string resPath, string assetPath)
            {
                if (regex == null || !regex.IsMatch(resPath))
                {
                    // 无人引用或者被多个场景引用，则单独打包成一个 CustomAssetBundle
                    if (!scenesDependenciesMap.ContainsKey(assetPath) || scenesDependenciesMap[assetPath] > 1)
                    {
                        AssetBundleBuild build = new AssetBundleBuild();
                        build.assetBundleName = resPath + bundleFileExt;
                        build.assetBundleName = build.assetBundleName.ToLower();
                        build.assetNames = new string[] { assetPath };

                        if (!bundleList.ContainsKey(build.assetBundleName))
                        {
                            bundleList[build.assetBundleName] = build;
                        }
                        else
                        {
                            Debug.LogWarning("same build in package " + build.assetBundleName);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 打包特效资源，只被一个目标类型的资源引用的剔除掉，和该资源打成一个包，不单独打包成 CustomAssetBundle
        /// </summary>
        static public void AddAssetsToPackagesByRef(string path, string targetPath, string filter = null, string exclude = null, string targetFilter = null)
        {
            // key：资源名   value：被引用的场景的个数
            Dictionary<string, int> assetsDependenciesMap = new Dictionary<string, int>();
            // 获取所有场景的引用
            ForEachAssets(targetPath, targetFilter, delegate (string resPath, string assetPath)
            {
                foreach (var patnTemp in AssetDatabase.GetDependencies(assetPath))
                {
                    if (assetsDependenciesMap.ContainsKey(patnTemp))
                    {
                        assetsDependenciesMap[patnTemp] += 1;
                    }
                    else
                    {
                        assetsDependenciesMap.Add(patnTemp, 1);
                    }
                }
            });


            Regex regex = string.IsNullOrEmpty(exclude) ? null : new Regex(@exclude, RegexOptions.Singleline);
            ForEachAssets(path, filter, delegate (string resPath, string assetPath)
            {
                if (regex == null || !regex.IsMatch(resPath))
                {
                    // 无人引用或者被多个场景引用，则单独打包成一个 CustomAssetBundle
                    if (!assetsDependenciesMap.ContainsKey(assetPath) || assetsDependenciesMap[assetPath] > 1)
                    {
                        AssetBundleBuild build = new AssetBundleBuild();
                        build.assetBundleName = resPath + bundleFileExt;
                        build.assetBundleName = build.assetBundleName.ToLower();
                        build.assetNames = new string[] { assetPath };

                        if (!bundleList.ContainsKey(build.assetBundleName))
                        {
                            bundleList[build.assetBundleName] = build;
                        }
                        else
                        {
                            Debug.LogWarning("same build in package " + build.assetBundleName);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 打包资源
        /// </summary>
        static public void PackAssetResources(string exportRoot, BuildTarget buildTarget, bool forceRebuild = false, bool buildResTemp = false, bool useLZMACompress = false)
        {
            if (bundleList.Count <= 0)
            {
                return;
            }

            var packageExportPath = exportRoot;
            if (forceRebuild)
            {
                if (Directory.Exists(packageExportPath))
                {
                    Directory.Delete(packageExportPath, true);
                }
            }
            Directory.CreateDirectory(packageExportPath);

            BuildAssetBundleOptions buildOptions = BuildAssetBundleOptions.DeterministicAssetBundle;
            if (forceRebuild)
            {
                buildOptions |= BuildAssetBundleOptions.ForceRebuildAssetBundle;
            }
            if (!useLZMACompress)
            {
                buildOptions |= BuildAssetBundleOptions.ChunkBasedCompression;
            }

            buildOptions |= BuildAssetBundleOptions.DisableWriteTypeTree;
            List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
            foreach (var value in bundleList)
            {
                builds.Add(value.Value);
            }
            var manifest = BuildPipeline.BuildAssetBundles(packageExportPath, builds.ToArray(), buildOptions, buildTarget);
            if (manifest == null)
            {
                throw new Exception("BuildAssetBundles Error!");
            }
        }
        /// <summary>
        /// 根据ABRule收集改打包的文件
        /// </summary>
        public static void CollectResources()
        {
            bundleList.Clear();
            try
            {
                var abRules = Utils.GetScriptableObjectAsset<MarkRules>("Assets/Editor/MarkRules.asset");
                foreach (var config in abRules.abRules)
                {
                    Logger.Info(config.searchPath + " " + config.searchPattern + " " + config.exclude + " " + config.buildType);
                    if (config.buildType == PackageMode.ONE_FILE2PACKAGE)
                    {
                        AddResourcesToPackages(config.searchPath, config.searchPattern, config.exclude);
                    }
                    else if (config.buildType == PackageMode.All_FILE2PACKAGE)
                    {
                        AddResourcesToOnePackage(config.searchPath, config.searchPattern, config.exclude);
                    }
                    else if (config.buildType == PackageMode.SUB_DIR_FOREACH2PACKAGE)
                    {
                        AddSubFoldersForEachToPackages(config.searchPath, config.searchPattern, config.exclude);
                    }
                    else if (config.buildType == PackageMode.SUB_DIR2PACKAGE)
                    {
                        AddSubFoldersToPackages(config.searchPath, config.searchPattern, config.exclude);
                    }
                    else if (config.buildType == PackageMode.SCENE2PACKAGE)
                    {
                        AddSceneAssetsToPackages(config.searchPath, config.searchPattern, config.exclude);
                    }
                    else if (config.buildType == PackageMode.REF2PACKAGE)
                    {
                        AddAssetsToPackagesByRef(config.searchPath, config.searchPattern, config.exclude);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }

       /// <summary>
       /// 收集单个资源到ab包名的映射
       /// </summary>
        static private void CollectAssetMap()
        {
            assetMap.Clear();
            foreach(var value in bundleList)
            {
                var assetBundleName = value.Key;
                var assetBundleBuild = value.Value;
                foreach (var path in assetBundleBuild.assetNames)
                {
                    if(assetMap.ContainsKey(path))
                    {
                        Logger.Error("CollectAssetMap ERROR! assetName = " + path);
                        continue;
                    }    
                    assetMap[path] = assetBundleBuild;
                }
            }
        }

        /// <summary>
        /// 收集并导出Manifest
        /// </summary>
        public static void CollectManifest()
        {
            CollectAssetMap();
            Manifest manifest = Utils.GetScriptableObjectAsset<Manifest>("Assets/Game/Manifest/manifest.asset");
            manifest.bundles.Clear();
            foreach (var data in bundleList)
            {
                var assetBundleName = data.Key;
                var assetBundleBuild = data.Value;

                foreach (var path in assetBundleBuild.assetNames)
                {
                    USAssetBundle bundle = new USAssetBundle();
                    bundle.assetName = path;
                    bundle.id = CRC32.ComputeCRC32(path);
                    bundle.abName = assetBundleName;

                    string[] resDependce = AssetDatabase.GetDependencies(path);
                    List<uint> uintList = new List<uint>();
                    for (int i = 0; i < resDependce.Length; i++)
                    {
                        string tempPath = resDependce[i];
                        if (tempPath == path || tempPath.EndsWith(".cs"))
                            continue;

                        AssetBundleBuild depAssetBundleBuild;
                        if (assetMap.TryGetValue(tempPath, out depAssetBundleBuild))
                        {
                            if (depAssetBundleBuild.assetBundleName == assetBundleName)
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
            }
            //var str = JsonUtility.ToJson(manifest);
            //File.WriteAllText(MANIFEST_JSON_PATH, str);
            AssetDatabase.SaveAssets();
            AddResourcesToOnePackage("Assets/Game/Manifest");
        }

    }
}
