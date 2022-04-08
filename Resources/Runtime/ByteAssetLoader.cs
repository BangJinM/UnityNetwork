using System.Collections;
using System.IO;
using System;


namespace US
{
    public class ByteAssetLoader : AbstractResourceLoader
    {
        LoaderMode loaderMode { get; set; }
        WebRequestLoader webRequestLoader;

        public static ByteAssetLoader Load(string url, LoaderMode loaderMode = LoaderMode.Sync, LoaderActionCallBack callback = null)
        {
            var loader = NewLoader<ByteAssetLoader>(url, callback);
            loader.loaderMode = loaderMode;
            loader.Init();
            return loader;
        }

        protected override void Init()
        {
            base.Init();
            ResourceLoaderManager.Instance.StartCoroutine(LoadAsset(Url));
        }

        public byte[] LoadAssetsSync(string fullPath)
        {
            byte[] bytes;
            using (FileStream fs = File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                bytes = new byte[fs.Length];
                fs.Read(bytes, 0, (int)fs.Length);
            }

            return bytes;
        }

        private IEnumerator LoadAsset(string path)
        {
            string fullPath = US.PathHelper.GetResourceFullPath(path);
            if (string.IsNullOrEmpty(fullPath))
                yield break;

            object result = null;
            if(loaderMode == LoaderMode.Sync)
            {
                result = LoadAssetsSync(fullPath);
            }
            else
            {
                webRequestLoader = WebRequestLoader.Load(fullPath);
                while (webRequestLoader.AsyncState != AsyncStates.FINISHED)
                {
                    yield return null;
                }

                if (webRequestLoader.AsyncError)
                {
                    AsyncError = true;
                    Finish(result);
                    yield break;
                }
                result = webRequestLoader.AsyncResult;
            }
            Finish(result);
        }

        public override void Dispose()
        {
            base.Dispose();
            if(webRequestLoader != null)
                webRequestLoader.Dispose();
        }
    }
}