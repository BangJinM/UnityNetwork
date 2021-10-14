using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace US
{
    [System.Serializable]
    public class CustomAssetBundle
    {
        public uint id;
        public long size;
        public uint[] deps;
        public string assetName;
        public string abName;
    }

    public class Manifest : ScriptableObject
    {
        public string version;
        public List<CustomAssetBundle> bundles = new List<CustomAssetBundle>();
    }

    public class AssetBundleDependenceManager : MonoSingleton<AssetBundleDependenceManager>
    {
        Manifest manifest;

        private Dictionary<uint, CustomAssetBundle> id2Bundles = new Dictionary<uint, CustomAssetBundle>();
        private Dictionary<string, uint> path2ID = new Dictionary<string, uint>();
        private Dictionary<string, CustomAssetBundle> name2Bundles = new Dictionary<string, CustomAssetBundle>();

        public void Init()
        {
            Bundle bundleAB = AssetBundleManager.Instance.LoadBundle(ABConfig.PlatformBuildPath + "/assets/customassets.bundle") as Bundle;
            TextAsset oj = bundleAB.assetBundle.LoadAsset<TextAsset>("Assets/CustomAssets/file.json");
            var manifest = ScriptableObject.CreateInstance<Manifest>();
            JsonUtility.FromJsonOverwrite(oj.text, manifest);
            this.manifest = manifest;
            foreach (var bundle in manifest.bundles)
            {
                id2Bundles[bundle.id] = bundle;
                path2ID[bundle.assetName] = bundle.id;
                name2Bundles[bundle.assetName] = bundle;
            }
            AssetBundleManager.Instance.UnloadBundle(bundleAB.name);
        }

        public List<string> GetDependences(string path)
        {
            List<string> dependeces = new List<string>();
            uint bundleID;
            if (!path2ID.TryGetValue(path, out bundleID)) { return dependeces; }

            CustomAssetBundle assetBundle;
            if (!id2Bundles.TryGetValue(bundleID, out assetBundle)) { return dependeces; }

            foreach (var id in assetBundle.deps)
            {
                CustomAssetBundle depBundle;
                if (id2Bundles.TryGetValue(bundleID, out depBundle)) { dependeces.Add(depBundle.assetName); }
            }

            return dependeces;
        }

        public CustomAssetBundle GetBundleAsset(string assetName)
        {
            CustomAssetBundle bundleAsset;
            if (name2Bundles.TryGetValue(assetName, out bundleAsset))
            {
                return bundleAsset;
            }
            return null;
        }
    }
}