using UnityEngine;

namespace US
{
    public class BundleBase : Reference
    {
        public string name;
        public string error;
        public LoadState loadState;
        public virtual bool isDone
        {
            get
            {
                return true;
            }
        }
        public BundleBase(string name)
        {
            loadState = LoadState.INIT;
            this.name = name;
        }

        virtual internal void Load() { }

        virtual internal void Unload() { }

        internal virtual void Update() { }

    }

    public class Bundle : BundleBase
    {
        public AssetBundle assetBundle;

        public override bool isDone
        {
            get
            {
                return true;
            }
        }

        public Bundle(string name) : base(name) { }

        internal override void Load()
        {
            Retain();
            assetBundle = AssetBundle.LoadFromFile(name);
            if (assetBundle == null)
            {
                error = name + " LoadFromFile failed.";
                Release();
            }
        }

        internal override void Unload()
        {
            if (assetBundle == null)
                return;
            Release();
            if (IsUnused())
            {
                assetBundle.Unload(true);
                assetBundle = null;
            }
        }
    }

    public class BundleAsync : BundleBase
    {
        public AssetBundleCreateRequest _request;

        public override bool isDone
        {
            get
            {
                if (_request == null || !_request.isDone)
                    return false;
                return true;
            }
        }

        public BundleAsync(string name) : base(name)
        {
        }


        internal override void Load()
        {
            Retain();
            loadState = LoadState.LOADING;
            _request = AssetBundle.LoadFromFileAsync(name);
            if (_request == null)
            {
                Release();
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
