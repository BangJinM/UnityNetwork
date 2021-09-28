using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace US
{
    [System.Serializable]
    public class CustomAssetBundle
    {
        public uint id;
        public long size;
        public uint[] deps;
        public string path;
        public string abName;
    }

    public class Manifest : ScriptableObject
    {
        public string version;
        public List<CustomAssetBundle> bundles = new List<CustomAssetBundle>();
        private Dictionary<string, CustomAssetBundle> nameWithBundles = new Dictionary<string, CustomAssetBundle>();
    }
}