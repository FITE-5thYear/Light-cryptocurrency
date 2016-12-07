using System.Security.Cryptography;

namespace Server.Algorithms
{
    public class RSAAlgorithm
    {
        static public RSACryptoServiceProvider RSAServiceProvider = new RSACryptoServiceProvider(1024);
    }
}
