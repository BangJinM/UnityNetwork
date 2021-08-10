using System.Collections;
using UnityEngine;

namespace US
{
    class ResourceLoader : IResourceLoader
    {
        public ResourceLoader(string path, LoadedCallback finishCallback = null) : base(path, finishCallback, LoadType.RESOURCES) { }

        public override void Load()
        {
            base.Load();
            mObject = Resources.Load(mPath);
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
            mObject = r.asset;
            Finish();
        }
    }
}
