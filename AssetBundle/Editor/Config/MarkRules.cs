using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace US
{


    [System.Serializable]
    public class MarkRule
    {
        [Tooltip("搜索路径")] public string searchPath;

        [Tooltip("搜索通配符，多个之间请用,(;)隔开")] public string searchPattern = "";

        [Tooltip("刨除")] public string exclude = "";

        [Tooltip("搜索方式：DIR=打包目录下的所有子目录 File=目录下的所有文件")] public PackageMode buildType = PackageMode.All_FILE2PACKAGE;
    }

    [CreateAssetMenu(fileName = "MarkRules", menuName = "UnitySupport/CreateBuildRules", order = 0)]
    public class MarkRules : ScriptableObject
    {
        public List<MarkRule> abRules;
    }
}