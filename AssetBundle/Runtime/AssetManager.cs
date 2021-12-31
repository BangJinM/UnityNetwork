using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace US
{
    public class AssetManager : MonoSingleton<AssetManager>
    {
        Dictionary<string, USAsset> assets = new Dictionary<string, USAsset>();

        List<USAsset> unusedAssets = new List<USAsset>();

        private void Update()
        {
            foreach (var asset in assets)
            {
                var item = asset.Value;
                if (!item.IsUnused())
                    item.Update();
                else
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
            USAsset asset;
            if (assets.TryGetValue(assetName, out asset))
            {
                asset.Release();
            }
        }

        public USAsset LoadAsset(string assetName, Type type, bool isAnysc = false)
        {
            USAsset asset;
            if (assets.TryGetValue(assetName, out asset))
            {
                return asset;
            }
            //#if UNITY_EDITOR
            //            asset = new AssetDataBaseAsset();
            //#else
            var ab = AssetBundleDependenceManager.Instance.GetBundleAsset(assetName);
            var abPath = ABConfig.PlatformBuildPath + "/" + ab.abName;
            abPath.ToLower();
            if (isAnysc)
                asset = new BundleAnsycAsset(abPath);
            else
                asset = new BundleAsset(abPath);
            //#endif

            asset.assetName = assetName;
            asset.assetType = type;
            asset.Load();
            asset.Retain();
            assets[assetName] = asset;
            return asset;
        }

        public USAsset ResourceLoad(string assetName, Type type)
        {
            USAsset asset;
            if (assets.TryGetValue(assetName, out asset))
            {
                return asset;
            }
            asset = new ResourceAsset();

            asset.assetName = assetName;
            asset.assetType = type;
            asset.Load();
            asset.Retain();
            assets[assetName] = asset;
            return asset;
        }
#if UNITY_EDITOR
        public USAsset AssetDataBaseLoad(string assetName, Type type)
        {
            USAsset asset;
            if (assets.TryGetValue(assetName, out asset))
            {
                return asset;
            }
            asset = new AssetDataBaseAsset();

            asset.assetName = assetName;
            asset.assetType = type;
            asset.Load();
            asset.Retain();
            assets[assetName] = asset;
            return asset;
        }
#endif
    }
}
