using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            MainManager.Instance.StartCoroutine(ReallyLoadAsync());
        }

        private IEnumerator ReallyLoadAsync()
        {
            ResourceRequest r = Resources.LoadAsync(mPath);
            yield return r;
            Finish();
        }
    }
}
