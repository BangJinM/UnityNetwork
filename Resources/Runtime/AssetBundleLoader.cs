using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace US
{
    public class AssetBundleLoader : AbstractResourceLoader
    {
        static Manifest manifest = null;
        LoaderMode loaderMode;
        List<AssetBundleLoader> depLoaders;

        public static AssetBundleLoader Load(string url, LoaderMode loaderMode = LoaderMode.Sync, LoaderActionCallBack callBack = null)
        {
            var loader = NewLoader<AssetBundleLoader>(url.ToLower(), callBack);
            loader.loaderMode = loaderMode;
            loader.Init();
            return loader;
        }

        protected override void Init()
        {
            base.Init();

            depLoaders = new List<AssetBundleLoader>();
            ResourceLoaderManager.Instance.StartCoroutine(LoadAssetBundle(Url));
        }

        private IEnumerator LoadAssetBundle(string path)
        {
            object result = null;

            var fullpath = PathHelper.GetResourceFullPath(path);
            if (fullpath == "")
                yield break;

            if (loaderMode == LoaderMode.Sync)
            {
                result = AssetBundle.LoadFromFile(fullpath);
            }
            else
            {
                LoadDeps(path);
                foreach (var loader in depLoaders)
                {
                    while (loader.AsyncState != AsyncStates.FINISHED)
                        yield return null;
                }

                var ab = AssetBundle.LoadFromFileAsync(fullpath);
                while (!ab.isDone)
                    yield return null;
                result = ab.assetBundle;
            }
            Finish(result);
        }

        public void LoadDeps(string path)
        {
            uint abID = AssetBundleManager.Instance.GetID(path);
            var depIDs = AssetBundleManager.Instance.GetDeps(abID);
            if (depIDs != null)
            {
                foreach (var id in depIDs)
                {
                    var bundle = AssetBundleManager.Instance.GetBundle(id);
                    if (bundle != null)
                    {
                        var depLoader = AssetBundleLoader.Load(bundle.abName, loaderMode);
                        depLoaders.Add(depLoader);
                    }
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (IsUnused())
            {
                var assetBundle = AsyncResult as AssetBundle;
                assetBundle.Unload(true);
            }
            foreach (var assetBundleLoader in depLoaders)
            {
                assetBundleLoader.Release();
            }
            depLoaders.Clear();
        }
    }
}