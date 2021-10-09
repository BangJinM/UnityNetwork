using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace US
{
    [Serializable]
    public class BundleAsset : Reference
    {
        public event Action<BundleAsset> completed;

        protected readonly string assetBundleName;
        protected Bundle bundle;

        public string assetName;
        public Type assetType;

        public LoadState loadState;

        private List<Object> objects = new List<Object>();

        public Object asset { get; internal set; }

        public virtual bool isDone
        {
            get
            {
                return true;
            }
        }

        public BundleAsset(string bundle)
        {
            assetBundleName = bundle;
            loadState = LoadState.INIT;
        }

        public virtual void Update()
        {
            if (!isDone)
                return;
            if (objects.Count > 0)
            {
                for (var i = objects.Count - 1; i >= 0; i--)
                {
                    var item = objects[i];
                    if (item != null)
                        continue;
                    Release();
                    objects.RemoveAt(i);
                }
            }
            if (completed == null)
                return;
            try
            {
                completed?.Invoke(this);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            completed = null;
        }

        internal virtual void Load()
        {
            bundle = AssetBundleManager.Instance.LoadBundle(assetBundleName);
            asset = bundle.assetBundle.LoadAsset(assetName, assetType);
            loadState = LoadState.FINISHED;
        }

        internal virtual void Unload()
        {
            if (bundle != null)
            {
                AssetBundleManager.Instance.UnloadBundle(assetBundleName);
            }
            loadState = LoadState.UNLOAD;
            asset = null;
        }
    }

    public class BundleAnsycAsset : BundleAsset
    {
        private AssetBundleRequest _request;

        public override bool isDone
        {
            get
            {
                if (loadState == LoadState.FINISHED)
                    return true;
                if (!bundle.isDone)
                    return false;

                return true;
            }
        }

        public BundleAnsycAsset(string bundle) : base(bundle)
        {
        }

        public override void Update()
        {
            if (_request.isDone)
            {
                loadState = LoadState.FINISHED;
            }
        }

        internal override void Load()
        {
            bundle = AssetBundleManager.Instance.LoadBundle(assetBundleName, true);
            loadState = LoadState.LOADING;
        }

        internal override void Unload()
        {
            base.Unload();
            _request = null;
        }
    }
}
