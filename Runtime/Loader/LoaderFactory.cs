namespace US
{
    public static class LoaderFactory
    {
        public static IResourceLoader CreateLoader(string path, LoadType loadType, LoadedCallback finishCallback = null)
        {
            IResourceLoader resourceLoader = null;
            switch (loadType)
            {
                case LoadType.ASSET:
                    resourceLoader = new AssetLoader(path, finishCallback);
                    break;

                case LoadType.ASSETBUNDLE:
                    resourceLoader = new AssetBundleLoader(path, finishCallback);
                    break;

                case LoadType.RESOURCES:
                    resourceLoader = new ResourceLoader(path, finishCallback);
                    break;

                default:
                    break;
            }
            return resourceLoader;
        }
    }
}