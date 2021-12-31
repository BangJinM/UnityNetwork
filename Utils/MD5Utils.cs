using System;
using System.Security.Cryptography;
using System.IO;

namespace US
{
    public class MD5Utils
    {
        public static string ComputeMD5(Stream stream)
        {
            var md5 = MD5.Create();
            var fileMD5Bytes = md5.ComputeHash(stream);//计算指定Stream 对象的哈希值                                     
            string filemd5  = FormateStr(fileMD5Bytes);
            return filemd5;
        }

        public static string ComputeMD5(string fliePath)
        {
            string fileMD5 = "";
            using (var fileStream = File.OpenRead(fliePath))
            {
                fileMD5 = ComputeMD5(fileStream);
            }
            return fileMD5;
        }

        /// <summary>
        /// 将byte[]装换成字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string FormateStr(Byte[] data)
        {
            return System.BitConverter.ToString(data).Replace("-", "").ToLower();
        }
    }
}