using System.Collections;
using System;

namespace US
{
    public class SceneLoader : AbstractResourceLoader
    {
        public static SceneLoader Load(string url)
        {
            var loader = NewLoader<SceneLoader>(url);
            loader.Init();
            return loader;
        }

        protected override void Init()
        {
            base.Init();
            ResourceLoaderManager.Instance.StartCoroutine(LoadAssetBundle(Url));
        }

        private IEnumerator LoadAssetBundle(string path) { yield break; }
    }
}