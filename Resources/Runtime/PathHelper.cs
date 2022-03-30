using System.Collections;
using UnityEngine;
using System.IO;

namespace US
{
    public static class PathHelper
    {
        public static string AssetPath = "Res/";

        /// <summary>
        /// 获取规范化的路径
        /// </summary>
        public static string GetRegularPath(string path)
        {
            return path.Replace('\\', '/').Replace("\\", "/"); //替换为Linux路径格式
        }

        /// <summary>
        /// 获取文件所在的目录路径（Linux格式）
        /// </summary>
        public static string GetDirectory(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            return GetRegularPath(directory);
        }

        public static string GetStreamingRootPath()
        {
            return UnityEngine.Application.streamingAssetsPath + "/";
        }

        /// <summary>
        /// 获取基于流文件夹的加载路径
        /// </summary>
        public static string GetStreamingPath(string path)
        {
            return CombineAssetPath(GetStreamingRootPath(), path);
        }

        /// <summary>
        /// 获取文件夹路径
        /// </summary>
        public static string GetPersistentRootPath()
        {
#if UNITY_EDITOR
            return GetDirectory(UnityEngine.Application.dataPath +"/");
#else
			return UnityEngine.Application.persistentDataPath + "/";
#endif
        }

        /// <summary>
        /// 获取基于文件夹的加载路径
        /// </summary>
        public static string GetPersistentPath(string path)
        {
            string root = GetPersistentRootPath();
            return CombineAssetPath(root, path);
        }

        /// <summary>
        /// 获取网络资源加载路径
        /// </summary>
        public static string ConvertToWWWPath(string path)
        {
            // 注意：WWW加载方式，必须要在路径前面加file://
#if UNITY_EDITOR
            return string.Format("file:///{0}", path);
#elif UNITY_IPHONE
			return string.Format("file://{0}", path);
#elif UNITY_ANDROID
			return path;
#elif UNITY_STANDALONE
			return string.Format("file:///{0}", path);
#endif
        }

        /// <summary>
        /// 合并资源路径
        /// </summary>
        public static string CombineAssetPath(string root, string location)
        {
            if (string.IsNullOrEmpty(root))
                return location;
            else
                return $"{root}/{location}";
        }

        /// <summary>
        /// 根据相对路径，获取到完整路径，優先从下載資源目录找，没有就读本地資源目錄 
        /// </summary>
        /// <param name="url">相对路径</param>
        /// <returns></returns>
        public static string GetResourceFullPath(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return "";
            }
            string reaultUrl = GetPersistentPath(url);
            if (File.Exists(Path.GetFullPath(reaultUrl)))
            {
                return reaultUrl;
            }

            reaultUrl = GetStreamingPath(url);
            if (File.Exists(Path.GetFullPath(reaultUrl))) // 连本地资源都没有，直接失败
            {
                return reaultUrl;
            }

            return "";
        }
    }
}