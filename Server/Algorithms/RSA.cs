using System.Security.Cryptography;

namespace Server.Algorithms
{
    public class RSA
    {
        public RSACryptoServiceProvider rsaSP { get; }
        
        public RSA(string containerName) {
            CspParameters cp = new CspParameters();
            cp.KeyContainerName = containerName;
            rsaSP = new RSACryptoServiceProvider(2048,cp);
        }

        public byte[] decrypt(byte[] data,string encryptionKey) {
            rsaSP.FromXmlString(encryptionKey);
            return rsaSP.Decrypt(data, false);
        }

        public byte[] encrypte(byte[] data,string encryptionKey)
        {
            rsaSP.FromXmlString(encryptionKey);
            return rsaSP.Encrypt(data, false);
        }


        public byte[] signData(byte[] data, string encryptionKey)
        {
            rsaSP.FromXmlString(encryptionKey);
            return rsaSP.SignData(data, new SHA1CryptoServiceProvider());
        }

        public bool verifyData(byte[] data, string encryptionKey,byte[] signature)
        {
            rsaSP.FromXmlString(encryptionKey);

            return rsaSP.VerifyData(data, new SHA1CryptoServiceProvider(), signature);
        }
    }
}
