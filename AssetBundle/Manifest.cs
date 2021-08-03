using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace US.AB
{
    [System.Serializable]
    public class Bundle
    {
        public string name;
        public string hash;
        public long size;
        public int id;
        public int[] deps;
    }

    [System.Serializable]
    public class Manifest : ScriptableObject
    {
        public List<Bundle> bundles = new List<Bundle>();
    }
}