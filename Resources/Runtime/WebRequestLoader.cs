using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;

namespace US
{
    public class WebRequestLoader : AbstractResourceLoader
    {
        /// <summary>
        /// 正在加载的总数
        /// </summary>
        private static int WebRequestLoadingCount = 0;

        UnityWebRequest request;

        public static WebRequestLoader Load(string url)
        {
            var loader = NewLoader<WebRequestLoader>(url);
            loader.Init();
            return loader;
        }

        protected override void Init()
        {
            base.Init();
            ResourceManager.Instance.StartCoroutine(LoadAsset(Url));
        }

        private IEnumerator LoadAsset(string path)
        {
            WebRequestLoadingCount++;
            object result = null;

            UnityWebRequest request = UnityWebRequest.Get(path);
            yield return request.SendWebRequest();

            WebRequestLoadingCount--;
            if (request.isHttpError || request.isNetworkError)
            {
                AsyncError = true;
                Finish(result);
                yield break;
            }
            result = request.downloadHandler.data;
            Finish(result);
        }

        public override void Dispose()
        {
            base.Dispose();

            if (request != null)
            {
                request.Dispose();
            }
        }
    }
}