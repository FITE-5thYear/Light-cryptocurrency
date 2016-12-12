using System.Security.Cryptography;

namespace Server.Algorithms
{
    public class RSA
    {
        public RSACryptoServiceProvider rsaSP { get; }
        
        public RSA(string containerName) {
            CspParameters cp = new CspParameters();
            cp.KeyContainerName = containerName;
            rsaSP = new RSACryptoServiceProvider(cp);
        }

        public byte[] decrypt(byte[] data,string encryptionKey) {
            return rsaSP.Decrypt(data, false);
        }

        public byte[] encrypte(byte[] data,string encryptionKey)
        {
            rsaSP.FromXmlString(encryptionKey);
            return rsaSP.Encrypt(data, false);
        }
    }
}
