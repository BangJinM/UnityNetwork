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
        public string path;
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

        private Dictionary<uint, CustomAssetBundle> id2Bundles;
        private Dictionary<string, uint> path2ID;
        private Dictionary<string, uint> name2ID;

        public void Start()
        {
            path2ID = new Dictionary<string, uint>();
            name2ID = new Dictionary<string, uint>();
            id2Bundles = new Dictionary<uint, CustomAssetBundle>();

            var bundle = AssetBundleManager.Instance.LoadBundle(ABConfig.PlatformBuildPath + "/assets/customassets.bundle");
            TextAsset oj = bundle.assetBundle.LoadAsset<TextAsset>("Assets/CustomAssets/file.json");
            var manifest = ScriptableObject.CreateInstance<Manifest>();
            JsonUtility.FromJsonOverwrite(oj.text, manifest);
            Init(manifest);
            AssetBundleManager.Instance.UnloadBundle(bundle.name);
        }


        public void Init(Manifest manifest)
        {
            this.manifest = manifest;
            foreach (var bundle in manifest.bundles)
            {
                id2Bundles[bundle.id] = bundle;
                path2ID[bundle.path] = bundle.id;
                name2ID[bundle.abName] = bundle.id;
            }
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
                if (id2Bundles.TryGetValue(bundleID, out depBundle)) { dependeces.Add(depBundle.path); }
            }

            return dependeces;
        }
    }
}