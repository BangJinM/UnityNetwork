using System.Collections;
using UnityEngine;

namespace US
{
    public class ByteAssetLoader : AbstractResourceLoader
    {
        protected override void Init()
        {
            base.Init();
            ResourceManager.Instance.StartCoroutine(LoadAsset(Url));
        }

        private IEnumerator LoadAsset(string path) { yield break; }

        public static ByteAssetLoader Load(string url)
        {
            var loader = NewLoader<ByteAssetLoader>(url);
            return loader;
        }
    }
}