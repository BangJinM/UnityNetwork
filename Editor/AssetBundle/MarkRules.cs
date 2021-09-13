using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace US
{
    public enum MarkType
    {
        MARK_WITH_DIR,
        MARK_WITH_FILE
    }


    [System.Serializable]
    public class MarkRule
    {
        [Tooltip("搜索路径")] public string searchPath;

        [Tooltip("搜索通配符，多个之间请用,(逗号)隔开")] public string searchPattern = "*";

        [Tooltip("搜索方式：DIR=打包目录下的所有子目录 File=目录下的所有文件")] public MarkType buildType = MarkType.MARK_WITH_DIR;

        public virtual void GetAssets(ref Dictionary<string, List<string>> dic)
        {
            switch (buildType)
            {
                case MarkType.MARK_WITH_DIR:
                    GetAssets2(ref dic);
                    break;
                case MarkType.MARK_WITH_FILE:
                    GetAssets1(ref dic);
                    break;
            }
        }

        private void GetAssets2(ref Dictionary<string, List<string>> dic)
        {
            if (!Directory.Exists(searchPath))
            {
                return;
            }
            var patterns = searchPattern.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in patterns)
            {
                var files = Directory.GetFiles(searchPath, item, SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    if (Directory.Exists(file)) continue;
                    var str = Path.GetDirectoryName(file);
                    var ext = Path.GetExtension(file).ToLower();
                    if ((ext == ".fbx" || ext == ".anim") && !item.Contains(ext)) continue;
                    if (!ValidateAsset(file)) continue;
                    var asset = file.Replace("\\", "/");
                    var abName = AssetBundleEditorUtils.RuledAssetBundleName(str);
                    if (!dic.ContainsKey(abName))
                        dic[abName] = new List<string>();
                    dic[abName].Add(asset);
                }
            }
        }

        public bool ValidateAsset(string asset)
        {
            if (!asset.StartsWith("Assets/")) return false;

            var ext = Path.GetExtension(asset).ToLower();
            return ext != ".dll" && ext != ".cs" && ext != ".meta" && ext != ".js" && ext != ".boo";
        }

        private void GetAssets1(ref Dictionary<string, List<string>> dic)
        {
            if (!Directory.Exists(searchPath))
            {
                return;
            }
            var patterns = searchPattern.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in patterns)
            {
                var files = Directory.GetFiles(searchPath, item, SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                    if (Directory.Exists(file)) continue;
                    var str = Path.GetDirectoryName(file);
                    var ext = Path.GetExtension(file).ToLower();
                    if ((ext == ".fbx" || ext == ".anim") && !item.Contains(ext)) continue;
                    if (!ValidateAsset(file)) continue;
                    var asset = file.Replace("\\", "/");
                    var abName = AssetBundleEditorUtils.RuledAssetBundleName(asset);
                    if (!dic.ContainsKey(abName))
                        dic[abName] = new List<string>();
                    dic[abName].Add(asset);
                }
            }
        }

    }

    [CreateAssetMenu(fileName = "MarkRules", menuName = "UnitySupport/CreateBuildRules", order = 0)]
    public class MarkRules : ScriptableObject
    {
        public List<MarkRule> abRules;
        public Dictionary<string, List<string>> analyseData = new Dictionary<string, List<string>>();
        public Dictionary<string, string> path2ABName = new Dictionary<string, string>();
        [Serializable]
        public struct AssetPath2Name
        {
            public string name;
            public List<string> paths;
        }
        public List<AssetPath2Name> analyseDataInSpector;
        public void Analyse()
        {
            analyseDataInSpector.Clear();
            analyseData.Clear();
            foreach (var rule in abRules)
            {
                rule.GetAssets(ref analyseData);
            }

            foreach (var bundle in analyseData)
            {
                AssetPath2Name assetPath2Name;
                assetPath2Name.name = bundle.Key;
                assetPath2Name.paths = bundle.Value;
                analyseDataInSpector.Add(assetPath2Name);
                foreach(var path in bundle.Value)
                {
                    path2ABName[path] = bundle.Key;
                }
            }
        }
    }
}