using System;
using System.Security.Cryptography;
using System.Text;
using ZM.Extensions.DataEx;

namespace ZM.Extensions.SecurityCryptographyEx
{
    public static class SecurityCryptographyExtensions
    {
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="source"></param>
        /// <param name="strFormat">X为大写时加密后的数值里的字母为大写，x为小写时加密后的数值里的字母为小写,加密结果"x"16,"x2"结果为32位,"x3"结果为48位,"x4"结果为64位</param>
        /// <returns></returns>
        public static string MD5Encrypt(this string source, string strFormat = "X2")
        {
            byte[] sor = Encoding.UTF8.GetBytes(source);
            MD5 md5 = MD5.Create();
            byte[] data = md5.ComputeHash(sor);
            return data.BytesToString(strFormat);
        }
        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="source"></param>
        /// <param name="strFormat">X为大写时加密后的数值里的字母为大写，x为小写时加密后的数值里的字母为小写,加密结果"x"16,"x2"结果为32位,"x3"结果为48位,"x4"结果为64位</param>
        /// <returns></returns>
        public static string SHA1Encrypt(this string source, string strFormat = "X2")
        {
            SHA1 sha1 = SHA1.Create();
            byte[] data = sha1.ComputeHash(Encoding.UTF8.GetBytes(source));
            return data.BytesToString(strFormat);
        }
        public static string SHA256Encrypt(this string source, string strFormat = "X2")
        {
            var bytes = Encoding.Default.GetBytes(source);
            var SHA256 = new SHA256CryptoServiceProvider();
            var data = SHA256.ComputeHash(bytes);
            return data.BytesToString(strFormat);
        }
        public static string SHA384Encrypt(this string source, string strFormat = "X2")
        {
            var bytes = Encoding.Default.GetBytes(source);
            var SHA384 = new SHA384CryptoServiceProvider();
            var data = SHA384.ComputeHash(bytes);
            return data.BytesToString(strFormat);
        }
        public static string SHA512Encrypt(this string source, string strFormat = "X2")
        {
            var bytes = Encoding.Default.GetBytes(source);
            var SHA512 = new SHA512CryptoServiceProvider();
            var data = SHA512.ComputeHash(bytes);
            return data.BytesToString(strFormat);
        }

        /// <summary>
        ///  AES 加密
        /// </summary>
        /// <param name="str">明文（待加密）</param>
        /// <param name="key">密文</param>
        /// <returns></returns>
        public static string AesEncrypt(this string str, string key)
        {
            if (string.IsNullOrEmpty(str)) return null;
            Byte[] strBytes = Encoding.UTF8.GetBytes(str);
            var keyBytes = Encoding.UTF8.GetBytes(key);
            using (RijndaelManaged rm = new RijndaelManaged())
            {
                rm.Key = keyBytes;
                rm.Mode = CipherMode.ECB;
                rm.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform cTransform = rm.CreateEncryptor())
                {
                    Byte[] resultArray = cTransform.TransformFinalBlock(strBytes, 0, strBytes.Length);
                    return Convert.ToBase64String(resultArray);
                }
            }
        }

        /// <summary>
        ///  AES 解密
        /// </summary>
        /// <param name="str">明文（待解密）</param>
        /// <param name="key">密文</param>
        /// <returns></returns>
        public static string AesDecrypt(this string str, string key)
        {
            if (string.IsNullOrEmpty(str)) return null;
            Byte[] strBytes = Convert.FromBase64String(str);
            var keyBytes = Encoding.UTF8.GetBytes(key);
            using (RijndaelManaged rm = new RijndaelManaged())
            {
                rm.Key = keyBytes;
                rm.Mode = CipherMode.ECB;
                rm.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform cTransform = rm.CreateDecryptor())
                {
                    Byte[] resultArray = cTransform.TransformFinalBlock(strBytes, 0, strBytes.Length);
                    return Encoding.UTF8.GetString(resultArray);
                }
            }
        }
    }

}

