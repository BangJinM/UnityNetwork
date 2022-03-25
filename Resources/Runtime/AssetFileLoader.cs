using System.Collections;
using UnityEngine;

namespace US
{
    public class AssetFileLoader : AbstractResourceLoader
    {
        protected override void Init()
        {
            base.Init();
            ResourceManager.Instance.StartCoroutine(LoadAssetBundle(Url));
        }

        private IEnumerator LoadAssetBundle(string path) { yield break; }

        public static AssetFileLoader Load(string url)
        {
            var loader = NewLoader<AssetFileLoader>(url);
            return loader;
        }
    }
}