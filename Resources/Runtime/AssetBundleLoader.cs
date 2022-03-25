using System.Collections;
using UnityEngine;

namespace US
{
    public class AssetBundleLoader : AbstractResourceLoader
    {

        protected override void Init()
        {
            base.Init();
            ResourceManager.Instance.StartCoroutine(LoadAssetBundle(Url));
        }

        private IEnumerator LoadAssetBundle(string path) { yield break; }

        public static AssetBundleLoader Load(string url)
        {
            var loader = NewLoader<AssetBundleLoader>(url);
            return loader;
        }

    }
}