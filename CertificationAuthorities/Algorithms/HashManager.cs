using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CertificationAuthorities.Algorithms
{
    public class HashManager
    {
        private MD5 md5Hash = MD5.Create();

        public HashManager()
        {

        }
        public string hash(string inputValue)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(inputValue));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public bool VerifyMd5Hash(string inputValue, string hashValue)
        {
            string hashOfInput = hash(inputValue);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            if (0 == comparer.Compare(hashOfInput, hashValue))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
