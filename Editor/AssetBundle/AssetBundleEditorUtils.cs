using UnityEditor;
using UnityEngine;

namespace US
{
    public static class AssetBundleEditorUtils
    {

        public static string RuledAssetBundleName(string name)
        {
            return name.Replace("\\", "/").ToLower() + ".unity3d";
        }

        public static MarkRules GetMarkRulesAssetData()
        {
            return GetScriptableObjectAsset<MarkRules>(Settings.MARK_RULES_PATH);
        }

        public static Manifest GetManifestAssetData()
        {
            return GetScriptableObjectAsset<Manifest>(Settings.MANIFEST_PATH);
        }

        public static T GetScriptableObjectAsset<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
            }

            return asset;
        }
    }

}