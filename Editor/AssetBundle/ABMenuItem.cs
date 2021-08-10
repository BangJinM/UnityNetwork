using UnityEditor;
using System.Collections.Generic;

namespace US
{
    public static class ABMenuItem
    {

        public static Dictionary<string, string> path2assetbundle = new Dictionary<string, string>();

        [MenuItem("UnitySupport/AssetBundle/Build")]
        public static void BuildRules()
        {
            IResourceLoader resourceLoader = LoaderFactory.CreateLoader("Assets/CustomAssets/Rules.asset", LoadType.ASSET, (IResourceLoader loader) =>
            {
                BuildRules buildRules = (BuildRules)loader.GetResult();
                buildRules.Build(ref path2assetbundle);
            });
            resourceLoader.Load();
        }
    }
}
