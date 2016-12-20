using System;
using Newtonsoft.Json;
using System.Text;
using Server.Algorithms;
using Server.Util;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public  class DigitalCertificate
    {
        public static string IssuerName = "Certification Authority";
        public int SerialNumber;
        public string SubjectName;
        public string SubjectPublicKey;
        public DateTime IssuingDate;
        public byte[] DigitalSignature;

        public DigitalCertificate() { }
        public DigitalCertificate(int number, string Name, DateTime Time, string PublicKey)
        {
            SerialNumber = number;
            SubjectName = Name;
            SubjectPublicKey = PublicKey;
            IssuingDate = Time;
        }



        public override string ToString()
        {
            string s;
            s = "Issuer Name: " + IssuerName + "\n" + "Owner: " + SubjectName + "\n"
                + "Owner public key: " + SubjectPublicKey + "\n" +
                "Date validate :" + IssuingDate.Date.ToString()
                + "Owner Public Key"
                 + SubjectPublicKey + "\n";
            return s;
            return s;
        }
        public bool verviy()

        {
            byte[] bmsg = Encoding.UTF8.GetBytes(ToString());
            RSA rsa = new RSA("Server");
            return (rsa.verifyData(bmsg,KeysManager.RSAPcublicKeyOfCA, DigitalSignature));

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
