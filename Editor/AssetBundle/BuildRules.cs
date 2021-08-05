using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace US.AB
{

    public enum PackageType
    {
        DIR,    //文件夹下面所有文件夹单独出ab包
        FILE,   //文件夹下面单个文件出ab包
    }

    [System.Serializable]
    public class BuildRule
    {
        public string searchPath;
        public PackageType type = PackageType.DIR;
    }

    [System.Serializable]
    public class BuildRules : ScriptableObject
    {
        public List<BuildRule> buildRules = new List<BuildRule>();
    }

}