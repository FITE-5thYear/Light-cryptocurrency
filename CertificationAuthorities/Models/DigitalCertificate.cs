using System;
using Newtonsoft.Json;
using System.Text;
using CertificationAuthorities.Algorithms;
using CertificationAuthorities.Util;


namespace CertificationAuthorities.Models
{
    class DigitalCertificate
    {
        public static string IssuerName="Certification Authority";
        public int SerialNumber;
        public string SubjectName;
        public string SubjectPublicKey;
        public DateTime IssuingDate;
        public byte[] DigitalSignature;

        public DigitalCertificate() { }
        public DigitalCertificate(int number, string Name,DateTime Time,string PublicKey)
        {
            SerialNumber = number;
            SubjectName = Name;
            SubjectPublicKey = PublicKey;
            IssuingDate = Time;
            sign();
        }

       
        public void sign()
        {
            byte[] message = Encoding.UTF8.GetBytes(ToString());
            DigitalSignature= ServerObject.rsa.signData(message, KeyManager.RSAPrivateKey);
        }
        public override string ToString()
        {
            string s;
            s = "Issuer Name: " + IssuerName + "\n" + "Owner: " + SubjectName + "\n"
                + "Owner public key: " + SubjectPublicKey + "\n" +
                "Date validate :" + IssuingDate.Date.ToString();
            return s;
        }
        public string toJsonObject()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static DigitalCertificate newClientObject(string jsonObject)
        {

            return JsonConvert.DeserializeObject<DigitalCertificate>(jsonObject);
        }
    }
}
