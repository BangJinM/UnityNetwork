using System;
using System.Collections.Generic;
using UnityEngine;

namespace US
{
    public class AssetBundleManager : MonoSingleton<AssetBundleManager>
    {
        private static readonly int PERFREME_MAX_LOAD_SIZE = 3;

        Dictionary<string, BundleBase> assetBundles = new Dictionary<string, BundleBase>();

        private static readonly List<BundleBase> _unusedBundles = new List<BundleBase>();
        private static readonly List<BundleBase> _ready2Load = new List<BundleBase>();
        private static readonly List<BundleBase> _loading = new List<BundleBase>();

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

        public BundleBase LoadBundle(string path, bool isAnsyc = false)
        {
            BundleBase bundle;
            if (assetBundles.TryGetValue(path, out bundle))
            {
                assetBundles[path].Retain();
                return bundle;
            }
            if (isAnsyc)
                bundle = new BundleAsync(path);
            else
                bundle = new Bundle(path);
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
            return bundle;
        }

        public void UnloadBundle(string path)
        {
            if (assetBundles.ContainsKey(path))
            {
                assetBundles[path].Release();
            }
        }

        public void LoadDependencies(BundleBase bundle, string path, bool isAnsyc)
        {
            var deps = AssetBundleDependenceManager.Instance.GetDependences(path);
            foreach (var dep in deps)
            {
                LoadBundle(dep, isAnsyc);
            }
        }

        public void UnloadDependencies(BundleBase bundle, string path)
        {
            var deps = AssetBundleDependenceManager.Instance.GetDependences(path);
            foreach (var dep in deps)
            {
                UnloadBundle(dep);
            }
        }

        public bool CheckBundleLoad(string bundleName)
        {
            BundleBase bundle;
            if (!assetBundles.TryGetValue(bundleName, out bundle))
                return false;
            var deps = AssetBundleDependenceManager.Instance.GetDependences(bundleName);
            foreach (var dep in deps)
            {
                BundleBase depbundle;
                if (!assetBundles.TryGetValue(bundleName, out depbundle))
                    return false;
            }
            return true;
        }
    }
}
