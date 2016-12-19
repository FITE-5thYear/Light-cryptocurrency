using System.Security.Cryptography;
using System.Text;

namespace CertificationAuthorities.Algorithms
{
    class KeyManager
    {
        
        public static string serverRSAPublicKey;
        public static string RSAPublicKey { get; set; }
        public static string RSAPrivateKey { get; set; }

        

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
