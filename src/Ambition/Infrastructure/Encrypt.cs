using System;
using System.Security.Cryptography;
using System.Text;

namespace Ambition.Infrastructure
{
    public static class Encrypt
    {
        public static string Md5Encrypt(string myString)
        {
            return Md5Encrypt32(myString).Substring(8, 16).ToLower();
        }

        public static string Md5Encrypt32(string myString)
        {
            var md5 = MD5.Create();
            byte[] fromData = Encoding.UTF8.GetBytes(myString);
            byte[] targetData = md5.ComputeHash(fromData);

            return BitConverter.ToString(targetData).Replace("-", "").ToLower();
        }
    }
}