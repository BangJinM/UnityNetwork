using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace US
{
    public enum BuildType
    {
        DIR,    //文件夹下面所有文件夹单独出ab包
        FILE,   //文件夹下面单个文件出ab包
    }


    public static class BuildRuleUtils
    {
        public static bool ValidateAsset(string asset)
        {
            if (!asset.StartsWith("Assets/")) return false;

            var ext = Path.GetExtension(asset).ToLower();
            return ext != ".dll" && ext != ".cs" && ext != ".meta" && ext != ".js" && ext != ".boo";
        }

        public static string RuledAssetBundleName(string name)
        {
            return name.Replace("\\", "/").ToLower() + ".unity3d";
        }
    }

    [System.Serializable]
    public class BuildRule
    {
        [Tooltip("搜索路径")] public string searchPath;

        [Tooltip("搜索通配符，多个之间请用,(逗号)隔开")] public string searchPattern;

        [Tooltip("搜索方式：DIR=打包目录下的所有子目录 File=目录下的所有文件")] public BuildType buildType = BuildType.DIR;

        private void GetDirs(string searchPath, ref List<string> dirList)
        {
            string[] dirs = Directory.GetDirectories(searchPath, "*", SearchOption.TopDirectoryOnly);
            foreach (var dir in dirs)
            {
                dirList.Add(dir);
                GetDirs(dir,ref dirList);
            }
        }

        public Dictionary<string, string> GetAssets( )
        {
            Dictionary<string, string> _asset2Bundles = new Dictionary<string, string>();
            if (!Directory.Exists(searchPath))
            {
                return _asset2Bundles;
            }
            var patterns = searchPattern.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            switch (buildType)
            {
                case BuildType.DIR:
                    var resultdirs = new List<string>();
                    GetDirs(searchPath,ref resultdirs);
                    foreach(var dir in resultdirs)
                    {
                        foreach (var item in patterns)
                        {
                            var files = Directory.GetFiles(dir, item, SearchOption.TopDirectoryOnly);
                            foreach (var file in files)
                            {
                                if (Directory.Exists(file)) continue;
                                var ext = Path.GetExtension(file).ToLower();
                                if ((ext == ".fbx" || ext == ".anim") && !item.Contains(ext)) continue;
                                if (!BuildRuleUtils.ValidateAsset(file)) continue;
                                var asset = file.Replace("\\", "/");
                                _asset2Bundles[asset] = BuildRuleUtils.RuledAssetBundleName(Path.GetDirectoryName(asset));
                            }
                        }
                    }
                    break;
                case BuildType.FILE:
                    foreach (var item in patterns)
                    {
                        var files = Directory.GetFiles(searchPath, item, SearchOption.AllDirectories);
                        foreach (var file in files)
                        {
                            if (Directory.Exists(file)) continue;
                            var ext = Path.GetExtension(file).ToLower();
                            if ((ext == ".fbx" || ext == ".anim") && !item.Contains(ext)) continue;
                            if (!BuildRuleUtils.ValidateAsset(file)) continue;
                            var asset = file.Replace("\\", "/");
                            _asset2Bundles[asset] = BuildRuleUtils.RuledAssetBundleName(asset);
                        }
                    }
                    break;
            }
            return _asset2Bundles;
        }
    }
    [CreateAssetMenu(fileName = "BuildRules", menuName = "CreateBuildRules", order = 0)]
    public class BuildRules : ScriptableObject
    {
        public Dictionary<string, string> _asset2Bundles = new Dictionary<string, string>();
        public List<BuildRule> buildRules = new List<BuildRule>();

        public void Build(ref Dictionary<string, string> path2assetbundle)
        {
            foreach (var buildRule in buildRules)
            {

                var tempAsset2Bundles = buildRule.GetAssets();
                foreach(var bundles in tempAsset2Bundles)
                {
                    _asset2Bundles[bundles.Key] = bundles.Value;
                }
            }
        }

        private void BuildFile(ref Dictionary<string, string> path2assetbundle)
        {
        }

        private void BuildDir(ref Dictionary<string, string> path2assetbundle)
        {
        }
    }

}