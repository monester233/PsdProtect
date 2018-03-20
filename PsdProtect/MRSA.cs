using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace PsdProtect
{
    public class MRSA
    {
        public string RSAPublicKey;
        public string RSAPrivateKey;

        public string genKey()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            RSAPublicKey = rsa.ToXmlString(false);
            return rsa.ToXmlString(true);
        }

        /// <summary>
        /// 采用RSA算法加密
        /// </summary>
        /// <param name="content">明文</param>
        /// <param name="encode">明文编码方式</param>
        /// <returns>密文</returns>
        public string encodeRSA(string content, Encoding encode)
        {
            string strEncode = "";
            try
            {
                byte[] bytesIn = encode.GetBytes(content);
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(RSAPublicKey);
                byte[] bytesOut = rsa.Encrypt(bytesIn, false);
                strEncode = Convert.ToBase64String(bytesOut);
                return strEncode;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 采用RSA算法解密
        /// </summary>
        /// <param name="content">密文</param>
        /// <returns>明文</returns>
        public string decodeRSA(string content, Encoding encode)
        {
            string strDecode = "";
            try
            {
                /*
                byte[] bytesIn = new byte[content.Length];
                for (int i = 0; i < bytesIn.Length; i++)
                    bytesIn[i] = Convert.ToByte(content.Substring(i, 1), 16);*/
                byte[] bytesIn = Convert.FromBase64String(content);
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(RSAPrivateKey);
                byte[] bytesOut = rsa.Decrypt(bytesIn, false);
                strDecode = encode.GetString(bytesOut);
                return strDecode;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
