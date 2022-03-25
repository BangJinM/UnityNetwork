using System.Collections;
using UnityEngine;

namespace US
{
    public class SceneLoader : AbstractResourceLoader
    {
        protected override void Init()
        {
            base.Init();
            ResourceManager.Instance.StartCoroutine(LoadAssetBundle(Url));
        }

        private IEnumerator LoadAssetBundle(string path) { yield break; }

        public static SceneLoader Load(string url)
        {
            var loader = NewLoader<SceneLoader>(url);
            return loader;
        }
    }
}