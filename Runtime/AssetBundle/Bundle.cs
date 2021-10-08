using UnityEngine;

namespace US
{
    public class Bundle : Reference
    {
        public string name;
        public string error;
        public AssetBundle assetBundle;
        public LoadState loadState;

        public Bundle(string name)
        {
            loadState = LoadState.INIT;
            this.name = name;
        }

        virtual internal void Load()
        {
            assetBundle = AssetBundle.LoadFromFile(name);
            if (assetBundle == null)
                error = name + " LoadFromFile failed.";
        }

        virtual internal void Unload()
        {
            if (assetBundle == null)
                return;
            assetBundle.Unload(true);
            assetBundle = null;
        }
    }

    public class BundleAsync : Bundle
    {
        private AssetBundleCreateRequest _request;

        public BundleAsync(string name) : base(name)
        {
        }


        internal override void Load()
        {
            _request = AssetBundle.LoadFromFileAsync(name);
            if (_request == null)
            {
                error = name + " LoadFromFile failed.";
                return;
            }
        }

        internal override void Unload()
        {
            if (_request != null)
            {
                _request = null;
            }
            base.Unload();
        }
    }

}
