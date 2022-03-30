using System;
using System.Collections;

namespace US
{
    public class AssetFileLoader : AbstractResourceLoader
    {
        LoaderMode loaderMode;

        public static AssetFileLoader Load(string url, LoaderMode loaderMode = LoaderMode.Sync)
        {
            var loader = NewLoader<AssetFileLoader>(url);
            loader.loaderMode = loaderMode;
            loader.Init();
            return loader;
        }

        protected override void Init()
        {
            base.Init();
            ResourceManager.Instance.StartCoroutine(LoadAssetBundle(Url));
        }

        private IEnumerator LoadAssetBundle(string path)
        {
#if UNITY_EDITOR
            
#else


#endif

            AsyncState = AsyncStates.FINISHED;
            yield break;
        }
    }
}