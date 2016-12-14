using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Client.Algorithms
{
    class KeyManager
    {
        public static string serverAESPublicKey;
        public static string serverRSAPublicKey;
        public static string  SessionKey;
        public static string RSAPublicKey { get; set; }
        public static string RSAPrivateKey { get; set; }

        public static void generateSessionKey() {
            var random = new RNGCryptoServiceProvider();
            var key = new byte[16];
            random.GetBytes(key);
            SessionKey = Encoding.UTF8.GetString(key);
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
