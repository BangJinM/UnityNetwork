using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace US
{
    [System.Serializable]
    public class Bundle
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
        public List<Bundle> bundles = new List<Bundle>();
        private Dictionary<string, Bundle> nameWithBundles = new Dictionary<string, Bundle>();

        public void Load(string path)
        {
            var json = File.ReadAllText(path);
            JsonUtility.FromJsonOverwrite(json, this);
        }
    }
}