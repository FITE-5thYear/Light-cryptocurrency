using System.Security.Cryptography;

namespace Server.Algorithms
{
    public class KeysManager
    {

        public static byte[] AESPublicKey;
        public static string RSAPublicKey;

        public static void generateAESPublicKey()
        {
            var random = new RNGCryptoServiceProvider();
            var key = new byte[16];
            random.GetBytes(key);
            AESPublicKey = key;
        }

        public static void generateRSAPublicKey(RSACryptoServiceProvider RSA) {
            RSAPublicKey = RSA.ToXmlString(false);
        }
    }
}
