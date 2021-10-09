using UnityEngine;

namespace US
{
    public class Bundle : Reference
    {
        public string name;
        public string error;
        public AssetBundle assetBundle;
        public LoadState loadState;

        public virtual bool isDone
        {
            get
            {
                return true;
            }
        }

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

        internal virtual void Update() { }
    }

    public class BundleAsync : Bundle
    {
        private AssetBundleCreateRequest _request;

        public override bool isDone
        {
            get
            {
                if (loadState == LoadState.INIT || loadState == LoadState.ERROR || loadState == LoadState.LOADING)
                    return false;
                if (loadState == LoadState.FINISHED)
                    return true;
                return _request.isDone;
            }
        }

        public BundleAsync(string name) : base(name)
        {
        }


        internal override void Load()
        {
            loadState = LoadState.LOADING;
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


        internal override void Update()
        {
            if (_request.isDone && loadState != LoadState.FINISHED)
            {
                loadState = LoadState.FINISHED;
            }
        }
    }

}
