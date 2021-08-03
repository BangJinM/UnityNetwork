using System;
using System.Security.Cryptography;
using System.IO;

namespace US.USMD5
{
    public class MD5Utils
    {
        public static string BuildFileMd5(string fliePath)
        {
            string filemd5 = null;
            using (var fileStream = File.OpenRead(fliePath))
            {
                var md5 = MD5.Create();
                var fileMD5Bytes = md5.ComputeHash(fileStream);//计算指定Stream 对象的哈希值                                     
                filemd5 = FormatMD5(fileMD5Bytes);
            }
            return filemd5;
        }

        public static string FormatMD5(Byte[] data)
        {
            return System.BitConverter.ToString(data).Replace("-", "").ToLower();//将byte[]装换成字符串
        }
    }
}