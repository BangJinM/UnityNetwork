using System;
using System.Collections.Generic;
using UnityEngine;

namespace US
{
    public class AssetBundleManager : MonoSingleton<AssetBundleManager>
    {
        private static readonly int PERFREME_MAX_LOAD_SIZE = 3;

        Dictionary<string, Bundle> assetBundles = new Dictionary<string, Bundle>();

        private static readonly List<Bundle> _unusedBundles = new List<Bundle>();
        private static readonly List<Bundle> _ready2Load = new List<Bundle>();
        private static readonly List<Bundle> _loading = new List<Bundle>();

        private void Start()
        {
            LoadManifest();
        }

        public void LoadManifest()
        {
            //var bundle = LoadBundle(ABConfig.PlatformBuildPath + "/assets/customassets.bundle");
            //TextAsset oj = bundle.assetBundle.LoadAsset<TextAsset>("Assets/CustomAssets/file.json");
            //var manifest = ScriptableObject.CreateInstance<Manifest>();
            //JsonUtility.FromJsonOverwrite(oj.text, manifest);
            //AssetBundleDependenceManager.Instance.Init(manifest);
            //UnloadBundle(bundle.name);
        }

        private void Update()
        {
            for (int i = Math.Min(PERFREME_MAX_LOAD_SIZE - _loading.Count, _ready2Load.Count) - 1; i >= 0; i--)
            {
                var bundle = _ready2Load[i];
                if (bundle.loadState == LoadState.INIT)
                {
                    bundle.Load();
                    _loading.Add(bundle);
                    _ready2Load.RemoveAt(i);
                }
            }

            for (int i = _loading.Count - 1; i >= 0; i--)
            {
                var bundle = _loading[i];
                bundle.Update();
                if (bundle.loadState == LoadState.FINISHED)
                {
                    _loading.RemoveAt(i);
                }
            }

            foreach (var key_bundle in assetBundles)
            {
                var bundle = key_bundle.Value;
                if (bundle.IsUnused())
                {
                    _unusedBundles.Add(bundle);
                }
            }

            for (int i = _unusedBundles.Count - 1; i >= 0; i--)
            {
                var bundle = _unusedBundles[i];
                bundle.Unload();
                assetBundles.Remove(bundle.name);
                UnloadDependencies(bundle, bundle.name);
                _unusedBundles.RemoveAt(i);
            }
        }

        public Bundle LoadBundle(string path, bool isAnsyc = false)
        {
            Bundle bundle;
            if (assetBundles.TryGetValue(path, out bundle))
            {
                assetBundles[path].Retain();
                return bundle;
            }
            bundle = isAnsyc ? new BundleAsync(path) : new Bundle(path);
            if (isAnsyc)
            {
                _ready2Load.Add(bundle);
            }
            else
            {
                bundle.Load();
            }
            LoadDependencies(bundle, path, isAnsyc);
            assetBundles[path] = bundle;
            bundle.Retain();
            return bundle;
        }

        public void UnloadBundle(string path)
        {
            if (assetBundles.ContainsKey(path))
            {
                assetBundles[path].Release();
            }
        }

        public void LoadDependencies(Bundle bundle, string path, bool isAnsyc)
        {
            var deps = AssetBundleDependenceManager.Instance.GetDependences(path);
            foreach (var dep in deps)
            {
                LoadBundle(dep, isAnsyc);
            }
        }

        public void UnloadDependencies(Bundle bundle, string path)
        {
            var deps = AssetBundleDependenceManager.Instance.GetDependences(path);
            foreach (var dep in deps)
            {
                UnloadBundle(dep);
            }
        }

        public bool CheckBundleLoad(string bundleName)
        {
            Bundle bundle;
            if (!assetBundles.TryGetValue(bundleName, out bundle))
                return false;
            var deps = AssetBundleDependenceManager.Instance.GetDependences(bundleName);
            foreach (var dep in deps)
            {
                Bundle depbundle;
                if (!assetBundles.TryGetValue(bundleName, out depbundle))
                    return false;
            }
            return true;
        }
    }
}
