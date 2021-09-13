using System.Collections;
using UnityEngine;

namespace US
{
    class AssetBundleLoader : IResourceLoader
    {
        public AssetBundleLoader(string path, LoadedCallback finishCallback = null) : base(path, finishCallback, LoadType.ASSETBUNDLE){ }

        public override void Load()
        {
            base.Load();
            Finish();
        }

        public override void LoadAsyc()
        {
            base.LoadAsyc();
            StartCoroutine(ReallyLoadAsync());
        }

        private IEnumerator ReallyLoadAsync()
        {
            ResourceRequest r = Resources.LoadAsync(mPath);
            yield return r;
            Finish();
        }
    }
}
