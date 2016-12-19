using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Server.Algorithms
{
    class KeysManager
    {
        private static int AES_KEY_LENGTH = 16;
        public static byte[] AESkey { get; set; }

        public static string RSAPublicKey { get; set; }
        public static string RSAPrivateKey { get; set; }

        public static void generateAESKey()
        {
            var random = new RNGCryptoServiceProvider();
            var key = new byte[AES_KEY_LENGTH];
            random.GetBytes(key);
            AESkey = key;
        }

        public static void generateRSAPublicKey(RSACryptoServiceProvider rsa)
        {
            RSAPublicKey = rsa.ToXmlString(false);
        
        }

        public static void generateRSAPrivateKey(RSACryptoServiceProvider rsa)
        {
            RSAPrivateKey = rsa.ToXmlString(true);
        }
    }
}
