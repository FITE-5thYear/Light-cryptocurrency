using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Client.Algorithms
{
    class KeyGenerator
    {
        public static byte[] getRandomKeyByteArray()
        {
            var random = new RNGCryptoServiceProvider();
            var key = new byte[16];
            random.GetBytes(key);
            return key;
        }

        public static string getRandomKeyString()
        {
            var random = new RNGCryptoServiceProvider();
            var key = new byte[16];
            random.GetBytes(key);
            return Convert.ToBase64String(key);
        }
    }
}
