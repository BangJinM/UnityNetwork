using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace US
{
    [Serializable]
    public class USAssetBundle
    {
        public uint id;
        public long size;
        public uint[] deps;
        public string assetName;
        public string abName;
    }

    [Serializable]
    public class Manifest : ScriptableObject
    {
        public string version;
        public List<USAssetBundle> bundles = new List<USAssetBundle>();

        public Dictionary<string, uint> pathIDDict = new Dictionary<string, uint>();
        public Dictionary<uint, USAssetBundle> idBundlsDict = new Dictionary<uint, USAssetBundle>();

        public bool inited = false;
        public void Init()
        {
            foreach (var bundle in bundles)
            {
                pathIDDict.Add(bundle.abName, bundle.id);
                idBundlsDict.Add(bundle.id, bundle);
            }
            inited = true;
        }

        public uint GetID(string abName)
        {
            uint result;
            if (pathIDDict.TryGetValue(abName, out result))
                return result;
            return 0;
        }

        public uint[] GetDeps(uint id)
        {
            USAssetBundle bundle = null;
            if (idBundlsDict.TryGetValue(id, out bundle))
                return bundle.deps;
            return null;
        }

        public USAssetBundle GetBundle(uint id)
        {
            USAssetBundle bundle = null;
            if (idBundlsDict.TryGetValue(id, out bundle))
                return bundle;
            return null;
        }
    }
}