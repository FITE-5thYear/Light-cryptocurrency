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

        public static void generateSessionKey() {
            var random = new RNGCryptoServiceProvider();
            var key = new byte[16];
            random.GetBytes(key);
            SessionKey = Encoding.UTF8.GetString(key);
        }
    }
}
