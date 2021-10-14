using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace US
{
    public class USAsset : Reference
    {
        public event Action<USAsset> completed;
        public string assetName;
        public Type assetType;
        private List<Object> objects = new List<Object>();
        public Object asset { get; internal set; }

        public void Add(Object obj)
        {
            objects.Add(obj);
        }

        public virtual void Update()
        {
            if (!IsDone())
                return;
            if (objects.Count > 0)
            {
                for (var i = objects.Count - 1; i >= 0; i--)
                {
                    var item = objects[i];
                    if (item != null)
                        continue;
                    objects.RemoveAt(i);
                    Release();
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

        public virtual void Load() { }
        public virtual void Unload() { }

        public virtual bool IsDone() { return true; }
    }


    [Serializable]
    public class ResourceAsset : USAsset
    {
        public ResourceAsset()
        {
        }

        public override void Load()
        {
            asset = Resources.Load(assetName);
        }

        public override void Unload()
        {
            asset = null;
        }
    }
#if UNITY_EDITOR
    [Serializable]
    public class AssetDataBaseAsset : USAsset
    {
        public AssetDataBaseAsset()
        {
        }

        public override void Load()
        {
            asset = AssetDatabase.LoadAssetAtPath(assetName, assetType);
        }

        public override void Unload()
        {
            asset = null;
        }
    }
#endif

    [Serializable]
    public class BundleAsset : USAsset
    {
        protected readonly string assetBundleName;
        protected Bundle bundle;

        public BundleAsset(string bundle)
        {
            assetBundleName = bundle;
        }

        public override void Load()
        {
            bundle = AssetBundleManager.Instance.LoadBundle(assetBundleName) as Bundle;
            asset = bundle.assetBundle.LoadAsset(assetName, assetType);
        }

        public override void Unload()
        {
            if (bundle != null)
            {
                bundle.Release();
            }
        }
    }

    public class BundleAnsycAsset : USAsset
    {
        protected readonly string assetBundleName;
        protected BundleAsync bundle = null;
        private AssetBundleRequest _request = null;

        public BundleAnsycAsset(string bundle)
        {
            assetBundleName = bundle;
        }

        public override void Update()
        {
            if (bundle == null || !bundle.isDone)
                return;
            if (_request == null)
            {
                _request = bundle._request.assetBundle.LoadAssetAsync(assetName, assetType);
                return;
            }
            if (IsDone()&&asset == null)
                asset = _request.asset;
            base.Update();
        }

        public override void Load()
        {
            bundle = AssetBundleManager.Instance.LoadBundle(assetBundleName, true) as BundleAsync;
        }

        public override void Unload()
        {
            bundle.Release();
            _request = null;
        }

        public override bool IsDone()
        {
            if (_request == null || bundle == null || !bundle.isDone || !_request.isDone)
                return false;
            return true;
        }
    }
}
