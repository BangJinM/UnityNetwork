using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace US
{
    [Serializable]
    public class USAssetBundle
    {
        public uint id;
        public long size;
        public uint[] deps;
        public string assetName;
        public string abName;
    }

    [Serializable]
    public class Manifest : ScriptableObject
    {
        public string version;
        public List<USAssetBundle> bundles = new List<USAssetBundle>();
    }

    [Serializable]
    public class AssetBundleManager : MonoSingleton<AssetBundleManager>
    {
        public List<USAssetBundle> bundles = new List<USAssetBundle>();

        public Dictionary<string, uint> pathIDDict = new Dictionary<string, uint>();
        public Dictionary<uint, USAssetBundle> idBundlsDict = new Dictionary<uint, USAssetBundle>();


        public bool inited = false;


        private void Awake() {
            LoadManifest();
            foreach (var bundle in bundles)
            {
                pathIDDict.Add(bundle.abName, bundle.id);
                idBundlsDict.Add(bundle.id, bundle);
            }
            inited = true;
        }

        /// <summary>
        /// 加载manifest
        /// </summary>
        void LoadManifest()
        {
            var loader = US.ByteAssetLoader.Load("assets/game/manifest.ab");
            var ab = AssetBundle.LoadFromMemory(loader.AsyncResult as byte[]);
            var manifest = ab.LoadAsset<US.Manifest>("manifest.asset");
            bundles = manifest.bundles;
            ab.Unload(true);
            loader.Release();
        }

        public void Init()
        {
            LoadManifest();
            foreach (var bundle in bundles)
            {
                pathIDDict.Add(bundle.abName, bundle.id);
                idBundlsDict.Add(bundle.id, bundle);
            }
            inited = true;

        }

        public uint GetID(string abName)
        {
            uint result;
            if (pathIDDict.TryGetValue(abName, out result))
                return result;
            return 0;
        }

        public uint[] GetDeps(uint id)
        {
            USAssetBundle bundle = null;
            if (idBundlsDict.TryGetValue(id, out bundle))
                return bundle.deps;
            return null;
        }

        public USAssetBundle GetBundle(uint id)
        {
            USAssetBundle bundle = null;
            if (idBundlsDict.TryGetValue(id, out bundle))
                return bundle;
            return null;
        }
    }
}