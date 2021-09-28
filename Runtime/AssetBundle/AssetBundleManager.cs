using UnityEngine;

namespace US
{
    public class AssetBundleManager : MonoSingleton<AssetBundleManager>
    {
        private Manifest manifest;

        private void Start()
        {
            var bundle = AssetBundle.LoadFromFile(ABConfig.PlatformBuildPath + "/assets/customassets.bundle");
            TextAsset oj = bundle.LoadAsset<TextAsset>("Assets/CustomAssets/file.json");
            manifest = new Manifest();
            JsonUtility.FromJsonOverwrite(oj.text, manifest);
            bundle.Unload(true);
        }
        private void Update()
        {

        }

        public void LoadManifest() { }
        public void UnloadManifest() { }

        public void LoadBundle() { }
        public void UnloadBundle() { }

        public void LoadBundleAsyc() { }
        public void UnloadBundleAsyc() { }

    }
}
