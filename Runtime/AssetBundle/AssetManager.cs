using System;
using System.Collections.Generic;
using UnityEngine;

namespace US
{
    public class AssetManager : MonoSingleton<AssetManager>
    {
        Dictionary<string, BundleAsset> assets = new Dictionary<string, BundleAsset>();

        List<BundleAsset> unusedAssets = new List<BundleAsset>();

        private void Update()
        {
            foreach (var asset in assets)
            {
                var item = asset.Value;
                if (!item.IsUnused())
                {
                    item.Update();
                    continue;
                }
                unusedAssets.Add(item);
            }

            for (var i = 0; i < unusedAssets.Count; i++)
            {
                var item = unusedAssets[i];
                assets.Remove(item.assetName);
                item.Unload();
            }

            unusedAssets.Clear();
        }

        public void UnloadAsset(string assetName)
        {
            BundleAsset asset;
            if (assets.TryGetValue(assetName, out asset))
            {
                asset.Release();
            }
        }

        public BundleAsset LoadAsset(string assetName, Type type, bool isAnysc = false)
        {
            BundleAsset asset;
            if (assets.TryGetValue(assetName, out asset))
            {
                return asset;
            }
            asset = isAnysc ? new BundleAnsycAsset(assetName) : new BundleAsset(assetName);
            asset.assetName = assetName;
            asset.assetType = type;
            asset.Load();
            asset.Retain();
            assets[assetName] = asset;
            return null;
        }
    }
}
